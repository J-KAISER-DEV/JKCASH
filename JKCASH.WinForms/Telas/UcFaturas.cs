using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Excecoes;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcFaturas : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private readonly ICartaoCreditoRepositorio? _cartoes;
    private readonly IFaturaCartaoRepositorio? _faturas;
    private readonly ICompraCartaoRepositorio? _compras;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private readonly PagamentoFaturaServico? _pagamentos;
    private DadosFaturas? _dados;
    private bool _ocupado, _preenchendo;

    public UcFaturas()
    {
        InitializeComponent();
        TabelaVisual.Configurar(dgvFaturas,
            ("Fechamento", "Fechamento", "dd/MM/yyyy", 100), ("Vencimento", "Vencimento", "dd/MM/yyyy", 100),
            ("Total", "Total", "C2", 110), ("Pago", "Pago", "C2", 110), ("Pendente", "Pendente", "C2", 110),
            ("Situacao", "Situação", "", 110), ("Pagamento", "Pagamento", "", 110), ("Atrasada", "Atrasada", "", 80));
        TabelaVisual.Configurar(dgvParcelasFatura,
            ("Data", "Compra em", "dd/MM/yyyy", 100), ("Descricao", "Compra", "", 210),
            ("Categoria", "Categoria", "", 130), ("Parcela", "Parcela", "", 80), ("Valor", "Valor", "C2", 110));
        TabelaVisual.Configurar(dgvPagamentosFatura,
            ("Data", "Data", "dd/MM/yyyy", 100), ("Conta", "Conta utilizada", "", 250), ("Valor", "Valor pago", "C2", 120));
        dgvFaturas.SelectionChanged += (_, _) => { if (!_preenchendo) MostrarFatura(); };
        btnValorPendente.Click += (_, _) =>
        {
            if (FaturaSelecionada is FaturaCartao fatura)
                numValorPagamento.Value = Math.Min(numValorPagamento.Maximum, fatura.SaldoPendente);
        };
    }

    public UcFaturas(ICartaoCreditoRepositorio cartoes, IFaturaCartaoRepositorio faturas,
        ICompraCartaoRepositorio compras, IContaRepositorio contas, ICategoriaRepositorio categorias,
        PagamentoFaturaServico pagamentos, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(cartoes);
        ArgumentNullException.ThrowIfNull(faturas);
        ArgumentNullException.ThrowIfNull(compras);
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(pagamentos);
        _cartoes = cartoes;
        _faturas = faturas;
        _compras = compras;
        _contas = contas;
        _categorias = categorias;
        _pagamentos = pagamentos;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        Load += async (_, _) => await AtualizarFaturasAsync();
        btnAtualizarFaturas.Click += async (_, _) => await AtualizarFaturasAsync();
        cmbCartaoFaturas.SelectedValueChanged += async (_, _) =>
        {
            if (!_preenchendo && !_ocupado) await AtualizarFaturasAsync();
        };
        btnPagarFatura.Click += async (_, _) => await PagarFaturaAsync();
    }

    private FaturaCartao? FaturaSelecionada => (dgvFaturas.CurrentRow?.DataBoundItem as LinhaFatura)?.Fatura;

    private async Task AtualizarFaturasAsync()
    {
        if (_ocupado || _cartoes is null || IsDisposed || Disposing) return;
        Ocupar(true);
        lblStatusFaturas.Text = "Consultando faturas…";
        try
        {
            await RecarregarCoreAsync();
            if (!IsDisposed) lblStatusFaturas.Text = _dados?.Cartoes.Count == 0
                ? "Cadastre um cartão em Cartões para começar."
                : _dados?.Faturas.Count == 0 ? "Este cartão ainda não tem faturas. Registre uma compra em Cartões."
                : _dados is not null && (!_dados.Contas.Any(c => c.Ativa) || _dados.Categorias.Count == 0)
                ? "Cadastre uma conta ativa e uma categoria para registrar pagamentos. As faturas estão disponíveis para consulta."
                : "Selecione uma fatura para consultar parcelas e pagamentos. Use Atualizar dados para consultar novamente.";
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            InvalidarConsulta();
            lblStatusFaturas.Text = "Não foi possível carregar os dados. Confira a conexão e clique em Atualizar dados.";
        }
        finally { Ocupar(false); }
    }

    private async Task RecarregarCoreAsync(Guid? preferirFatura = null)
    {
        if (_cartoes is null || _contas is null || _categorias is null || _compras is null || _faturas is null) return;
        Guid? cartaoId = cmbCartaoFaturas.SelectedValue as Guid?;
        Guid? contaId = cmbContaPagamento.SelectedValue as Guid?;
        Guid? categoriaId = cmbCategoriaPagamento.SelectedValue as Guid?;
        Guid? faturaId = preferirFatura ?? FaturaSelecionada?.Id;
        DadosFaturas dados = await Task.Run(() =>
        {
            var cartoes = _cartoes.ListarTodos();
            Guid? escolhido = cartoes.Any(c => c.Id == cartaoId) ? cartaoId : cartoes.FirstOrDefault()?.Id;
            return new DadosFaturas(cartoes, _contas.ListarTodas(), _categorias.ListarTodas(),
                escolhido.HasValue ? _compras.ListarPorCartao(escolhido.Value) : new(),
                escolhido.HasValue ? _faturas.ListarPorCartao(escolhido.Value) : new(), escolhido);
        });
        if (IsDisposed) return;
        _dados = dados;
        _preenchendo = true;
        try
        {
            var contas = dados.Contas.ToDictionary(c => c.Id);
            cmbCartaoFaturas.DisplayMember = "Nome";
            cmbCartaoFaturas.ValueMember = "Id";
            cmbCartaoFaturas.DataSource = dados.Cartoes.Select(c => new
            {
                c.Id, Nome = contas.TryGetValue(c.ContaId, out Conta? conta)
                    ? $"{c.Nome} — {conta.Nome} ({conta.Banco})" : c.Nome
            }).ToList();
            if (dados.CartaoId.HasValue) cmbCartaoFaturas.SelectedValue = dados.CartaoId.Value;
            cmbContaPagamento.DisplayMember = "Nome";
            cmbContaPagamento.ValueMember = "Id";
            cmbContaPagamento.DataSource = dados.Contas.Where(c => c.Ativa).Select(c => new
            {
                c.Id, Nome = $"{c.Nome} ({c.Banco}) • saldo {c.Saldo.SaldoAtual.ToString("C2", TabelaVisual.Cultura)}"
            }).ToList();
            if (contaId.HasValue && dados.Contas.Any(c => c.Id == contaId && c.Ativa)) cmbContaPagamento.SelectedValue = contaId.Value;
            cmbCategoriaPagamento.DisplayMember = nameof(Categoria.Nome);
            cmbCategoriaPagamento.ValueMember = nameof(Categoria.Id);
            cmbCategoriaPagamento.DataSource = dados.Categorias;
            if (categoriaId.HasValue && dados.Categorias.Any(c => c.Id == categoriaId)) cmbCategoriaPagamento.SelectedValue = categoriaId.Value;
            var linhas = dados.Faturas.OrderBy(f => f.Ciclo.Vencimento).Select(f => new LinhaFatura(f)).ToList();
            dgvFaturas.DataSource = linhas;
            int indice = linhas.FindIndex(l => l.Fatura.Id == faturaId);
            if (indice < 0) indice = linhas.FindIndex(l => l.Fatura.SaldoPendente > 0);
            if (indice >= 0) dgvFaturas.CurrentCell = dgvFaturas.Rows[indice].Cells[0];
            CartaoCredito? cartao = dados.Cartoes.FirstOrDefault(c => c.Id == dados.CartaoId);
            lblLimiteFatura.Text = cartao is null ? "Nenhum cartão cadastrado." :
                $"Limite: {cartao.Limite.ToString("C2", TabelaVisual.Cultura)} • Utilizado: {cartao.ValorUtilizado.ToString("C2", TabelaVisual.Cultura)} • Disponível: {cartao.LimiteDisponivel.ToString("C2", TabelaVisual.Cultura)}";
        }
        finally { _preenchendo = false; }
        MostrarFatura();
    }

    private void MostrarFatura()
    {
        dgvParcelasFatura.DataSource = null;
        dgvPagamentosFatura.DataSource = null;
        numValorPagamento.Value = 0m;
        if (FaturaSelecionada is not FaturaCartao fatura || _dados is null)
        {
            lblResumoFatura.Text = "Selecione uma fatura para consultar os detalhes.";
            AtualizarPagamento();
            return;
        }
        var compras = _dados.Compras.ToDictionary(c => c.Id);
        var categorias = _dados.Categorias.ToDictionary(c => c.Id, c => c.Nome);
        var contas = _dados.Contas.ToDictionary(c => c.Id);
        dgvParcelasFatura.DataSource = fatura.Parcelas.OrderBy(p => p.Numero).Select(p =>
        {
            compras.TryGetValue(p.CompraId, out CompraCartao? compra);
            return new
            {
                Data = compra?.Data, Descricao = compra?.Descricao ?? "Compra não encontrada",
                Categoria = compra is null ? "" : categorias.GetValueOrDefault(compra.CategoriaId, "Categoria não encontrada"),
                Parcela = $"{p.Numero}/{p.Quantidade}", p.Valor
            };
        }).ToList();
        dgvPagamentosFatura.DataSource = fatura.Pagamentos.OrderByDescending(p => p.Data).Select(p => new
        {
            p.Data, Conta = contas.TryGetValue(p.ContaId, out Conta? conta)
                ? $"{conta.Nome} ({conta.Banco})" : "Conta não encontrada", p.Valor
        }).ToList();
        lblResumoFatura.Text = $"Vencimento: {fatura.Ciclo.Vencimento:dd/MM/yyyy} • Total: {fatura.Total.ToString("C2", TabelaVisual.Cultura)} • Pago: {fatura.TotalPago.ToString("C2", TabelaVisual.Cultura)} • Pendente: {fatura.SaldoPendente.ToString("C2", TabelaVisual.Cultura)}" +
            (fatura.EstaAtrasadaEm(DateTime.Today) ? "\r\nFatura atrasada — valores sem cálculo de juros e multas." : "");
        AtualizarPagamento();
    }

    private async Task PagarFaturaAsync()
    {
        if (_ocupado || _dados is null || _pagamentos is null || IsDisposed || Disposing) return;
        PagamentoFatura? salvo = null;
        Guid? faturaTentativa = null;
        try
        {
            FaturaCartao fatura = FaturaSelecionada ?? throw new ArgumentException("Selecione uma fatura.");
            faturaTentativa = fatura.Id;
            if (cmbContaPagamento.SelectedValue is not Guid contaId || cmbCategoriaPagamento.SelectedValue is not Guid categoriaId)
                throw new ArgumentException("Selecione a conta e a categoria do pagamento.");
            decimal valor = numValorPagamento.Value;
            Ocupar(true);
            Conta conta = _dados.Contas.Single(c => c.Id == contaId);
            CartaoCredito cartao = _dados.Cartoes.Single(c => c.Id == fatura.CartaoId);
            _ = new PagamentoFatura(fatura.Id, contaId, Guid.NewGuid(), valor, DateTime.Today);
            if (valor > fatura.SaldoPendente || valor > cartao.ValorUtilizado)
                throw new ArgumentException("O pagamento ultrapassa o valor pendente ou utilizado.");
            conta.ValidarSaida(valor);
            if (!_confirmacao.Confirmar(this, "Registrar pagamento", ResumoOperacao.Pagamento(fatura, conta, cartao, valor) + "\nCategoria: " + cmbCategoriaPagamento.Text))
            { lblStatusFaturas.Text = "Pagamento cancelado. Preenchimento preservado."; return; }
            lblStatusFaturas.Text = "Registrando pagamento…";
            salvo = await Task.Run(() => _pagamentos.Registrar(fatura.Id, contaId, valor, categoriaId));
            if (IsDisposed) return;
            numValorPagamento.Value = 0m;
            await RecarregarCoreAsync(fatura.Id);
            if (!IsDisposed) lblStatusFaturas.Text = $"Pagamento de {salvo.Valor.ToString("C2", TabelaVisual.Cultura)} confirmado em {salvo.Data:dd/MM/yyyy}. Histórico, saldo da conta e limite atualizados.";
        }
        catch (ConflitoFinanceiroException erro) when (salvo is null)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            try
            {
                await RecarregarCoreAsync(faturaTentativa);
                if (!IsDisposed) lblStatusFaturas.Text = erro.Message +
                    " Dados atualizados. Confira a fatura e informe o valor novamente.";
            }
            catch (Exception falhaConsulta)
            {
                Debug.WriteLine(falhaConsulta);
                if (IsDisposed) return;
                InvalidarConsulta();
                lblStatusFaturas.Text = "O pagamento não foi confirmado. Use Atualizar dados antes de tentar novamente.";
            }
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            if (salvo is not null || !FalhaTela.EhValidacao(erro))
                InvalidarConsulta();
            lblStatusFaturas.Text = salvo is not null
                ? "O pagamento foi salvo, mas a consulta não pôde ser atualizada. Clique em Atualizar dados para conferir."
                : FalhaTela.EhValidacao(erro) ? erro.Message
                : "Não foi possível concluir o pagamento. Confira a conexão e consulte o histórico antes de tentar novamente.";
        }
        finally { Ocupar(false); }
    }

    private void Ocupar(bool ocupado)
    {
        _ocupado = ocupado;
        if (IsDisposed) return;
        UseWaitCursor = ocupado;
        pnlFiltrosFaturas.Enabled = !ocupado;
        dgvFaturas.Enabled = !ocupado;
        pnlPagamentoFatura.Enabled = !ocupado;
        AtualizarPagamento();
    }

    private void InvalidarConsulta()
    {
        _dados = null;
        dgvFaturas.DataSource = null;
        dgvParcelasFatura.DataSource = null;
        dgvPagamentosFatura.DataSource = null;
        cmbContaPagamento.DataSource = null;
        cmbCategoriaPagamento.DataSource = null;
        lblResumoFatura.Text = "Consulta indisponível. Atualize os dados para conferir.";
        lblLimiteFatura.Text = "Limites indisponíveis.";
    }

    private void AtualizarPagamento()
    {
        bool pendente = !_ocupado && _dados is not null && FaturaSelecionada?.SaldoPendente > 0;
        btnValorPendente.Enabled = pendente;
        btnPagarFatura.Enabled = pendente && _pagamentos is not null &&
            cmbContaPagamento.Items.Count > 0 && cmbCategoriaPagamento.Items.Count > 0;
    }

    private sealed record DadosFaturas(List<CartaoCredito> Cartoes, List<Conta> Contas,
        List<Categoria> Categorias, List<CompraCartao> Compras, List<FaturaCartao> Faturas, Guid? CartaoId);

    private sealed record LinhaFatura(FaturaCartao Fatura)
    {
        public DateTime Fechamento => Fatura.Ciclo.Fechamento;
        public DateTime Vencimento => Fatura.Ciclo.Vencimento;
        public decimal Total => Fatura.Total;
        public decimal Pago => Fatura.TotalPago;
        public decimal Pendente => Fatura.SaldoPendente;
        public string Situacao => Fatura.SituacaoEm(DateTime.Today).ToString();
        public string Pagamento => Fatura.SituacaoPagamento == SituacaoPagamentoFatura.SemPagamento
            ? "Sem pagamento" : Fatura.SituacaoPagamento.ToString();
        public string Atrasada => Fatura.EstaAtrasadaEm(DateTime.Today) ? "Sim" : "Não";
    }
}
