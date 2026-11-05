using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato
{
    private readonly ICategoriaRepositorio? _categoriasGastos;
    private bool _categoriasGastosCarregadas;
    private TableLayoutPanel _painelFiltrosGastos = null!;
    private readonly CheckedListBox _categoriasMarcadas = CriarListaFiltro("clbCategoriasGastos");
    private readonly CheckedListBox _importanciasMarcadas = CriarListaFiltro("clbImportanciasGastos");
    private readonly CheckedListBox _urgenciasMarcadas = CriarListaFiltro("clbUrgenciasGastos");

    private void ConfigurarFiltrosGastos()
    {
        _painelFiltrosGastos = new TableLayoutPanel
        {
            Name = "pnlFiltrosGastos",
            Dock = DockStyle.Top,
            Height = 138,
            ColumnCount = 3,
            RowCount = 2,
            Padding = new Padding(8),
            BackColor = EstiloVisual.Fundo
        };

        for (int coluna = 0; coluna < 3; coluna++)
            _painelFiltrosGastos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));

        _painelFiltrosGastos.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        _painelFiltrosGastos.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        AdicionarLista("Categoria", _categoriasMarcadas, 0);
        AdicionarLista("Importância", _importanciasMarcadas, 1);
        AdicionarLista("Urgência", _urgenciasMarcadas, 2);

        foreach (ImportanciaTransacao importancia in Enum.GetValues<ImportanciaTransacao>())
        {
            string nome = importancia == ImportanciaTransacao.Media ? "Média" : importancia.ToString();
            _importanciasMarcadas.Items.Add(new OpcaoFiltro<ImportanciaTransacao>(importancia, nome));
        }

        foreach (UrgenciaTransacao urgencia in Enum.GetValues<UrgenciaTransacao>())
        {
            string nome = urgencia == UrgenciaTransacao.SemUrgencia ? "Sem urgência" : urgencia.ToString();
            _urgenciasMarcadas.Items.Add(new OpcaoFiltro<UrgenciaTransacao>(urgencia, nome));
        }

        tabRelatorioGastos.Controls.Add(_painelFiltrosGastos);
        _painelFiltrosGastos.SendToBack();
    }

    private void AdicionarLista(string titulo, CheckedListBox lista, int coluna)
    {
        _painelFiltrosGastos.Controls.Add(new Label
        {
            Text = titulo,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, coluna, 0);
        _painelFiltrosGastos.Controls.Add(lista, coluna, 1);
        lista.ItemCheck += (_, _) => InvalidarFiltroGastos();
    }

    private static CheckedListBox CriarListaFiltro(string nome) => new()
    {
        Name = nome,
        CheckOnClick = true,
        Dock = DockStyle.Fill,
        IntegralHeight = false
    };

    private async Task CarregarCategoriasGastosAsync()
    {
        if (_categoriasGastosCarregadas || _categoriasGastos is null) return;

        var categorias = await Task.Run(() => _categoriasGastos.ListarTodas()
            .OrderBy(c => c.Nome).ToList());
        if (IsDisposed || Disposing) return;

        _categoriasMarcadas.Items.Clear();
        foreach (var categoria in categorias)
            _categoriasMarcadas.Items.Add(new OpcaoFiltro<Guid>(categoria.Id, categoria.Nome));

        _categoriasGastosCarregadas = true;
    }

    private void InvalidarFiltroGastos()
    {
        if (_consultandoExtrato || IsDisposed || Disposing) return;
        LimparGastos();
        MostrarStatusGastos("Filtros alterados. Clique em Consultar gastos para aplicar.");
    }

    private FiltrosRelatorioGastos LerFiltrosGastos() => new(
        ValoresMarcados<Guid>(_categoriasMarcadas),
        ValoresMarcados<ImportanciaTransacao>(_importanciasMarcadas),
        ValoresMarcados<UrgenciaTransacao>(_urgenciasMarcadas));

    private static T[] ValoresMarcados<T>(CheckedListBox lista) =>
        lista.CheckedItems.Cast<OpcaoFiltro<T>>().Select(item => item.Valor).ToArray();

    private sealed record OpcaoFiltro<T>(T Valor, string Nome)
    {
        public override string ToString() => Nome;
    }
}
