using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

// O agrupamento usa os dados do lançamento, preservando sua classificação histórica.
public sealed record ResumoGasto(Guid? CadastroId, string Nome, string Categoria,
    NaturezaGasto? Natureza, ImportanciaTransacao? Importancia, NecessidadeGasto? Necessidade,
    PlanejamentoTransacao? Planejamento, FrequenciaGasto? Frequencia, int Quantidade, decimal Total);
