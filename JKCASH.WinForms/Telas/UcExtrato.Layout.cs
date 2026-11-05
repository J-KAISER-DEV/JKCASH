using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato
{
    private void ConfigurarLayoutExtrato()
    {
        // Recolher os filtros libera a mesma área para as duas abas do extrato.
        var layout = new TableLayoutPanel
        {
            Name = "tlpLayoutExtrato", Dock = DockStyle.Fill,
            ColumnCount = 1, RowCount = 4, Margin = Padding.Empty
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 218));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var cabecalho = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        cabecalho.Controls.Add(lblTitulo);
        cabecalho.Controls.Add(lblDescricao);
        layout.Controls.Add(cabecalho, 0, 0);

        var barra = new Panel { Dock = DockStyle.Fill, Margin = new Padding(24, 0, 24, 0) };
        var alternar = EstiloVisual.Botao("Minimizar filtros");
        alternar.Name = "btnAlternarFiltrosExtrato";
        alternar.AccessibleName = "Mostrar ou minimizar filtros de período, contas e cartões";
        alternar.Size = new Size(210, 30);
        barra.Controls.Add(alternar);
        layout.Controls.Add(barra, 0, 1);

        pnlFiltrosExtrato.Dock = DockStyle.Fill;
        pnlFiltrosExtrato.Margin = new Padding(24, 0, 24, 8);
        layout.Controls.Add(pnlFiltrosExtrato, 0, 2);
        tabExtrato.Dock = DockStyle.Fill;
        tabExtrato.Margin = new Padding(24, 8, 24, 24);
        layout.Controls.Add(tabExtrato, 0, 3);
        Controls.Add(layout);

        bool filtrosVisiveis = true;
        alternar.Click += (_, _) =>
        {
            filtrosVisiveis = !filtrosVisiveis;
            layout.SuspendLayout();
            pnlFiltrosExtrato.Visible = filtrosVisiveis;
            layout.RowStyles[2].Height = filtrosVisiveis ? 218 : 0;
            alternar.Text = filtrosVisiveis ? "Minimizar filtros" : "Mostrar filtros";
            layout.ResumeLayout(true);
        };
    }

    private void ConfigurarAbasRelatorioGastos()
    {
        var abas = new TabControl { Name = "tabVisualizacaoGastos", Dock = DockStyle.Fill };
        var tabela = new TabPage("Tabela") { Name = "tabTabelaGastos", Padding = new Padding(3) };
        var grafico = new TabPage("Gráfico") { Name = "tabGraficoGastos", Padding = new Padding(3) };
        dgvResumoGastos.Dock = DockStyle.Fill;
        tabela.Controls.Add(dgvResumoGastos);
        grafico.Controls.Add(_painelGraficoGastos);
        abas.TabPages.AddRange([tabela, grafico]);

        var layout = new TableLayoutPanel
        {
            Name = "tlpRelatorioGastos", Dock = DockStyle.Fill,
            ColumnCount = 1, RowCount = 4, Margin = Padding.Empty
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var barra = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        var alternar = EstiloVisual.Botao("Mostrar filtros do relatório");
        alternar.Name = "btnAlternarFiltrosGastos";
        alternar.AccessibleName = "Mostrar ou minimizar filtros do relatório de gastos";
        alternar.Size = new Size(255, 30);
        barra.Controls.Add(alternar);
        layout.Controls.Add(barra, 0, 0);
        _painelFiltrosGastos.Dock = DockStyle.Fill;
        _painelFiltrosGastos.Visible = false;
        _painelFiltrosGastos.Margin = Padding.Empty;
        layout.Controls.Add(_painelFiltrosGastos, 0, 1);
        _statusGastos.Dock = DockStyle.Fill;
        layout.Controls.Add(_statusGastos, 0, 2);
        layout.Controls.Add(abas, 0, 3);
        tabRelatorioGastos.Controls.Add(layout);

        bool filtrosVisiveis = false;
        alternar.Click += (_, _) =>
        {
            filtrosVisiveis = !filtrosVisiveis;
            layout.SuspendLayout();
            _painelFiltrosGastos.Visible = filtrosVisiveis;
            layout.RowStyles[1].Height = filtrosVisiveis ? 232 : 0;
            alternar.Text = filtrosVisiveis ? "Minimizar filtros do relatório" : "Mostrar filtros do relatório";
            layout.ResumeLayout(true);
        };
    }
}
