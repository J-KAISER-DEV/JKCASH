using JKCASH.WinForms.Visual;

using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.WinForms.Telas;

public sealed class UcDashboard : UserControl
{
    public event EventHandler? AbrirTransacoes;
    public event EventHandler? AbrirFaturas;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private readonly ITransacaoRepositorio? _transacoes;
    private readonly ICartaoCreditoRepositorio? _cartoes;
    private readonly IFaturaCartaoRepositorio? _faturas;
    private readonly ComboBox _cmbContas = new() { Name = "cmbContaDashboard", DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
    private readonly DateTimePicker _mes = new() { Name = "dtpMesDashboard", Format = DateTimePickerFormat.Custom, CustomFormat = "MMMM 'de' yyyy", ShowUpDown = true, Width = 170 };
    private readonly Button _atualizar = EstiloVisual.Botao("Atualizar");
    private readonly Label[] _valores = new Label[4];
    private Label _status = null!, _creditoNome = null!, _creditoDisponivel = null!, _creditoTotal = null!;
    private readonly Panel _categoriasGrafico = new() { Dock = DockStyle.Fill };
    private DataGridView _movimentacoes = null!;
    private Panel _faixa = null!, _barra = null!;
    private decimal _proporcaoDisponivel;
    private bool _ocupado, _preenchendo;

    public UcDashboard()
    {
        Dock = DockStyle.Fill;
        BackColor = EstiloVisual.Fundo;
        AutoScroll = true;
        AutoScrollMinSize = new Size(0, 870);
        Font = new Font("Segoe UI", 10);

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            Height = 870,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(24, 14, 24, 16),
            BackColor = EstiloVisual.Fundo
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 260));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.Controls.Add(CriarCabecalho(), 0, 0);
        layout.Controls.Add(CriarIndicadores(), 0, 1);
        layout.Controls.Add(CriarGraficos(), 0, 2);
        layout.Controls.Add(CriarMovimentacoes(), 0, 3);
        Label aviso = EstiloVisual.Rotulo("Carregando dados…", 9);
        aviso.Name = "lblStatusDashboard";
        _status = aviso;
        aviso.ForeColor = EstiloVisual.Secundario;
        layout.Controls.Add(aviso, 0, 4);
        Controls.Add(layout);
        SizeChanged += (_, _) => layout.Height = Math.Max(870, ClientSize.Height - 2);
        _mes.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    public UcDashboard(IContaRepositorio contas, ICategoriaRepositorio categorias, ITransacaoRepositorio transacoes,
        ICartaoCreditoRepositorio cartoes, IFaturaCartaoRepositorio faturas) : this()
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(transacoes);
        ArgumentNullException.ThrowIfNull(cartoes);
        ArgumentNullException.ThrowIfNull(faturas);
        _contas = contas; _categorias = categorias; _transacoes = transacoes; _cartoes = cartoes; _faturas = faturas;
        Load += async (_, _) => await AtualizarDashboardAsync();
        _atualizar.Click += async (_, _) => await AtualizarDashboardAsync();
        _cmbContas.SelectedValueChanged += async (_, _) => { if (!_preenchendo) await AtualizarDashboardAsync(); };
        _mes.ValueChanged += async (_, _) => { if (!_preenchendo) await AtualizarDashboardAsync(); };
    }

    private Control CriarCabecalho()
    {
        TableLayoutPanel cabecalho = new() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        cabecalho.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        cabecalho.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 510));
        TableLayoutPanel titulos = new() { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        titulos.RowStyles.Add(new RowStyle(SizeType.Absolute, 51));
        titulos.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        titulos.Controls.Add(EstiloVisual.Rotulo("Dashboard", 28, true), 0, 0);
        Label subtitulo = EstiloVisual.Rotulo("Uma visão geral das suas finanças", 10);
        subtitulo.ForeColor = EstiloVisual.Secundario;
        subtitulo.TextAlign = ContentAlignment.TopLeft;
        titulos.Controls.Add(subtitulo, 0, 1);
        cabecalho.Controls.Add(titulos, 0, 0);
        FlowLayoutPanel acoes = new() { Dock = DockStyle.Fill, Padding = new Padding(0, 3, 0, 0), WrapContents = true };
        _cmbContas.Margin = _mes.Margin = new Padding(0, 6, 10, 0);
        _cmbContas.AccessibleName = "Filtro de contas";
        _mes.AccessibleName = "Mês do dashboard";
        _atualizar.Name = "btnAtualizarDashboard";
        _atualizar.Width = 125;
        _atualizar.Height = 32;
        acoes.Controls.Add(_cmbContas);
        acoes.Controls.Add(_mes);
        acoes.Controls.Add(_atualizar);
        Button nova = EstiloVisual.Botao("+ Nova transação", true);
        nova.Width = 170;
        nova.Click += (_, _) => AbrirTransacoes?.Invoke(this, EventArgs.Empty);
        acoes.Controls.Add(nova);
        cabecalho.Controls.Add(acoes, 1, 0);
        return cabecalho;
    }

    private Control CriarIndicadores()
    {
        TableLayoutPanel linha = new() { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        var dados = new[]
        {
            ("Saldo atual", "—", "Contas selecionadas", EstiloVisual.Texto),
            ("Receitas do mês", "—", "Sem transferências", EstiloVisual.Verde),
            ("Despesas do mês", "—", "Sem transferências", EstiloVisual.Vermelho),
            ("Faturas pendentes", "—", "Vencimento no mês", EstiloVisual.Texto)
        };
        for (int i = 0; i < dados.Length; i++)
        {
            linha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            PainelCartao cartao = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, i == 3 ? 0 : 14, 14) };
            TableLayoutPanel textos = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            textos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            textos.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
            textos.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            textos.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            textos.Controls.Add(EstiloVisual.Rotulo(dados[i].Item1, 11, true), 0, 0);
            Label valor = new RotuloMonetario { Text = dados[i].Item2, ForeColor = dados[i].Item4 };
            valor.Name = new[] { "lblSaldoDashboard", "lblReceitasDashboard", "lblDespesasDashboard", "lblFaturasDashboard" }[i];
            _valores[i] = valor;
            textos.Controls.Add(valor, 0, 1);
            Label legenda = EstiloVisual.Rotulo(dados[i].Item3, 9);
            legenda.ForeColor = i is 1 or 2 ? dados[i].Item4 : EstiloVisual.Secundario;
            textos.Controls.Add(legenda, 0, 2);
            cartao.Controls.Add(textos);
            linha.Controls.Add(cartao, i, 0);
        }
        return linha;
    }

    private Control CriarGraficos()
    {
        TableLayoutPanel linha = new() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        linha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        linha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        PainelCartao gastos = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 16, 14) };
        gastos.Controls.Add(_categoriasGrafico);
        linha.Controls.Add(gastos, 0, 0);
        PainelCartao credito = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 14) };
        TableLayoutPanel info = new() { Dock = DockStyle.Fill, RowCount = 7, ColumnCount = 1 };
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        float[] alturas = [34, 25, 20, 44, 22, 16, 38];
        foreach (float altura in alturas) info.RowStyles.Add(new RowStyle(SizeType.Absolute, altura));
        info.Controls.Add(EstiloVisual.Rotulo("Cartões de crédito", 15, true), 0, 0);
        _creditoNome = EstiloVisual.Rotulo("Nenhum cartão vinculado", 10);
        info.Controls.Add(_creditoNome, 0, 1);
        info.Controls.Add(EstiloVisual.Rotulo("Limite disponível", 9), 0, 2);
        Label disponivel = new RotuloMonetario { Text = "—" };
        _creditoDisponivel = disponivel;
        disponivel.ForeColor = EstiloVisual.Verde;
        info.Controls.Add(disponivel, 0, 3);
        _creditoTotal = EstiloVisual.Rotulo("Limite total: —", 9);
        info.Controls.Add(_creditoTotal, 0, 4);
        Panel faixa = new() { Dock = DockStyle.Fill, BackColor = EstiloVisual.Borda, Margin = new Padding(0, 6, 0, 6) };
        Panel preenchimento = new() { Dock = DockStyle.Left, BackColor = EstiloVisual.Verde };
        _faixa = faixa;
        _barra = preenchimento;
        faixa.Resize += (_, _) => preenchimento.Width = (int)(faixa.Width * _proporcaoDisponivel);
        faixa.Controls.Add(preenchimento);
        info.Controls.Add(faixa, 0, 5);
        Button faturas = EstiloVisual.Botao("Ver faturas");
        faturas.Dock = DockStyle.Fill;
        faturas.Click += (_, _) => AbrirFaturas?.Invoke(this, EventArgs.Empty);
        info.Controls.Add(faturas, 0, 6);
        credito.Controls.Add(info);
        linha.Controls.Add(credito, 1, 0);
        return linha;
    }

    private async Task AtualizarDashboardAsync()
    {
        if (_ocupado || _contas is null || _categorias is null || _transacoes is null || _cartoes is null || _faturas is null) return;
        _ocupado = true;
        _atualizar.Enabled = _cmbContas.Enabled = _mes.Enabled = false;
        _status.Text = "Consultando dados…";
        Guid? contaId = (_cmbContas.SelectedItem as OpcaoConta)?.Id;
        DateTime inicio = new(_mes.Value.Year, _mes.Value.Month, 1);
        DateTime fim = inicio.AddMonths(1).AddDays(-1);
        try
        {
            var dados = await Task.Run(() =>
            {
                var contas = _contas.ListarTodas();
                Guid? filtro = contas.Any(c => c.Id == contaId) ? contaId : null;
                var selecionadas = contas.Where(c => !filtro.HasValue || c.Id == filtro).ToList();
                var ids = selecionadas.Select(c => c.Id).ToList();
                var transacoes = ids.Count == 0 ? new List<Transacao>() : _transacoes.ListarPorPeriodo(ids, inicio, fim);
                var cartoes = _cartoes.ListarTodos().Where(c => ids.Contains(c.ContaId)).ToList();
                var faturas = cartoes.SelectMany(c => _faturas.ListarPorCartao(c.Id)).ToList();
                return new DadosDashboard(contas, selecionadas, transacoes, _categorias.ListarTodas(), cartoes, faturas, filtro);
            });
            if (IsDisposed) return;
            _preenchendo = true;
            try
            {
                var opcoes = new List<OpcaoConta> { new(null, "Todas as contas") };
                opcoes.AddRange(dados.Contas.Select(c => new OpcaoConta(c.Id, $"{c.Nome} ({c.Banco}){(c.Ativa ? "" : " — inativa")}")));
                _cmbContas.DisplayMember = nameof(OpcaoConta.Nome);
                _cmbContas.DataSource = opcoes;
                _cmbContas.SelectedItem = opcoes.First(c => c.Id == dados.Filtro);
            }
            finally { _preenchendo = false; }
            var comuns = dados.Transacoes.Where(t => t.TransferenciaId is null).ToList();
            decimal receitas = comuns.Where(t => t.Tipo == TipoTransacao.Entrada).Sum(t => t.Valor);
            decimal despesas = comuns.Where(t => t.Tipo == TipoTransacao.Saida).Sum(t => t.Valor);
            decimal pendente = dados.Faturas.Where(f => f.Ciclo.Vencimento >= inicio && f.Ciclo.Vencimento <= fim).Sum(f => f.SaldoPendente);
            decimal[] valores = [dados.Selecionadas.Sum(c => c.Saldo.SaldoAtual), receitas, despesas, pendente];
            for (int i = 0; i < valores.Length; i++) _valores[i].Text = valores[i].ToString("C2", TabelaVisual.Cultura);
            _valores[0].ForeColor = valores[0] < 0 ? EstiloVisual.Vermelho : EstiloVisual.Texto;
            var categorias = dados.Categorias.ToDictionary(c => c.Id, c => c.Nome);
            var contasNome = dados.Contas.ToDictionary(c => c.Id, c => c.Nome);
            _movimentacoes.DataSource = dados.Transacoes.OrderByDescending(t => t.Data).ThenByDescending(t => t.Id).Take(8)
                .Select(t => new LinhaMovimentacao(t, contasNome.GetValueOrDefault(t.ContaId, "Conta não encontrada"), categorias.GetValueOrDefault(t.CategoriaId, "Categoria não encontrada"))).ToList();
            RenderizarCategorias(comuns.Where(t => t.Tipo == TipoTransacao.Saida)
                .GroupBy(t => t.CategoriaId).Select(g => (categorias.GetValueOrDefault(g.Key, "Categoria não encontrada"), g.Sum(t => t.Valor))).OrderByDescending(g => g.Item2).ToList());
            decimal limite = dados.Cartoes.Sum(c => c.Limite), disponivel = dados.Cartoes.Sum(c => c.LimiteDisponivel);
            _creditoNome.Text = dados.Cartoes.Count == 0 ? "Nenhum cartão vinculado"
                : dados.Cartoes.Count == 1 ? "1 cartão vinculado" : $"{dados.Cartoes.Count} cartões vinculados";
            _creditoDisponivel.Text = disponivel.ToString("C2", TabelaVisual.Cultura);
            _creditoTotal.Text = $"Limite total: {limite.ToString("C2", TabelaVisual.Cultura)}";
            _proporcaoDisponivel = limite == 0 ? 0 : Math.Clamp(disponivel / limite, 0m, 1m);
            _barra.Width = (int)(_faixa.ClientSize.Width * _proporcaoDisponivel);
            _status.Text = dados.Contas.Count == 0 ? "Nenhuma conta cadastrada. Comece pelo menu Contas." :
                $"Dados atualizados • {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}. Saldo e limites são atuais; receitas e despesas excluem transferências internas." +
                (dados.Transacoes.Count == 0 ? " Nenhuma movimentação no período." : "");
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            foreach (Label valor in _valores) valor.Text = "—";
            _movimentacoes.DataSource = null;
            RenderizarCategorias(new());
            _creditoNome.Text = "Consulta indisponível";
            _creditoDisponivel.Text = "—";
            _creditoTotal.Text = "Limite total: —";
            _proporcaoDisponivel = 0;
            _barra.Width = 0;
            _status.Text = "Não foi possível carregar os dados. Confira a conexão e clique em Atualizar.";
        }
        finally
        {
            _ocupado = false;
            if (!IsDisposed) _atualizar.Enabled = _cmbContas.Enabled = _mes.Enabled = true;
        }
    }

    private void RenderizarCategorias(List<(string Nome, decimal Valor)> categorias)
    {
        while (_categoriasGrafico.Controls.Count > 0) _categoriasGrafico.Controls[0].Dispose();
        TableLayoutPanel grafico = new() { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 6 };
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
        grafico.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        for (int linha = 0; linha < 5; linha++) grafico.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        Label titulo = EstiloVisual.Rotulo("Saídas por categoria", 15, true);
        grafico.Controls.Add(titulo, 0, 0);
        grafico.SetColumnSpan(titulo, 3);
        if (categorias.Count == 0)
        {
            Label vazio = EstiloVisual.Rotulo("Nenhuma saída no período.");
            grafico.Controls.Add(vazio, 0, 1);
            grafico.SetColumnSpan(vazio, 3);
        }
        var exibidas = categorias.Take(4).ToList();
        if (categorias.Count > 4) exibidas.Add(("Outras categorias", categorias.Skip(4).Sum(c => c.Valor)));
        decimal maior = exibidas.Count == 0 ? 1m : exibidas.Max(c => c.Valor);
        for (int i = 0; i < exibidas.Count; i++)
        {
            grafico.Controls.Add(EstiloVisual.Rotulo(exibidas[i].Nome, 9), 0, i + 1);
            Panel trilho = new() { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(3, 9, 10, 9) };
            Panel barra = new() { Dock = DockStyle.Left, BackColor = Color.FromArgb(31, 85 + i * 20, 121 + i * 15) };
            decimal proporcao = maior == 0 ? 0 : exibidas[i].Valor / maior;
            trilho.Resize += (_, _) => barra.Width = (int)(trilho.ClientSize.Width * proporcao);
            trilho.Controls.Add(barra);
            grafico.Controls.Add(trilho, 1, i + 1);
            grafico.Controls.Add(EstiloVisual.Rotulo(exibidas[i].Valor.ToString("C2", TabelaVisual.Cultura), 9), 2, i + 1);
        }
        _categoriasGrafico.Controls.Add(grafico);
    }

    private sealed record OpcaoConta(Guid? Id, string Nome);
    private sealed record DadosDashboard(List<Conta> Contas, List<Conta> Selecionadas, List<Transacao> Transacoes,
        List<Categoria> Categorias, List<CartaoCredito> Cartoes, List<FaturaCartao> Faturas, Guid? Filtro);
    private sealed record LinhaMovimentacao(Transacao Transacao, string Conta, string Categoria)
    {
        public DateTime Data => Transacao.Data;
        public string Descricao => Transacao.Descricao;
        public decimal Valor => Transacao.Valor;
        public string Tipo => (Transacao.TransferenciaId.HasValue ? "Transferência — " : "") +
            (Transacao.Tipo == TipoTransacao.Entrada ? "Entrada" : "Saída");
    }

    private Control CriarMovimentacoes()
    {
        PainelCartao painel = new() { Dock = DockStyle.Fill, Margin = Padding.Empty };
        Panel cabecalho = new() { Dock = DockStyle.Top, Height = 43 };
        Button extrato = EstiloVisual.Botao("Ver extrato");
        extrato.Width = 110;
        extrato.Dock = DockStyle.Right;
        extrato.Click += (_, _) => AbrirTransacoes?.Invoke(this, EventArgs.Empty);
        Label titulo = EstiloVisual.Rotulo("Últimas movimentações do mês", 15, true);
        cabecalho.Controls.Add(titulo);
        cabecalho.Controls.Add(extrato);

        DataGridView tabela = new()
        {
            Name = "dgvUltimasMovimentacoes",
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = EstiloVisual.Borda,
            EnableHeadersVisualStyles = false,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            ColumnHeadersHeight = 38,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };
        tabela.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        { BackColor = EstiloVisual.Fundo, ForeColor = EstiloVisual.Secundario, Font = new Font("Segoe UI", 9), Padding = new Padding(8, 0, 0, 0) };
        tabela.DefaultCellStyle = new DataGridViewCellStyle
        { BackColor = Color.White, ForeColor = EstiloVisual.Texto, SelectionBackColor = Color.FromArgb(232, 243, 239), SelectionForeColor = EstiloVisual.Texto, Font = new Font("Segoe UI", 9), Padding = new Padding(8, 0, 0, 0) };
        tabela.RowTemplate.Height = 43;
        _movimentacoes = tabela;
        TabelaVisual.Configurar(tabela,
            ("Data", "Data", "dd/MM/yyyy", 90), ("Descricao", "Descrição", "", 180),
            ("Conta", "Conta", "", 140), ("Categoria", "Categoria", "", 120),
            ("Tipo", "Tipo", "", 125), ("Valor", "Valor", "C2", 110));
        tabela.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || tabela.Rows[e.RowIndex].DataBoundItem is not LinhaMovimentacao linha || e.CellStyle is null) return;
            if (tabela.Columns[e.ColumnIndex].Name is not ("Tipo" or "Valor")) return;
            Color cor = linha.Transacao.TransferenciaId.HasValue ? EstiloVisual.MenuSelecionado
                : linha.Transacao.Tipo == TipoTransacao.Entrada ? EstiloVisual.Verde : EstiloVisual.Vermelho;
            e.CellStyle.ForeColor = e.CellStyle.SelectionForeColor = cor;
        };
        painel.Controls.Add(tabela);
        painel.Controls.Add(cabecalho);
        return painel;
    }
}
