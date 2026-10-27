using System.Diagnostics;
using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Backup;
using JKCASH.Infrastructure.Integracoes.Api;

namespace JKCASH.WinForms;

public partial class FrmInicial
{
    private BackupApiCliente? _backupsApi;
    private BackupLocal? _backupLocal;
    private HistoricoBackup? _historico;
    private bool _operandoBackup, _armazenamentoDisponivel;

    public FrmInicial(ConexaoApiCliente conexao, BackupApiCliente backups,
        BackupLocal local, HistoricoBackup historico) : this(conexao)
    {
        _backupsApi = backups;
        _backupLocal = local;
        _historico = historico;
        btnEnviarBackup.Click += async (_, _) => await EnviarBackupAsync();
        btnRestaurarBackup.Click += async (_, _) => await RestaurarBackupAsync();
        btnReceberDadosSql.Click += async (_, _) => await ReceberDadosSqlAsync();
        btnHistorico.Click += (_, _) => MostrarHistorico();
        AtualizarHistorico();
    }

    private void AtualizarAcoesBackup()
    {
        if (IsDisposed || Disposing) return;
        bool livre = !_operandoBackup && !_verificando;
        btnEnviarBackup.Enabled = livre && _armazenamentoDisponivel && _backupLocal is not null &&
            File.Exists(_backupLocal.CaminhoBanco);
        btnRestaurarBackup.Enabled = livre && _armazenamentoDisponivel && _backupsApi is not null;
        btnReceberDadosSql.Enabled = livre && _apiDisponivel && _backupsApi is not null;
        btnVerificarConexao.Enabled = livre && _clienteApi is not null;
        btnHistorico.Enabled = !_operandoBackup && _historico is not null;
        btnAbrirAplicativo.Enabled = !_operandoBackup;
    }

    private void AtualizarHistorico()
    {
        try
        {
            var itens = _historico?.Ler() ?? [];
            string Ultimo(string operacao) => itens.FirstOrDefault(i =>
                i.Operacao == operacao && i.Estado == "Concluído")?.DataUtc.ToLocalTime()
                .ToString("dd/MM/yyyy HH:mm:ss") ?? "Nenhum confirmado nesta máquina";
            lblEstadoEnvio.Text = Ultimo("Envio");
            lblEstadoRestauracao.Text = itens.FirstOrDefault(i =>
                (i.Operacao == "Restauração" || i.Operacao == "Importação SQL") && i.Estado == "Concluído")
                ?.DataUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") ?? "Nenhuma concluída nesta máquina";
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            lblEstadoEnvio.Text = "Histórico local indisponível";
            lblEstadoRestauracao.Text = "Histórico local indisponível";
        }
    }

    private string Registrar(string operacao, string estado, Guid? id, string mensagem)
    {
        try { _historico?.Registrar(operacao, estado, id, mensagem); AtualizarHistorico(); return ""; }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            return "\nA operação foi processada, mas o histórico local não pôde ser atualizado.";
        }
    }

    private async Task EnviarBackupAsync()
    {
        if (_operandoBackup || _verificando || _backupsApi is null || _backupLocal is null) return;
        if (MessageBox.Show(this, "Enviar uma cópia completa dos dados locais ao servidor?\nOs dados locais serão preservados.",
            "Enviar backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

        _operandoBackup = true;
        AtualizarAcoesBackup();
        BackupEnvio? envio = null;
        try
        {
            lblOrientacao.Text = "Preparando uma cópia consistente dos dados locais…";
            envio = await Task.Run(_backupLocal.PrepararEnvio);
            // Registra a intenção antes do pedido, para não ocultar uma operação interrompida.
            Registrar("Envio", "Iniciado", envio.Id, "Aguardando confirmação do servidor.");
            lblOrientacao.Text = "Enviando backup. Aguarde a confirmação do servidor…";
            using var prazo = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            BackupResumo confirmado = await _backupsApi.EnviarAsync(envio, prazo.Token);
            if (confirmado.Id != envio.Id || confirmado.Sha256 != envio.Sha256 ||
                confirmado.TamanhoBytes != envio.Banco.LongLength)
                throw new InvalidDataException("O servidor retornou uma confirmação diferente do envio.");
            string aviso = Registrar("Envio", "Concluído", confirmado.Id,
                $"Armazenado no servidor em {confirmado.CriadoEmUtc:O}.");
            lblOrientacao.Text = "Backup confirmado pelo servidor. Seus dados locais continuam preservados.";
            MessageBox.Show(this, "Backup armazenado com sucesso!" + aviso, "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            string mensagem = envio is null ? erro.Message :
                "Não foi possível confirmar o envio. Confira os backups disponíveis antes de enviar outra cópia.";
            Registrar("Envio", envio is null ? "Falhou" : "Sem confirmação", envio?.Id, mensagem);
            lblOrientacao.Text = mensagem;
            MessageBox.Show(this, mensagem, "Backup não confirmado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { _operandoBackup = false; AtualizarAcoesBackup(); }
    }

    private async Task RestaurarBackupAsync()
    {
        if (_operandoBackup || _verificando || _backupsApi is null || _backupLocal is null) return;
        _operandoBackup = true;
        AtualizarAcoesBackup();
        Guid? escolhido = null;
        try
        {
            using var prazo = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            lblOrientacao.Text = "Consultando backups do servidor…";
            var backups = await _backupsApi.ListarAsync(prazo.Token);
            if (backups.Count == 0)
            {
                MessageBox.Show(this, "Ainda não há backups armazenados no servidor.", "JKCASH");
                return;
            }
            using var selecao = new FrmBackups(backups);
            if (selecao.ShowDialog(this) != DialogResult.OK || selecao.Escolhido is not BackupResumo resumo) return;
            escolhido = resumo.Id;
            lblOrientacao.Text = "Baixando e conferindo o backup. O banco local ainda não foi alterado…";
            // Novo prazo após o tempo gasto pelo usuário na escolha.
            using var download = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            BackupPacote pacote = await _backupsApi.ReceberAsync(resumo.Id, download.Token);
            if (pacote.Resumo != resumo) throw new InvalidDataException("O backup recebido não corresponde à cópia escolhida.");
            await ConferirEAplicarAsync(pacote, "Restauração",
                $"Backup de {resumo.CriadoEmUtc.ToLocalTime():dd/MM/yyyy HH:mm:ss}\nComputador: {resumo.Maquina}");
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            string mensagem = "A restauração não foi concluída. O banco local anterior foi preservado.";
            Registrar("Restauração", "Falhou", escolhido, mensagem + " " + erro.Message);
            lblOrientacao.Text = mensagem;
            MessageBox.Show(this, mensagem + "\n\n" +
                (erro is InvalidDataException or IOException ? erro.Message : "Confira a conexão e tente novamente."),
                "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _operandoBackup = false;
            if (lblOrientacao.Text.StartsWith("Baixando") || lblOrientacao.Text.StartsWith("Consultando"))
                lblOrientacao.Text = "Operação encerrada. Você pode abrir o aplicativo usando os dados locais.";
            AtualizarAcoesBackup();
        }
    }

    private void MostrarHistorico()
    {
        try
        {
            using var tela = new Form { Text = "JKCASH — Histórico local de backups", ClientSize = new Size(820, 420),
                StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 10) };
            var tabela = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _historico!.Ler().Select(i => new
                { Data = i.DataUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), i.Operacao, i.Estado, i.BackupId, i.Mensagem }).ToList() };
            tela.Controls.Add(tabela);
            tela.ShowDialog(this);
        }
        catch (Exception erro) { Debug.WriteLine(erro); MessageBox.Show(this, "Não foi possível ler o histórico local.", "JKCASH"); }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_operandoBackup)
        {
            e.Cancel = true;
            lblOrientacao.Text = "Aguarde a operação de backup terminar antes de fechar ou abrir o aplicativo.";
        }
        base.OnFormClosing(e);
    }
}
