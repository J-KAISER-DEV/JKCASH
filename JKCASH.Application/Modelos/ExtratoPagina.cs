using JKCASH.Domain.ObjetosDeValor;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

public sealed record ExtratoItem(Guid Id, DateTime Data, string Conta, string Banco,
    string Descricao, string Categoria, TipoTransacao Tipo, decimal Valor, bool Transferencia,
    ImportanciaTransacao? Importancia, PlanejamentoTransacao? Planejamento,
    UrgenciaTransacao? Urgencia, string? Justificativa, int? QuantidadeParcelas = null)
{
    public DetalhesGasto? Gasto { get; init; }
    public string NomeGasto => Gasto?.Nome ?? (Tipo == TipoTransacao.Entrada || Transferencia ? "—" : "Sem gasto vinculado");
    public string NaturezaGasto => Gasto?.Natureza.ToString() == "Servico" ? "Serviço" : Gasto?.Natureza.ToString() ?? "—";
    public string NecessidadeGasto => Gasto?.Necessidade.ToString() == "Media" ? "Média" : Gasto?.Necessidade.ToString() ?? "—";
    public string FrequenciaGasto => Gasto?.Frequencia.ToString() ?? "—";
    public IReadOnlyList<ExtratoParcela> Parcelas { get; init; } = [];

    public string Movimento => QuantidadeParcelas.HasValue ? "Compra no cartão"
        : (Transferencia ? "Transferência — " : "") + Tipo;
}

public sealed record ExtratoParcela(int Numero, int Quantidade, decimal Valor,
    DateTime Fechamento, DateTime Vencimento);

public sealed record ExtratoTotais(decimal Entradas, decimal Saidas,
    decimal TransferenciasRecebidas, decimal TransferenciasEnviadas);

public sealed record ExtratoPagina(int Pagina, int TamanhoPagina, int QuantidadeTotal,
    ExtratoTotais Totais, IReadOnlyList<ExtratoItem> Itens)
{
    public int TotalPaginas => QuantidadeTotal == 0 ? 1 : (QuantidadeTotal - 1) / TamanhoPagina + 1;
}
