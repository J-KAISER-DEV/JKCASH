using JKCASH.Domain.Enums;
namespace JKCASH.Api.Modelos;

public sealed class ContaResposta
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Banco {  get; init; } = string.Empty;
    public TipoConta Tipo {  get; init; }
    public DestinacaoConta Destinacao { get; init; }
    public decimal SaldoInicial { get; init; }
    public decimal SaldoAtual { get; init; }
    public decimal LimiteChequeEspecial { get; init; }
    public bool Ativa {  get; init; }

}
