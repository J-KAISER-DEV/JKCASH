using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Excecoes;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcCompraCartao : UserControl
{
    private readonly ICartaoCreditoRepositorio? _cartoes;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private readonly CompraCartaoServico? _servicoCompras;
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private bool _operacaoCompra, _consultaCartoesValida;

    public UcCompraCartao()
    {
        InitializeComponent();
        ConfigurarCompras();
        cmbCartaoCompra.SelectedValueChanged += (_, _) => AtualizarLimiteCompra();
        numValorCompra.ValueChanged += (_, _) => AtualizarLimiteCompra();
    }

    public UcCompraCartao(ICartaoCreditoRepositorio cartoes, IContaRepositorio contas,
        ICategoriaRepositorio categorias, CompraCartaoServico servico, IConfirmacaoOperacao confirmacao) : this()
    {
        _cartoes = cartoes;
        _contas = contas;
        _categorias = categorias;
        _servicoCompras = servico;
        _confirmacao = confirmacao;
        btnRegistrarCompra.Click += async (_, _) => await RegistrarCompraAsync();
    }

    public async Task AtualizarDadosAsync()
    {
        if (_operacaoCompra || IsDisposed || Disposing) return;
        OcupacaoCompra(true);
        txtResultadoCompra.Text = "Consultando cartões e categorias…";
        try { await RecarregarCoreAsync(); }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            InvalidarDados();
            txtResultadoCompra.Text = "Não foi possível carregar os dados. Use Atualizar dados para tentar novamente.";
        }
        finally { OcupacaoCompra(false); }
    }

    private async Task RecarregarCoreAsync(Guid? preferirCartao = null)
    {
        if (_cartoes is null || _contas is null || _categorias is null) return;
        Guid? anterior = preferirCartao ?? cmbCartaoCompra.SelectedValue as Guid?;
        Guid? categoria = cmbCategoriaCompra.SelectedValue as Guid?;
        var dados = await Task.Run(() => (_cartoes.ListarTodos(), _contas.ListarTodas(), _categorias.ListarTodas()));
        if (IsDisposed || Disposing) return;

        var contas = dados.Item2.ToDictionary(c => c.Id);
        var opcoes = dados.Item1.Select(c => new OpcaoCartao(c.Id,
            contas.TryGetValue(c.ContaId, out Conta? conta) ? $"{c.Nome} — {conta.Nome} ({conta.Banco})" : c.Nome,
            c.LimiteDisponivel)).ToList();
        PreencherSelecao(cmbCartaoCompra, opcoes, anterior);
        cmbCategoriaCompra.DisplayMember = nameof(Categoria.Nome);
        cmbCategoriaCompra.ValueMember = nameof(Categoria.Id);
        cmbCategoriaCompra.DataSource = dados.Item3;
        if (categoria.HasValue && dados.Item3.Any(c => c.Id == categoria))
            cmbCategoriaCompra.SelectedValue = categoria.Value;

        dtpDataCompra.MaxDate = DateTime.Today;
        AtualizarLimiteCompra();
        _consultaCartoesValida = true;
        txtResultadoCompra.Text = opcoes.Count == 0 ? "Cadastre um cartão em Cartões para registrar compras."
            : dados.Item3.Count == 0 ? "Cadastre uma categoria em Categorias para registrar compras."
            : "Informe o valor total e os demais campos. As parcelas entram nas faturas do cartão.";
    }

    private void OcupacaoCompra(bool ocupado)
    {
        _operacaoCompra = ocupado;
        if (IsDisposed || Disposing) return;
        UseWaitCursor = ocupado;
        tlpDadosCompra.Enabled = !ocupado;
        txtDescricaoCompra.Enabled = !ocupado;
        txtJustificativaCompra.Enabled = !ocupado;
        btnRegistrarCompra.Enabled = !ocupado && _consultaCartoesValida && _servicoCompras is not null &&
            cmbCartaoCompra.Items.Count > 0 && cmbCategoriaCompra.Items.Count > 0;
    }

    private void InvalidarDados()
    {
        _consultaCartoesValida = false;
        cmbCartaoCompra.DataSource = null;
        cmbCategoriaCompra.DataSource = null;
        AtualizarLimiteCompra();
    }

    private sealed record OpcaoCartao(Guid Id, string Nome, decimal Disponivel);

    private void ConfigurarCompras()
    {
        dtpDataCompra.Value = DateTime.Today;
        dtpDataCompra.MaxDate = DateTime.Today;
        cmbImportanciaCompra.DataSource = Enum.GetValues<ImportanciaTransacao>();
        cmbPlanejamentoCompra.DataSource = Enum.GetValues<PlanejamentoTransacao>();
        cmbUrgenciaCompra.DataSource = Enum.GetValues<UrgenciaTransacao>();
        foreach (ComboBox combo in new[] { cmbImportanciaCompra, cmbPlanejamentoCompra, cmbUrgenciaCompra })
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
    }

    private static void PreencherSelecao(ComboBox combo, List<OpcaoCartao> opcoes, Guid? anterior)
    {
        combo.DisplayMember = nameof(OpcaoCartao.Nome);
        combo.ValueMember = nameof(OpcaoCartao.Id);
        combo.DataSource = opcoes;
        if (anterior.HasValue && opcoes.Any(c => c.Id == anterior)) combo.SelectedValue = anterior.Value;
    }

    private void AtualizarLimiteCompra()
    {
        if (cmbCartaoCompra.SelectedItem is not OpcaoCartao cartao)
        {
            lblLimiteCompra.Text = "Selecione um cartão para consultar o limite disponível.";
            return;
        }
        lblLimiteCompra.Text = $"Disponível: {cartao.Disponivel.ToString("C2", TabelaVisual.Cultura)} • A compra compromete o valor total do limite.";
        lblLimiteCompra.ForeColor = numValorCompra.Value > cartao.Disponivel ? EstiloVisual.Vermelho : EstiloVisual.Verde;
    }

    private async Task RegistrarCompraAsync()
    {
        if (_operacaoCompra || !_consultaCartoesValida || _servicoCompras is null || IsDisposed || Disposing) return;
        CompraCartao? salva = null;
        try
        {
            if (cmbCartaoCompra.SelectedValue is not Guid cartaoId || cmbCategoriaCompra.SelectedValue is not Guid categoriaId)
                throw new ArgumentException("Selecione o cartão e a categoria.");
            if (cmbImportanciaCompra.SelectedItem is not ImportanciaTransacao importancia ||
                cmbPlanejamentoCompra.SelectedItem is not PlanejamentoTransacao planejamento ||
                cmbUrgenciaCompra.SelectedItem is not UrgenciaTransacao urgencia)
                throw new ArgumentException("Informe importância, planejamento e urgência.");
            decimal valor = numValorCompra.Value;
            int quantidade = (int)numParcelasCompra.Value;
            string descricao = txtDescricaoCompra.Text, justificativa = txtJustificativaCompra.Text;
            DateTime data = dtpDataCompra.Value.Date;
            OcupacaoCompra(true);
            CartaoCredito cartao = await Task.Run(() => _cartoes!.BuscarPorId(cartaoId)
                ?? throw new ConflitoFinanceiroException("O cartão não está mais disponível."));
            if (IsDisposed || Disposing) return;
            CompraCartao previa = new(cartao, valor, descricao, data, categoriaId, importancia, planejamento, urgencia, justificativa, quantidade);
            if (data > DateTime.Today || previa.Parcelas[0].Ciclo.Encerramento <= DateTime.Today)
                throw new ArgumentException("Informe uma compra do ciclo aberto, sem data futura.");
            if (valor > cartao.LimiteDisponivel) throw new ArgumentException("A compra ultrapassa o limite disponível.");
            if (!_confirmacao.Confirmar(this, "Registrar compra", ResumoOperacao.Compra(previa, cmbCartaoCompra.Text, cmbCategoriaCompra.Text)))
            { txtResultadoCompra.Text = "Compra cancelada. Preenchimento preservado."; return; }
            txtResultadoCompra.Text = "Registrando compra…";
            salva = await Task.Run(() => _servicoCompras.Registrar(cartaoId, valor, descricao, data,
                categoriaId, importancia, planejamento, urgencia, justificativa, quantidade));
            if (IsDisposed) return;
            txtDescricaoCompra.Clear();
            txtJustificativaCompra.Clear();
            numValorCompra.Value = 0m;
            numParcelasCompra.Value = 1m;
            dtpDataCompra.MaxDate = DateTime.Today;
            dtpDataCompra.Value = DateTime.Today;
            await RecarregarCoreAsync(cartaoId);
            if (IsDisposed) return;
            txtResultadoCompra.Text = $"Compra registrada: {salva.Descricao}\r\nTotal: {salva.Valor.ToString("C2", TabelaVisual.Cultura)} • {salva.QuantidadeParcelas} parcela(s).\r\nPrimeiro vencimento: {salva.Parcelas[0].Ciclo.Vencimento:dd/MM/yyyy}. Consulte os detalhes no Extrato e em Faturas.";
        }
        catch (ConflitoFinanceiroException erro) when (salva is null)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            try
            {
                await RecarregarCoreAsync();
                if (!IsDisposed && !Disposing)
                    txtResultadoCompra.Text = erro.Message + " Dados atualizados. Confira antes de confirmar novamente.";
            }
            catch (Exception falhaConsulta)
            {
                Debug.WriteLine(falhaConsulta);
                if (IsDisposed || Disposing) return;
                InvalidarDados();
                txtResultadoCompra.Text = "A compra não foi confirmada. Use Atualizar dados antes de tentar novamente.";
            }
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            if (salva is not null || !FalhaTela.EhValidacao(erro))
                InvalidarDados();
            txtResultadoCompra.Text = salva is not null
                ? "A compra foi salva, mas a consulta não pôde ser atualizada. Use Atualizar dados para conferir."
                : FalhaTela.EhValidacao(erro) ? erro.Message
                : "Não foi possível concluir o registro. Confira a conexão e consulte o histórico antes de tentar novamente.";
        }
        finally { OcupacaoCompra(false); }
    }

}
