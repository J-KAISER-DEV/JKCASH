using JKCASH.Domain.ObjetosDeValor;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

public sealed class ParcelaCartao
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CompraId { get; private set; }
    public Guid CartaoId { get; private set; }
    public int Numero { get; private set; }
    public int Quantidade { get; private set; }
    public decimal Valor { get; private set; }
    public CicloFatura Ciclo => Fatura?.Ciclo
    ?? _ciclo
    ?? throw new InvalidOperationException(
        "O ciclo da parcela não está disponível.");
    public Guid FaturaId { get; private set; }
    public FaturaCartao? Fatura { get; private set; }

    private CicloFatura? _ciclo;
    private ParcelaCartao() { }
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
        ArgumentNullException.ThrowIfNull(ciclo);
        _ciclo = ciclo;
    }
}
