using JKCASH.Domain.Entidades;

namespace JKCASH.Domain.ObjetosDeValor;

public sealed record CicloFatura
{
    public DateTime Fechamento { get; }
    public DateTime Vencimento { get; }
    // Quando o cartão inclui compras no dia de fechamento, encerra ao fim desse dia.
    public DateTime Encerramento { get; }

    private CicloFatura(CartaoCredito cartao, DateTime mes)
    {
        Fechamento = DiaNoMes(mes, cartao.DiaFechamento);
        DateTime vencimento = DiaNoMes(mes, cartao.DiaVencimento);
        if (vencimento <= Fechamento)
            vencimento = DiaNoMes(mes.AddMonths(1), cartao.DiaVencimento);
        Vencimento = vencimento;
        Encerramento = cartao.CompraNoFechamentoNaProximaFatura
            ? Fechamento : Fechamento.AddDays(1);
    }

    public static CicloFatura DaCompra(CartaoCredito cartao, DateTime data)
    {
        ArgumentNullException.ThrowIfNull(cartao);
        DateTime mes = new(data.Year, data.Month, 1);
        DateTime fechamento = DiaNoMes(mes, cartao.DiaFechamento);
        if (data.Date > fechamento ||
            (data.Date == fechamento && cartao.CompraNoFechamentoNaProximaFatura))
            mes = mes.AddMonths(1);
        return new CicloFatura(cartao, mes);
    }

    public static CicloFatura DoMes(CartaoCredito cartao, DateTime mes) =>
        new(cartao, new DateTime(mes.Year, mes.Month, 1));

    private static DateTime DiaNoMes(DateTime mes, int dia) =>
        new(mes.Year, mes.Month, Math.Min(dia, DateTime.DaysInMonth(mes.Year, mes.Month)));
}
