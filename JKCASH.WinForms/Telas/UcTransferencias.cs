using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Excecoes;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcTransferencias : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    public event EventHandler? TransferenciaRealizada;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private readonly TransferenciaServico? _servico;
    private List<OpcaoConta> _contasAtivas = new();
    private bool _ocupado, _preenchendo, _consultaValida;

    public UcTransferencias()
    {
        InitializeComponent();
        cmbImportanciaTransferencia.DataSource = Enum.GetValues<ImportanciaTransacao>();
        cmbPlanejamentoTransferencia.DataSource = Enum.GetValues<PlanejamentoTransacao>();
        cmbUrgenciaTransferencia.DataSource = Enum.GetValues<UrgenciaTransacao>();
        foreach (ComboBox combo in new[] { cmbImportanciaTransferencia, cmbPlanejamentoTransferencia, cmbUrgenciaTransferencia })
        {
            combo.FormattingEnabled = true;
            combo.Format += (_, e) => e.Value = e.ListItem switch
            {
                ImportanciaTransacao.Media => "Média",
                PlanejamentoTransacao.NaoPlanejada => "Não planejada",
                UrgenciaTransacao.SemUrgencia => "Sem urgência",
                _ => e.Value
            };
        }
        lblHojeTransferencia.Text = DateTime.Today.ToString("dd/MM/yyyy");
        cmbOrigemTransferencia.SelectedValueChanged += (_, _) =>
        {
            if (!_preenchendo) AtualizarDestinos();
        };
        cmbDestinoTransferencia.SelectedValueChanged += (_, _) => AtualizarSaldos();
        numValorTransferencia.ValueChanged += (_, _) => AtualizarSaldos();
    }

    public UcTransferencias(IContaRepositorio contas, ICategoriaRepositorio categorias, TransferenciaServico servico, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(servico);
        _contas = contas;
        _categorias = categorias;
        _servico = servico;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        Load += async (_, _) => await AtualizarDadosAsync();
        btnAtualizarTransferencia.Click += async (_, _) => await AtualizarDadosAsync();
        btnConfirmarTransferencia.Click += async (_, _) => await ConfirmarTransferenciaAsync();
    }

    public async Task AtualizarDadosAsync()
    {
        if (_ocupado || _contas is null || _categorias is null || IsDisposed || Disposing) return;
        Ocupar(true);
        txtResultadoTransferencia.Text = "Consultando contas e categorias…";
        try
        {
            await RecarregarCoreAsync();
            if (IsDisposed) return;
            txtResultadoTransferencia.Text = _contasAtivas.Count < 2
                ? "Cadastre pelo menos duas contas ativas para transferir."
                : cmbCategoriaTransferencia.Items.Count == 0 ? "Cadastre uma categoria para classificar a transferência."
                : "Selecione a origem, o destino e preencha os dados. Ao confirmar, os dois saldos e os dois registros serão salvos juntos.";
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            InvalidarConsulta();
            txtResultadoTransferencia.Text = "Não foi possível carregar os dados. Confira a conexão e clique em Atualizar contas.";
        }
        finally { Ocupar(false); }
    }

    private async Task RecarregarCoreAsync()
    {
        if (_contas is null || _categorias is null) return;
        Guid? origem = cmbOrigemTransferencia.SelectedValue as Guid?;
        Guid? categoria = cmbCategoriaTransferencia.SelectedValue as Guid?;
        var dados = await Task.Run(() => (_contas.ListarTodas(), _categorias.ListarTodas()));
        if (IsDisposed) return;
        _contasAtivas = dados.Item1.Where(c => c.Ativa).OrderBy(c => c.Nome).ThenBy(c => c.Banco)
            .Select(c => new OpcaoConta(c.Id, $"{c.Nome} ({c.Banco})", c.Saldo.SaldoAtual, c.Saldo.DisponivelComChequeEspecial)).ToList();
        _preenchendo = true;
        try
        {
            cmbOrigemTransferencia.DisplayMember = nameof(OpcaoConta.Nome);
            cmbOrigemTransferencia.ValueMember = nameof(OpcaoConta.Id);
            cmbOrigemTransferencia.DataSource = _contasAtivas.ToList();
            if (origem.HasValue && _contasAtivas.Any(c => c.Id == origem)) cmbOrigemTransferencia.SelectedValue = origem.Value;
            cmbCategoriaTransferencia.DisplayMember = nameof(Categoria.Nome);
            cmbCategoriaTransferencia.ValueMember = nameof(Categoria.Id);
            cmbCategoriaTransferencia.DataSource = dados.Item2;
            if (categoria.HasValue && dados.Item2.Any(c => c.Id == categoria)) cmbCategoriaTransferencia.SelectedValue = categoria.Value;
            lblHojeTransferencia.Text = DateTime.Today.ToString("dd/MM/yyyy");
            _consultaValida = true;
        }
        finally { _preenchendo = false; }
        AtualizarDestinos();
    }

    private void AtualizarDestinos()
    {
        Guid? destino = cmbDestinoTransferencia.SelectedValue as Guid?;
        Guid? origem = cmbOrigemTransferencia.SelectedValue as Guid?;
        var destinos = _contasAtivas.Where(c => c.Id != origem).ToList();
        cmbDestinoTransferencia.DisplayMember = nameof(OpcaoConta.Nome);
        cmbDestinoTransferencia.ValueMember = nameof(OpcaoConta.Id);
        cmbDestinoTransferencia.DataSource = destinos;
        if (destino.HasValue && destinos.Any(c => c.Id == destino)) cmbDestinoTransferencia.SelectedValue = destino.Value;
        AtualizarSaldos();
        AtualizarBotao();
    }

    private void AtualizarSaldos()
    {
        if (cmbOrigemTransferencia.SelectedItem is not OpcaoConta origem)
        {
            lblSaldosTransferencia.Text = "Selecione duas contas ativas para consultar os saldos.";
            return;
        }
        string saldoOrigem = $"Origem: saldo {Moeda(origem.Saldo)} • disponível com cheque especial: {Moeda(origem.Disponivel)}.";
        string saldoDestino = cmbDestinoTransferencia.SelectedItem is OpcaoConta destino
            ? $"Destino: saldo {Moeda(destino.Saldo)}." : "Selecione uma conta de destino diferente da origem.";
        lblSaldosTransferencia.Text = saldoOrigem + Environment.NewLine + saldoDestino;
        lblSaldosTransferencia.ForeColor = numValorTransferencia.Value > origem.Disponivel ? EstiloVisual.Vermelho : EstiloVisual.Secundario;
    }

    private async Task ConfirmarTransferenciaAsync()
    {
        if (_ocupado || _servico is null || IsDisposed || Disposing) return;
        Transferencia? salva = null;
        try
        {
            if (!_consultaValida) throw new ArgumentException("Atualize as contas antes de transferir.");
            if (cmbOrigemTransferencia.SelectedValue is not Guid origemId ||
                cmbDestinoTransferencia.SelectedValue is not Guid destinoId ||
                cmbCategoriaTransferencia.SelectedValue is not Guid categoriaId)
                throw new ArgumentException("Selecione a origem, o destino e a categoria.");
            if (origemId == destinoId) throw new ArgumentException("Escolha duas contas diferentes.");
            if (cmbImportanciaTransferencia.SelectedItem is not ImportanciaTransacao importancia ||
                cmbPlanejamentoTransferencia.SelectedItem is not PlanejamentoTransacao planejamento ||
                cmbUrgenciaTransferencia.SelectedItem is not UrgenciaTransacao urgencia)
                throw new ArgumentException("Informe importância, planejamento e urgência.");
            decimal valor = numValorTransferencia.Value;
            string descricao = txtDescricaoTransferencia.Text, justificativa = txtJustificativaTransferencia.Text;
            Ocupar(true);
            Transferencia previa = new(origemId, destinoId, valor, descricao, DateTime.Now, categoriaId, importancia, planejamento, urgencia, justificativa);
            OpcaoConta origemPrevia = _contasAtivas.Single(c => c.Id == origemId);
            OpcaoConta destinoPrevia = _contasAtivas.Single(c => c.Id == destinoId);
            if (valor > origemPrevia.Disponivel) throw new ArgumentException("Saldo e cheque especial insuficientes.");
            if (!_confirmacao.Confirmar(this, "Registrar transferência", ResumoOperacao.Transferencia(previa, origemPrevia.Nome, destinoPrevia.Nome) + "\nCategoria: " + cmbCategoriaTransferencia.Text))
            { txtResultadoTransferencia.Text = "Transferência cancelada. Preenchimento preservado."; return; }
            txtResultadoTransferencia.Text = "Registrando transferência…";
            // O serviço lê as contas novamente e confirma a disponibilidade atual no repositório.
            salva = await Task.Run(() => _servico.Registrar(origemId, destinoId, valor, categoriaId,
                importancia, planejamento, urgencia, descricao, justificativa));
            if (IsDisposed) return;
            numValorTransferencia.Value = 0m;
            txtDescricaoTransferencia.Clear();
            txtJustificativaTransferencia.Clear();
            TransferenciaRealizada?.Invoke(this, EventArgs.Empty);
            await RecarregarCoreAsync();
            if (IsDisposed) return;
            OpcaoConta? origem = _contasAtivas.FirstOrDefault(c => c.Id == origemId);
            OpcaoConta? destino = _contasAtivas.FirstOrDefault(c => c.Id == destinoId);
            txtResultadoTransferencia.Text = $"Transferência de {Moeda(salva.Valor)} registrada em {salva.Saida.Data:dd/MM/yyyy HH:mm}.\r\n"
                + $"Origem: {origem?.Nome} • saldo atualizado: {(origem is null ? "Consulte o extrato" : Moeda(origem.Saldo))}.\r\n"
                + $"Destino: {destino?.Nome} • saldo atualizado: {(destino is null ? "Consulte o extrato" : Moeda(destino.Saldo))}.\r\n"
                + "Os dois registros estão disponíveis no extrato. Transferências internas não compõem receitas e despesas.";
        }
        catch (ConflitoFinanceiroException erro) when (salva is null)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            try
            {
                await RecarregarCoreAsync();
                if (!IsDisposed) txtResultadoTransferencia.Text = erro.Message +
                    "\r\nSaldos atualizados. Confira os dados antes de confirmar novamente.";
            }
            catch (Exception falhaConsulta)
            {
                Debug.WriteLine(falhaConsulta);
                if (IsDisposed) return;
                InvalidarConsulta();
                txtResultadoTransferencia.Text = "A transferência não foi confirmada. Não foi possível atualizar os saldos; use Atualizar contas antes de tentar novamente.";
            }
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            if (salva is not null)
            {
                InvalidarConsulta();
                txtResultadoTransferencia.Text = "A transferência foi salva, mas a consulta dos saldos falhou. Atualize as contas ou consulte o extrato para conferir.";
            }
            else if (FalhaTela.EhValidacao(erro))
                txtResultadoTransferencia.Text = erro.Message;
            else
            {
                InvalidarConsulta();
                txtResultadoTransferencia.Text = "Não foi possível concluir a operação. Confira a conexão, atualize os saldos e consulte o extrato antes de tentar novamente.";
            }
        }
        finally { Ocupar(false); }
    }

    private void InvalidarConsulta()
    {
        _consultaValida = false;
        _contasAtivas.Clear();
        _preenchendo = true;
        try
        {
            cmbOrigemTransferencia.DataSource = null;
            cmbDestinoTransferencia.DataSource = null;
            cmbCategoriaTransferencia.DataSource = null;
        }
        finally { _preenchendo = false; }
        lblSaldosTransferencia.Text = "Saldos indisponíveis. Atualize as contas para consultar novamente.";
    }

    private void Ocupar(bool ocupado)
    {
        _ocupado = ocupado;
        if (IsDisposed) return;
        UseWaitCursor = ocupado;
        tlpTransferencia.Enabled = !ocupado;
        txtDescricaoTransferencia.Enabled = !ocupado;
        txtJustificativaTransferencia.Enabled = !ocupado;
        btnAtualizarTransferencia.Enabled = !ocupado;
        AtualizarBotao();
    }

    private void AtualizarBotao()
    {
        btnConfirmarTransferencia.Enabled = !_ocupado && _consultaValida && _servico is not null &&
            cmbOrigemTransferencia.SelectedValue is Guid origem && cmbDestinoTransferencia.SelectedValue is Guid destino &&
            origem != destino && cmbCategoriaTransferencia.Items.Count > 0;
    }

    private static string Moeda(decimal valor) => valor.ToString("C2", TabelaVisual.Cultura);
    private sealed record OpcaoConta(Guid Id, string Nome, decimal Saldo, decimal Disponivel);
}
