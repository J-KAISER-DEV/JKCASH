using JKCASH.Domain.ObjetosDeValor;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

public sealed class ParcelaCartao
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid CompraId { get; }
    public Guid CartaoId { get; }
    public int Numero { get; }
    public int Quantidade { get; }
    public decimal Valor { get; }
    public CicloFatura Ciclo { get; }

    internal ParcelaCartao(Guid compraId, Guid cartaoId, int numero, int quantidade,
        decimal valor, CicloFatura ciclo)
    {
        Validacao.GuidNaoVazio(compraId, nameof(compraId));
        Validacao.GuidNaoVazio(cartaoId, nameof(cartaoId));
        Validacao.InteiroNoIntervalo(numero, 1, quantidade, nameof(numero));
        Validacao.ValorMonetarioPositivo(valor, nameof(valor));
        CompraId = compraId;
        CartaoId = cartaoId;
        Numero = numero;
        Quantidade = quantidade;
        Valor = valor;
        Ciclo = ciclo;
    }
}
