using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

// Representa um pagamento já confirmado, não uma intenção de pagamento.
public sealed class PagamentoFatura
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid FaturaId { get; }
    public Guid ContaId { get; }
    public Guid TransacaoId { get; }
    public decimal Valor { get; }
    public DateTime Data { get; }

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
