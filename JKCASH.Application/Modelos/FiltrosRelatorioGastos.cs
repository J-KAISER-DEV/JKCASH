using JKCASH.Domain.Enums;
using JKCASH.Domain.Entidades;

namespace JKCASH.Application.Modelos;

public sealed record FiltrosRelatorioGastos(
    IReadOnlyCollection<Guid> Categorias,
    IReadOnlyCollection<ImportanciaTransacao> Importancias,
    IReadOnlyCollection<UrgenciaTransacao> Urgencias)
{
    public IReadOnlyCollection<string> NomesGastos { get; init; } = [];
    public bool SemGastoVinculado { get; init; }
    public IReadOnlyCollection<NaturezaGasto> Naturezas { get; init; } = [];
    public IReadOnlyCollection<NecessidadeGasto> Necessidades { get; init; } = [];
    public IReadOnlyCollection<PlanejamentoTransacao> Planejamentos { get; init; } = [];
    public IReadOnlyCollection<FrequenciaGasto> Frequencias { get; init; } = [];
}
