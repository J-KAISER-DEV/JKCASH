using System.Diagnostics;
using System.Net;
using JKCASH.Application.Modelos;

namespace JKCASH.WinForms;

public partial class FrmInicial
{
    private async Task ReceberDadosSqlAsync()
    {
        if (_operandoBackup || _verificando || _backupsApi is null || _backupLocal is null) return;
        _operandoBackup = true;
        AtualizarAcoesBackup();
        Guid? recebido = null;
        try
        {
            lblOrientacao.Text = "Lendo os dados existentes no SQL Server pela API…\r\nOs dados locais ainda não foram alterados.";
            using var prazo = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            BackupPacote pacote = await _backupsApi.ReceberDadosSqlAsync(prazo.Token);
            recebido = pacote.Resumo.Id;
            await ConferirEAplicarAsync(pacote, "Importação SQL",
                $"Dados existentes no SQL Server\nConsulta de {pacote.Resumo.CriadoEmUtc.ToLocalTime():dd/MM/yyyy HH:mm:ss}");
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            string detalhe = erro switch
            {
                HttpRequestException { StatusCode: HttpStatusCode.ServiceUnavailable } =>
                    "Confira o SQL Server e se todas as tabelas e colunas estão atualizadas.",
                HttpRequestException { StatusCode: HttpStatusCode.UnprocessableEntity } =>
                    "Os dados da origem não puderam ser convertidos. Confira o log da API.",
                OperationCanceledException => "A operação excedeu o tempo de espera. Tente novamente.",
                InvalidDataException or IOException => erro.Message,
                _ => "Confira a conexão e a execução da API e tente novamente."
            };
            string mensagem = "A importação não foi concluída. O banco local anterior foi preservado.";
            Registrar("Importação SQL", "Falhou", recebido, mensagem + " " + detalhe);
            lblOrientacao.Text = mensagem;
            MessageBox.Show(this, mensagem + "\n\n" + detalhe, "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { _operandoBackup = false; AtualizarAcoesBackup(); }
    }

    // Restauração de backup e importação do SQL usam a mesma proteção do banco local.
    private async Task ConferirEAplicarAsync(BackupPacote pacote, string operacao, string origem)
    {
        var conferencia = await Task.Run(() => _backupLocal!.ConferirRecebido(pacote));
        Registrar("Recebimento", "Concluído", pacote.Resumo.Id,
            operacao + ": dados recebidos e conferidos; o banco local ainda não foi substituído.");
        string texto = origem + "\n\n" + conferencia.Descricao +
            "\n\nEsta operação SUBSTITUI os dados locais; não combina os históricos." +
            "\nUma cópia do banco local atual será preservada antes da troca." +
            "\nO SQL Server de origem não será alterado.\n\nDeseja continuar?";
        if (MessageBox.Show(this, texto, "Confirmar " + operacao.ToLowerInvariant(),
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            Registrar(operacao, "Cancelada", pacote.Resumo.Id, "O usuário manteve os dados locais.");
            lblOrientacao.Text = "Operação cancelada. Seus dados locais foram mantidos.";
            return;
        }
        // A intenção precisa ser persistida antes de substituir o arquivo local.
        _historico!.Registrar(operacao, "Iniciada", pacote.Resumo.Id, "Preparando a substituição do banco local.");
        lblOrientacao.Text = "Preparando a cópia de segurança e aplicando os dados locais…";
        string? seguranca = await Task.Run(() => _backupLocal!.Restaurar(pacote));
        string aviso = Registrar(operacao, "Concluído", pacote.Resumo.Id,
            seguranca is null ? "Banco local criado a partir dos dados recebidos." : $"Cópia anterior preservada em: {seguranca}");
        lblEstadoLocal.Text = "Banco local atualizado";
        lblOrientacao.Text = operacao + " concluída. Você já pode abrir o aplicativo.";
        MessageBox.Show(this, operacao + " concluída com sucesso!" +
            (seguranca is null ? "" : $"\n\nBanco anterior preservado em:\n{seguranca}") + aviso,
            "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
