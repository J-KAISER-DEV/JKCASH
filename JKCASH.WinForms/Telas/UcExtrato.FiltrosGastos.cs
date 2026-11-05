using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato
{
    private readonly ICategoriaRepositorio? _categoriasGastos;
    private TableLayoutPanel _painelFiltrosGastos = null!;
    private readonly CheckedListBox _gastosMarcados = CriarListaFiltro("clbNomesGastos");
    private readonly CheckedListBox _categoriasMarcadas = CriarListaFiltro("clbCategoriasGastos");
    private readonly CheckedListBox _naturezasMarcadas = CriarListaFiltro("clbNaturezasGastos");
    private readonly CheckedListBox _importanciasMarcadas = CriarListaFiltro("clbImportanciasGastos");
    private readonly CheckedListBox _necessidadesMarcadas = CriarListaFiltro("clbNecessidadesGastos");
    private readonly CheckedListBox _planejamentosMarcados = CriarListaFiltro("clbPlanejamentosGastos");
    private readonly CheckedListBox _frequenciasMarcadas = CriarListaFiltro("clbFrequenciasGastos");

    private void ConfigurarFiltrosGastos()
    {
        _painelFiltrosGastos = new TableLayoutPanel
        {
            Name = "pnlFiltrosGastos", Dock = DockStyle.Top, Height = 232,
            ColumnCount = 4, RowCount = 4, Padding = new Padding(8), BackColor = EstiloVisual.Fundo
        };
        for (int coluna = 0; coluna < 4; coluna++)
            _painelFiltrosGastos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        for (int linha = 0; linha < 2; linha++)
        {
            _painelFiltrosGastos.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            _painelFiltrosGastos.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        }
        AdicionarLista("Gasto", _gastosMarcados, 0);
        AdicionarLista("Categoria", _categoriasMarcadas, 1);
        AdicionarLista("Natureza", _naturezasMarcadas, 2);
        AdicionarLista("Frequência", _frequenciasMarcadas, 3);
        AdicionarLista("Importância", _importanciasMarcadas, 0, 1);
        AdicionarLista("Necessidade", _necessidadesMarcadas, 1, 1);
        AdicionarLista("Planejamento", _planejamentosMarcados, 2, 1);
        var ajuda = new Label { Text = "Sem marcações = todas as opções.\n\nInclui gastos salvos, avulsos e nomes usados no histórico.",
            Dock = DockStyle.Fill, ForeColor = EstiloVisual.Secundario };
        _painelFiltrosGastos.Controls.Add(ajuda, 3, 2); _painelFiltrosGastos.SetRowSpan(ajuda, 2);
        PreencherEnums<ImportanciaTransacao>(_importanciasMarcadas);
        PreencherEnums<NaturezaGasto>(_naturezasMarcadas);
        PreencherEnums<NecessidadeGasto>(_necessidadesMarcadas);
        PreencherEnums<PlanejamentoTransacao>(_planejamentosMarcados);
        PreencherEnums<FrequenciaGasto>(_frequenciasMarcadas);
        tabRelatorioGastos.Controls.Add(_painelFiltrosGastos);
        _painelFiltrosGastos.SendToBack();
    }

    private static string NomeOpcao(object? valor) => valor?.ToString() switch
    {
        "Media" => "Média", "Servico" => "Serviço", "NaoPlanejada" => "Não planejado",
        "Planejada" => "Planejado", null => "Não informado", var nome => nome
    };
    private static void PreencherEnums<T>(CheckedListBox lista) where T : struct, Enum
    {
        foreach (T valor in Enum.GetValues<T>()) lista.Items.Add(new OpcaoFiltro<T>(valor, NomeOpcao(valor)));
    }
    private void AdicionarLista(string titulo, CheckedListBox lista, int coluna, int grupo = 0)
    {
        _painelFiltrosGastos.Controls.Add(new Label
        { Text = titulo, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, coluna, grupo * 2);
        _painelFiltrosGastos.Controls.Add(lista, coluna, grupo * 2 + 1);
        lista.ItemCheck += (_, _) => InvalidarFiltroGastos();
    }
    private static CheckedListBox CriarListaFiltro(string nome) => new()
    { Name = nome, CheckOnClick = true, Dock = DockStyle.Fill, IntegralHeight = false };

    private async Task CarregarFiltrosGastosAsync()
    {
        if (_categoriasGastos is null || _relatorioGastos is null) return;
        var categoriasSelecionadas = ValoresMarcados<Guid>(_categoriasMarcadas).ToHashSet();
        var gastosSelecionados = ValoresMarcados<string?>(_gastosMarcados).ToHashSet();
        var dados = await Task.Run(() => (_categoriasGastos.ListarTodas(), _relatorioGastos.ListarNomesGastos()));
        if (IsDisposed || Disposing) return;
        _categoriasMarcadas.Items.Clear();
        foreach (var categoria in dados.Item1.OrderBy(c => c.Nome))
            _categoriasMarcadas.Items.Add(new OpcaoFiltro<Guid>(categoria.Id, categoria.Nome), categoriasSelecionadas.Contains(categoria.Id));
        _gastosMarcados.Items.Clear();
        foreach (var nome in dados.Item2)
            _gastosMarcados.Items.Add(new OpcaoFiltro<string?>(nome, nome), gastosSelecionados.Contains(nome));
        _gastosMarcados.Items.Add(new OpcaoFiltro<string?>(null, "Sem gasto vinculado (legado)"), gastosSelecionados.Contains(null));
    }
    private void InvalidarFiltroGastos()
    {
        if (_consultandoExtrato || IsDisposed || Disposing) return;
        LimparGastos(); MostrarStatusGastos("Filtros alterados. Clique em Consultar gastos para aplicar.");
    }
    private FiltrosRelatorioGastos LerFiltrosGastos() => new(
        ValoresMarcados<Guid>(_categoriasMarcadas), ValoresMarcados<ImportanciaTransacao>(_importanciasMarcadas), [])
    {
        NomesGastos = ValoresMarcados<string?>(_gastosMarcados).OfType<string>().ToArray(),
        SemGastoVinculado = ValoresMarcados<string?>(_gastosMarcados).Contains(null),
        Naturezas = ValoresMarcados<NaturezaGasto>(_naturezasMarcadas),
        Necessidades = ValoresMarcados<NecessidadeGasto>(_necessidadesMarcadas),
        Planejamentos = ValoresMarcados<PlanejamentoTransacao>(_planejamentosMarcados),
        Frequencias = ValoresMarcados<FrequenciaGasto>(_frequenciasMarcadas)
    };
    private static T[] ValoresMarcados<T>(CheckedListBox lista) =>
        lista.CheckedItems.Cast<OpcaoFiltro<T>>().Select(item => item.Valor).ToArray();
    private sealed record OpcaoFiltro<T>(T Valor, string Nome)
    { public override string ToString() => Nome; }
}
