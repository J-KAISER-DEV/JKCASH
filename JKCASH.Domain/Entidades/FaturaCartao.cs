using JKCASH.Domain.Enums;
using JKCASH.Domain.ObjetosDeValor;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

// Cada alteração produz uma nova versão. A versão anterior continua intacta em caso de falha.
public sealed class FaturaCartao
{
    private FaturaCartao() { }
    private readonly List<ParcelaCartao> _parcelas = new();
    private readonly List<PagamentoFatura> _pagamentos = new();
    public Guid Id { get; private set; }
    public Guid CartaoId { get; private set; }
    public CicloFatura Ciclo { get; private set; } = null!;

    public IReadOnlyList<ParcelaCartao> Parcelas =>
        _parcelas.AsReadOnly();

    public IReadOnlyList<PagamentoFatura> Pagamentos =>
        _pagamentos.AsReadOnly();
    public decimal Total => Parcelas.Sum(parcela => parcela.Valor);
    public decimal TotalPago => Pagamentos.Sum(pagamento => pagamento.Valor);
    public decimal SaldoPendente => Total - TotalPago;
    public SituacaoPagamentoFatura SituacaoPagamento => TotalPago == 0m
        ? SituacaoPagamentoFatura.SemPagamento
        : SaldoPendente == 0m ? SituacaoPagamentoFatura.Quitado : SituacaoPagamentoFatura.Parcial;

    private FaturaCartao(Guid id, Guid cartaoId, CicloFatura ciclo,
        IEnumerable<ParcelaCartao> parcelas, IEnumerable<PagamentoFatura> pagamentos)
    {
        Id = id;
        CartaoId = cartaoId;
        Ciclo = ciclo;
        _parcelas = parcelas.ToList();
        _pagamentos = pagamentos.ToList();
    }

    public static FaturaCartao Criar(ParcelaCartao parcela) =>
        new(Guid.NewGuid(), parcela.CartaoId, parcela.Ciclo, new[] { parcela }, Array.Empty<PagamentoFatura>());

    public SituacaoFatura SituacaoEm(DateTime data)
    {
        if (data.Date < Ciclo.Encerramento) return SituacaoFatura.Aberta;
        return SaldoPendente == 0m ? SituacaoFatura.Paga : SituacaoFatura.Fechada;
    }

    public bool EstaAtrasadaEm(DateTime data) => data.Date > Ciclo.Vencimento && SaldoPendente > 0m;

    public FaturaCartao ComParcela(ParcelaCartao parcela, DateTime hoje)
    {
        if (SituacaoEm(hoje) != SituacaoFatura.Aberta)
            throw new InvalidOperationException("A fatura já fechou e não recebe novas compras.");
        if (parcela.CartaoId != CartaoId || parcela.Ciclo != Ciclo)
            throw new ArgumentException("A parcela pertence a outro cartão ou ciclo.", nameof(parcela));
        if (Parcelas.Any(existente => existente.Id == parcela.Id))
            throw new ArgumentException("A parcela já está na fatura.", nameof(parcela));
        return new FaturaCartao(Id, CartaoId, Ciclo, Parcelas.Append(parcela), Pagamentos);
    }

    public FaturaCartao ComPagamento(PagamentoFatura pagamento)
    {
        if (pagamento.FaturaId != Id)
            throw new ArgumentException("O pagamento pertence a outra fatura.", nameof(pagamento));
        if (pagamento.Valor > SaldoPendente)
            throw new ArgumentException("O pagamento ultrapassa o saldo pendente da fatura.", nameof(pagamento));
        if (Pagamentos.Any(existente => existente.Id == pagamento.Id || existente.TransacaoId == pagamento.TransacaoId))
            throw new ArgumentException("O pagamento já foi registrado.", nameof(pagamento));
        return new FaturaCartao(Id, CartaoId, Ciclo, Parcelas, Pagamentos.Append(pagamento));
    }

    public FaturaCartao SemRegistros(IReadOnlySet<Guid> parcelas, IReadOnlySet<Guid> pagamentos)
    {
        var restantes = Parcelas.Where(p => !parcelas.Contains(p.Id)).ToList();
        var confirmados = Pagamentos.Where(p => !pagamentos.Contains(p.Id)).ToList();
        if (confirmados.Sum(p => p.Valor) > restantes.Sum(p => p.Valor))
            throw new InvalidOperationException("Exclua os pagamentos antes de remover as compras desta fatura.");
        return new FaturaCartao(Id, CartaoId, Ciclo, restantes, confirmados);
    }
}
