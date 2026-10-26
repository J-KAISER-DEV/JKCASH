using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

public sealed record ExtratoItem(Guid Id, DateTime Data, string Conta, string Banco,
    string Descricao, string Categoria, TipoTransacao Tipo, decimal Valor, bool Transferencia,
    ImportanciaTransacao? Importancia, PlanejamentoTransacao? Planejamento,
    UrgenciaTransacao? Urgencia, string? Justificativa)
{
    public string Movimento => (Transferencia ? "Transferência — " : "") + Tipo;
}

public sealed record ExtratoTotais(decimal Entradas, decimal Saidas,
    decimal TransferenciasRecebidas, decimal TransferenciasEnviadas);

public sealed record ExtratoPagina(int Pagina, int TamanhoPagina, int QuantidadeTotal,
    ExtratoTotais Totais, IReadOnlyList<ExtratoItem> Itens)
{
    public int TotalPaginas => QuantidadeTotal == 0 ? 1 : (QuantidadeTotal - 1) / TamanhoPagina + 1;
}
