using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public sealed class UcDashboard : UserControl
{
    public event EventHandler? AbrirTransacoes;
    public event EventHandler? AbrirFaturas;

    public UcDashboard()
    {
        Dock = DockStyle.Fill;
        BackColor = EstiloVisual.Fundo;
        AutoScroll = true;
        Font = new Font("Segoe UI", 10);

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            Height = 826,
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
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.Controls.Add(CriarCabecalho(), 0, 0);
        layout.Controls.Add(CriarIndicadores(), 0, 1);
        layout.Controls.Add(CriarGraficos(), 0, 2);
        layout.Controls.Add(CriarMovimentacoes(), 0, 3);
        Label aviso = EstiloVisual.Rotulo("Prévia visual • Valores ilustrativos • Sem conexão com o banco", 9);
        aviso.ForeColor = EstiloVisual.Secundario;
        layout.Controls.Add(aviso, 0, 4);
        Controls.Add(layout);
        SizeChanged += (_, _) => layout.Height = Math.Max(826, ClientSize.Height - 2);
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
        FlowLayoutPanel acoes = new() { Dock = DockStyle.Fill, Padding = new Padding(0, 19, 0, 0), WrapContents = false };
        foreach (string texto in new[] { "Todas as contas", "Outubro de 2026" })
        {
            ComboBox filtro = new()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 153,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 6, 10, 0)
            };
            filtro.Items.Add(texto);
            filtro.SelectedIndex = 0;
            filtro.AccessibleName = texto == "Todas as contas" ? "Filtro de contas demonstrativo" : "Período demonstrativo";
            acoes.Controls.Add(filtro);
        }
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
            ("Saldo total", "R$ 4.250,00", "Soma das contas", EstiloVisual.Texto),
            ("Receitas do mês", "R$ 3.000,00", "Entradas", EstiloVisual.Verde),
            ("Despesas do mês", "R$ 1.750,00", "Saídas", EstiloVisual.Vermelho),
            ("Faturas pendentes", "R$ 680,00", "Neste mês", EstiloVisual.Texto)
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
        TableLayoutPanel grafico = new() { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 5 };
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        grafico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        grafico.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        Label titulo = EstiloVisual.Rotulo("Gastos por categoria", 15, true);
        grafico.Controls.Add(titulo, 0, 0);
        grafico.SetColumnSpan(titulo, 3);
        string[] categorias = ["Alimentação", "Moradia", "Estudos", "Lazer"];
        int[] valores = [650, 600, 300, 200];
        Color[] cores = [Color.FromArgb(31, 85, 121), Color.FromArgb(58, 130, 159), Color.FromArgb(97, 167, 188), Color.FromArgb(154, 199, 212)];
        for (int i = 0; i < 4; i++)
        {
            grafico.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            grafico.Controls.Add(EstiloVisual.Rotulo(categorias[i]), 0, i + 1);
            Panel trilho = new() { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(3, 9, 10, 9) };
            Panel barra = new() { Dock = DockStyle.Left, BackColor = cores[i] };
            double proporcao = valores[i] / 650d;
            trilho.Resize += (_, _) => barra.Width = (int)(trilho.ClientSize.Width * proporcao);
            trilho.Controls.Add(barra);
            grafico.Controls.Add(trilho, 1, i + 1);
            grafico.Controls.Add(EstiloVisual.Rotulo($"R$ {valores[i]:N2}"), 2, i + 1);
        }
        gastos.Controls.Add(grafico);
        linha.Controls.Add(gastos, 0, 0);
        PainelCartao credito = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 14) };
        TableLayoutPanel info = new() { Dock = DockStyle.Fill, RowCount = 7, ColumnCount = 1 };
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        float[] alturas = [34, 25, 20, 44, 22, 16, 38];
        foreach (float altura in alturas) info.RowStyles.Add(new RowStyle(SizeType.Absolute, altura));
        info.Controls.Add(EstiloVisual.Rotulo("Cartões de crédito", 15, true), 0, 0);
        info.Controls.Add(EstiloVisual.Rotulo("Cartão principal", 11, true), 0, 1);
        info.Controls.Add(EstiloVisual.Rotulo("Limite disponível", 9), 0, 2);
        Label disponivel = EstiloVisual.Rotulo("R$ 2.320,00", 20, true);
        disponivel.ForeColor = EstiloVisual.Verde;
        info.Controls.Add(disponivel, 0, 3);
        info.Controls.Add(EstiloVisual.Rotulo("Limite total: R$ 3.000,00", 9), 0, 4);
        Panel faixa = new() { Dock = DockStyle.Fill, BackColor = EstiloVisual.Borda, Margin = new Padding(0, 6, 0, 6) };
        Panel preenchimento = new() { Dock = DockStyle.Left, BackColor = EstiloVisual.Verde };
        faixa.Resize += (_, _) => preenchimento.Width = (int)(faixa.Width * 2320d / 3000d);
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

    private void InitializeComponent()
    {

    }

    private Control CriarMovimentacoes()
    {
        PainelCartao painel = new() { Dock = DockStyle.Fill, Margin = Padding.Empty };
        Panel cabecalho = new() { Dock = DockStyle.Top, Height = 43 };
        Button extrato = EstiloVisual.Botao("Ver extrato");
        extrato.Width = 110;
        extrato.Dock = DockStyle.Right;
        extrato.Click += (_, _) => AbrirTransacoes?.Invoke(this, EventArgs.Empty);
        Label titulo = EstiloVisual.Rotulo("Últimas movimentações", 15, true);
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
        foreach (string coluna in new[] { "Data", "Descrição", "Conta", "Categoria", "Tipo", "Valor" }) tabela.Columns.Add(coluna, coluna);
        tabela.Columns[1].FillWeight = 160;
        tabela.Columns[2].FillWeight = 130;
        tabela.Columns[5].FillWeight = 120;
        tabela.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        tabela.Rows.Add("17/10/2026", "Compra de livros", "Conta principal", "Estudos", "Saída", "− R$ 150,00");
        tabela.Rows.Add("16/10/2026", "Salário", "Conta principal", "Salário", "Entrada", "+ R$ 3.000,00");
        tabela.Rows.Add("15/10/2026", "Supermercado", "Conta principal", "Alimentação", "Saída", "− R$ 180,00");
        for (int i = 0; i < tabela.Rows.Count; i++)
        {
            Color cor = i == 1 ? EstiloVisual.Verde : EstiloVisual.Vermelho;
            tabela.Rows[i].Cells[4].Style.ForeColor = cor;
            tabela.Rows[i].Cells[5].Style.ForeColor = cor;
            tabela.Rows[i].Cells[4].Style.SelectionForeColor = cor;
            tabela.Rows[i].Cells[5].Style.SelectionForeColor = cor;
        }
        tabela.ClearSelection();
        painel.Controls.Add(tabela);
        painel.Controls.Add(cabecalho);
        return painel;
    }
}
