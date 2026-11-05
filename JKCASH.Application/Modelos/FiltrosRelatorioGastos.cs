using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

public sealed record FiltrosRelatorioGastos(
    IReadOnlyCollection<Guid> Categorias,
    IReadOnlyCollection<ImportanciaTransacao> Importancias,
    IReadOnlyCollection<UrgenciaTransacao> Urgencias);
