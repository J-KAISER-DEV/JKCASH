using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

public static class ResumoOperacao
{
    public static string Conta(Conta c) =>
        $"Conta: {c.Nome}\nBanco: {c.Banco}\nTipo: {c.Tipo}\nDestinação: {c.Destinacao}\nSaldo inicial: {c.Saldo.SaldoInicial:C2}\nCheque especial: {c.Saldo.LimiteChequeEspecial:C2}";
    public static string Categoria(Categoria c) => $"Categoria: {c.Nome}";
    public static string Cartao(CartaoCredito c, string conta) =>
        $"Cartão: {c.Nome}\nConta: {conta}\nLimite: {c.Limite:C2}\nFechamento: dia {c.DiaFechamento}\nVencimento: dia {c.DiaVencimento}\nCompra no fechamento: {(c.CompraNoFechamentoNaProximaFatura ? "próxima fatura" : "fatura atual")}";
    public static string Transacao(Transacao t, string conta, string categoria) =>
        $"Conta: {conta}\nTipo: {t.Tipo}\nValor: {t.Valor:C2}\nData: {t.Data:dd/MM/yyyy}\nCategoria: {categoria}\nDescrição: {t.Descricao}" +
        (t.Tipo == TipoTransacao.Saida || t.TransferenciaId.HasValue
            ? Avaliacao(t.Importancia, t.Planejamento, t.Urgencia, t.Justificativa) : "\nAvaliação: Não se aplica.");
    public static string Transferencia(Transferencia t, string origem, string destino) =>
        $"Origem: {origem}\nDestino: {destino}\nValor: {t.Valor:C2}\nData: {t.Saida.Data:dd/MM/yyyy}\nDescrição: {t.Saida.Descricao}" +
        Avaliacao(t.Saida.Importancia, t.Saida.Planejamento, t.Saida.Urgencia, t.Saida.Justificativa);
    public static string Compra(CompraCartao c, string cartao, string categoria) =>
        $"Cartão: {cartao}\nCategoria: {categoria}\nCompra: {c.Descricao}\nData: {c.Data:dd/MM/yyyy}\nTotal: {c.Valor:C2}\nParcelas: {c.QuantidadeParcelas}\nPrimeiro vencimento: {c.Parcelas[0].Ciclo.Vencimento:dd/MM/yyyy}\nO valor total compromete o limite do cartão." +
        Avaliacao(c.Importancia, c.Planejamento, c.Urgencia, c.Justificativa);
    public static string Pagamento(FaturaCartao f, Conta conta, CartaoCredito cartao, decimal valor) =>
        $"Cartão: {cartao.Nome}\nFatura: {f.Ciclo.Vencimento:dd/MM/yyyy}\nConta pagadora: {conta.Nome} ({conta.Banco})\nValor: {valor:C2}\nData: {DateTime.Today:dd/MM/yyyy}\nPendente após pagar: {f.SaldoPendente - valor:C2}\nSaldo previsto da conta: {conta.Saldo.SaldoAtual - valor:C2}\nLimite a liberar: {valor:C2}";

    private static string Avaliacao(ImportanciaTransacao? importancia, PlanejamentoTransacao? planejamento,
        UrgenciaTransacao? urgencia, string? justificativa) =>
        $"\nImportância: {importancia}\nPlanejamento: {planejamento}\nUrgência: {urgencia}\nJustificativa: {justificativa}";
}
