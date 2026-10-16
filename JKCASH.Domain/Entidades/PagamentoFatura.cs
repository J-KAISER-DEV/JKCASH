using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

// Representa um pagamento já confirmado, não uma intenção de pagamento.
public sealed class PagamentoFatura
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FaturaId { get; private set; }
    public Guid ContaId { get; private set; }
    public Guid TransacaoId { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }

    private PagamentoFatura() { }
    public PagamentoFatura(Guid faturaId, Guid contaId, Guid transacaoId,
        decimal valor, DateTime data)
    {
        Validacao.GuidNaoVazio(faturaId, nameof(faturaId));
        Validacao.GuidNaoVazio(contaId, nameof(contaId));
        Validacao.GuidNaoVazio(transacaoId, nameof(transacaoId));
        Validacao.ValorMonetarioPositivo(valor, nameof(valor));
        FaturaId = faturaId;
        ContaId = contaId;
        TransacaoId = transacaoId;
        Valor = valor;
        Data = data.Date;
    }
}
