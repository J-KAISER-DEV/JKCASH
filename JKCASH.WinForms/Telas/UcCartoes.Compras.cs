using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcCartoes
{
    private readonly ICategoriaRepositorio? _categorias;
    private readonly ICompraCartaoRepositorio? _compras;
    private readonly CompraCartaoServico? _servicoCompras;
    private bool _operacaoCompra;
    private bool _preenchendoCompras;

    public UcCartoes(ICartaoCreditoRepositorio cartoes, IContaRepositorio contas,
        ICategoriaRepositorio categorias, ICompraCartaoRepositorio compras, CompraCartaoServico servico)
        : this(cartoes, contas)
    {
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(compras);
        ArgumentNullException.ThrowIfNull(servico);
        _categorias = categorias;
        _compras = compras;
        _servicoCompras = servico;
        Load += async (_, _) => await CarregarDadosComprasAsync();
        btnAtualizarCartoes.Click += async (_, _) =>
        {
            CarregarContas();
            CarregarCartoes();
            await CarregarDadosComprasAsync();
        };
        btnRegistrarCompra.Click += async (_, _) => await RegistrarCompraAsync();
        cmbCartaoCompra.SelectedValueChanged += (_, _) => AtualizarLimiteCompra();
        numValorCompra.ValueChanged += (_, _) => AtualizarLimiteCompra();
        cmbCartaoHistorico.SelectedValueChanged += async (_, _) =>
        {
            if (!_preenchendoCompras && !_operacaoCompra) await CarregarHistoricoAsync();
        };
        dgvComprasCartao.SelectionChanged += (_, _) => MostrarDetalhesCompra();
        tabCartoes.SelectedIndexChanged += async (_, _) =>
        {
            if (!_operacaoCompra) await CarregarDadosComprasAsync();
        };
    }

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
        TabelaVisual.Configurar(dgvComprasCartao,
            ("Data", "Data", "dd/MM/yyyy", 90), ("Descricao", "Descrição", "", 200),
            ("Categoria", "Categoria", "", 120), ("Valor", "Total", "C2", 110),
            ("QuantidadeParcelas", "Parcelas", "", 80));
    }

    private async Task CarregarDadosComprasAsync(Guid? preferirCartao = null)
    {
        if (_operacaoCompra || _categorias is null || _cartoes is null || _contas is null) return;
        OcupacaoCompra(true);
        try { await RecarregarComprasCoreAsync(preferirCartao); }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            _preenchendoCompras = true;
            cmbCartaoCompra.DataSource = null;
            cmbCartaoHistorico.DataSource = null;
            cmbCategoriaCompra.DataSource = null;
            dgvComprasCartao.DataSource = null;
            txtDetalhesCompra.Clear();
            txtResultadoCompra.Text = "Não foi possível carregar os dados. Confira a conexão e clique em Atualizar dados.";
        }
        finally { _preenchendoCompras = false; OcupacaoCompra(false); }
    }

    private async Task RecarregarComprasCoreAsync(Guid? preferirCartao = null)
    {
        if (_cartoes is null || _contas is null || _categorias is null) return;
        Guid? anterior = preferirCartao ?? cmbCartaoCompra.SelectedValue as Guid?;
        Guid? historico = cmbCartaoHistorico.SelectedValue as Guid?;
        Guid? categoria = cmbCategoriaCompra.SelectedValue as Guid?;
        var dados = await Task.Run(() => (_cartoes.ListarTodos(), _contas.ListarTodas(), _categorias.ListarTodas()));
        if (IsDisposed) return;
        dtpDataCompra.MaxDate = DateTime.Today;
        var contas = dados.Item2.ToDictionary(c => c.Id);
        var opcoes = dados.Item1.Select(c => new OpcaoCartao(c.Id,
            contas.TryGetValue(c.ContaId, out Conta? conta) ? $"{c.Nome} — {conta.Nome} ({conta.Banco})" : c.Nome,
            c.LimiteDisponivel)).ToList();
        _preenchendoCompras = true;
        try
        {
            PreencherSelecao(cmbCartaoCompra, opcoes.ToList(), anterior);
            PreencherSelecao(cmbCartaoHistorico, opcoes.ToList(), historico ?? anterior);
            cmbCategoriaCompra.DisplayMember = nameof(Categoria.Nome);
            cmbCategoriaCompra.ValueMember = nameof(Categoria.Id);
            cmbCategoriaCompra.DataSource = dados.Item3;
            if (categoria.HasValue && dados.Item3.Any(c => c.Id == categoria)) cmbCategoriaCompra.SelectedValue = categoria.Value;
            AtualizarLimiteCompra();
            if (opcoes.Count == 0) txtResultadoCompra.Text = "Cadastre um cartão na primeira aba para registrar compras.";
            else if (dados.Item3.Count == 0) txtResultadoCompra.Text = "Cadastre uma categoria em Categorias para registrar compras.";
            else txtResultadoCompra.Text = "Informe o valor total e todos os campos. A compra gera suas parcelas e faturas; o saldo da conta será movimentado no pagamento.";
        }
        finally { _preenchendoCompras = false; }
        await CarregarHistoricoCoreAsync();
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
        if (_operacaoCompra || _servicoCompras is null) return;
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
            await RecarregarComprasCoreAsync(cartaoId);
            if (IsDisposed) return;
            CarregarCartoes();
            txtResultadoCompra.Text = $"Compra registrada: {salva.Descricao}\r\nTotal: {salva.Valor.ToString("C2", TabelaVisual.Cultura)} • {salva.QuantidadeParcelas} parcela(s).\r\nPrimeiro vencimento: {salva.Parcelas[0].Ciclo.Vencimento:dd/MM/yyyy}. Consulte os detalhes em Compras do cartão e em Faturas.";
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            txtResultadoCompra.Text = salva is not null
                ? "A compra foi salva, mas a consulta não pôde ser atualizada. Use Atualizar dados para conferir."
                : erro is ArgumentException or InvalidOperationException ? erro.Message
                : "Não foi possível concluir o registro. Confira a conexão e consulte o histórico antes de tentar novamente.";
        }
        finally { OcupacaoCompra(false); }
    }

    private async Task CarregarHistoricoAsync()
    {
        OcupacaoCompra(true);
        try { await CarregarHistoricoCoreAsync(); }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (!IsDisposed)
            {
                dgvComprasCartao.DataSource = null;
                txtDetalhesCompra.Text = "Não foi possível consultar as compras. Clique em Atualizar dados.";
            }
        }
        finally { OcupacaoCompra(false); }
    }

    private async Task CarregarHistoricoCoreAsync()
    {
        if (_compras is null || _categorias is null) return;
        dgvComprasCartao.DataSource = null;
        txtDetalhesCompra.Clear();
        if (cmbCartaoHistorico.SelectedValue is not Guid id) return;
        var dados = await Task.Run(() => (_compras.ListarPorCartao(id), _categorias.ListarTodas()));
        if (IsDisposed) return;
        var categorias = dados.Item2.ToDictionary(c => c.Id, c => c.Nome);
        dgvComprasCartao.DataSource = dados.Item1.OrderByDescending(c => c.Data).ThenBy(c => c.Id)
            .Select(c => new LinhaCompra(c, categorias.GetValueOrDefault(c.CategoriaId, "Categoria não encontrada"))).ToList();
        if (dgvComprasCartao.Rows.Count > 0) dgvComprasCartao.CurrentCell = dgvComprasCartao.Rows[0].Cells[0];
        MostrarDetalhesCompra();
        if (dados.Item1.Count == 0) txtDetalhesCompra.Text = "Nenhuma compra registrada neste cartão.";
    }

    private void MostrarDetalhesCompra()
    {
        if (dgvComprasCartao.CurrentRow?.DataBoundItem is not LinhaCompra linha) return;
        CompraCartao compra = linha.Compra;
        var linhas = new List<string>
        {
            $"{compra.Data:dd/MM/yyyy} • {compra.Descricao}",
            $"Total: {compra.Valor.ToString("C2", TabelaVisual.Cultura)} • Categoria: {linha.Categoria}",
            $"Importância: {compra.Importancia} • Planejamento: {(compra.Planejamento == PlanejamentoTransacao.NaoPlanejada ? "Não planejada" : "Planejada")} • Urgência: {(compra.Urgencia == UrgenciaTransacao.SemUrgencia ? "Sem urgência" : compra.Urgencia.ToString())}",
            $"Justificativa: {compra.Justificativa}", "", "Parcelas:"
        };
        linhas.AddRange(compra.Parcelas.OrderBy(p => p.Numero).Select(p =>
            $"{p.Numero}/{p.Quantidade} • {p.Valor.ToString("C2", TabelaVisual.Cultura)} • Fechamento: {p.Ciclo.Fechamento:dd/MM/yyyy} • Vencimento: {p.Ciclo.Vencimento:dd/MM/yyyy}"));
        txtDetalhesCompra.Lines = linhas.ToArray();
        txtDetalhesCompra.Select(0, 0);
    }

    private void OcupacaoCompra(bool ocupado)
    {
        _operacaoCompra = ocupado;
        if (IsDisposed) return;
        tabCartoes.Enabled = !ocupado;
        btnAtualizarCartoes.Enabled = !ocupado;
        btnRegistrarCompra.Enabled = !ocupado && _servicoCompras is not null &&
            cmbCartaoCompra.Items.Count > 0 && cmbCategoriaCompra.Items.Count > 0;
    }

    private sealed record OpcaoCartao(Guid Id, string Nome, decimal Disponivel);
    private sealed record LinhaCompra(CompraCartao Compra, string Categoria)
    {
        public DateTime Data => Compra.Data;
        public string Descricao => Compra.Descricao;
        public decimal Valor => Compra.Valor;
        public int QuantidadeParcelas => Compra.QuantidadeParcelas;
    }
}
