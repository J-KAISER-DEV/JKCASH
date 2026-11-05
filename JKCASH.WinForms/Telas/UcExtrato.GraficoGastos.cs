using JKCASH.Application.Modelos;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato
{
    private readonly Panel _painelGraficoGastos = new()
    {
        Name = "pnlGraficoGastos",
        Dock = DockStyle.Top,
        BackColor = Color.White,
        Padding = new Padding(8, 0, 8, 6)
    };

    private readonly Panel _barrasGastos = new()
    {
        Dock = DockStyle.Fill,
        AutoScroll = true
    };

    private void ConfigurarGraficoGastos()
    {
        var titulo = EstiloVisual.Rotulo("Gastos por categoria", 11, true);
        titulo.Dock = DockStyle.Top;
        titulo.Height = 28;
        _painelGraficoGastos.Controls.Add(_barrasGastos);
        _painelGraficoGastos.Controls.Add(titulo);
        tabRelatorioGastos.Controls.Add(_painelGraficoGastos);
        tabRelatorioGastos.Controls.SetChildIndex(_painelGraficoGastos, 1);
        tabRelatorioGastos.SizeChanged += (_, _) => AjustarAlturaGrafico();
        ExibirGraficoGastos([]);
    }

    private void AjustarAlturaGrafico()
    {
        int disponivel = tabRelatorioGastos.DisplayRectangle.Height -
            _painelFiltrosGastos.Height - _statusGastos.Height;
        if (disponivel < 180)
        {
            _painelGraficoGastos.Dock = DockStyle.Left;
            _painelGraficoGastos.Width = tabRelatorioGastos.DisplayRectangle.Width * 45 / 100;
            return;
        }

        _painelGraficoGastos.Dock = DockStyle.Top;
        int desejada = (_barrasGastos.Controls.Count == 0 ? 28 : _barrasGastos.Controls[0].Height) + 34;
        _painelGraficoGastos.Height = Math.Clamp(disponivel / 2, 0, Math.Min(200, desejada));
    }

    private void ExibirGraficoGastos(IReadOnlyList<GastoPorCategoria> gastos)
    {
        _barrasGastos.SuspendLayout();
        try
        {
            while (_barrasGastos.Controls.Count > 0)
                _barrasGastos.Controls[0].Dispose();

            if (gastos.Count == 0)
            {
                var vazio = EstiloVisual.Rotulo("Nenhum gasto para exibir.", 9);
                vazio.Dock = DockStyle.Top;
                vazio.Height = 26;
                vazio.ForeColor = EstiloVisual.Secundario;
                _barrasGastos.Controls.Add(vazio);
                return;
            }

            const int alturaLinha = 29;
            var linhas = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                RowCount = gastos.Count,
                Height = gastos.Count * alturaLinha
            };
            linhas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            linhas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            linhas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            decimal maiorTotal = gastos.Max(g => g.Total);

            for (int i = 0; i < gastos.Count; i++)
            {
                GastoPorCategoria gasto = gastos[i];
                linhas.RowStyles.Add(new RowStyle(SizeType.Absolute, alturaLinha));

                var nome = EstiloVisual.Rotulo(gasto.Categoria, 9);
                nome.AutoEllipsis = true;
                nome.Dock = DockStyle.Fill;

                var trilho = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = EstiloVisual.Fundo,
                    Margin = new Padding(4, 7, 8, 7)
                };
                var barra = new Panel { Dock = DockStyle.Left, BackColor = EstiloVisual.Verde };
                decimal proporcao = maiorTotal > 0 ? gasto.Total / maiorTotal : 0m;
                trilho.Resize += (_, _) => barra.Width = (int)(trilho.ClientSize.Width * proporcao);
                trilho.Controls.Add(barra);
                barra.Width = (int)(trilho.ClientSize.Width * proporcao);

                var valor = EstiloVisual.Rotulo(Moeda(gasto.Total), 9);
                valor.Dock = DockStyle.Fill;
                valor.TextAlign = ContentAlignment.MiddleRight;
                linhas.Controls.Add(nome, 0, i);
                linhas.Controls.Add(trilho, 1, i);
                linhas.Controls.Add(valor, 2, i);
            }

            _barrasGastos.Controls.Add(linhas);
            _barrasGastos.AutoScrollPosition = Point.Empty;
        }
        finally
        {
            _barrasGastos.ResumeLayout();
            AjustarAlturaGrafico();
        }
    }

    private void LimparGastos()
    {
        dgvGastosPorCategoria.DataSource = null;
        ExibirGraficoGastos([]);
    }
}
