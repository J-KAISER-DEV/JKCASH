using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;

namespace JKCASH.Infrastructure.Repositorios;

// Compras, parcelas e faturas compartilham a mesma unidade de trabalho em memória.
public class CompraCartaoRepositorio : ICompraCartaoRepositorio, IFaturaCartaoRepositorio
{
    private Dictionary<Guid, CompraCartao> _compras = new();
    private Dictionary<Guid, FaturaCartao> _faturas = new();
    private readonly ITransacaoRepositorio? _transacoes;
    private readonly TimeProvider _relogio;

    public CompraCartaoRepositorio(ITransacaoRepositorio? transacoes = null, TimeProvider? relogio = null)
    {
        _transacoes = transacoes;
        _relogio = relogio ?? TimeProvider.System;
    }

    public void Adicionar(CompraCartao compra, CartaoCredito cartao)
    {
        ArgumentNullException.ThrowIfNull(compra);
        ArgumentNullException.ThrowIfNull(cartao);
        if (compra.CartaoId != cartao.Id)
            throw new ArgumentException("A compra pertence a outro cartão.", nameof(compra));

        // Prepara todas as faturas em cópias antes de alterar o armazenamento.
        Dictionary<Guid, CompraCartao> compras = new(_compras);
        Dictionary<Guid, FaturaCartao> faturas = new(_faturas);
        compras.Add(compra.Id, compra);
        DateTime hoje = _relogio.GetLocalNow().Date;
        foreach (ParcelaCartao parcela in compra.Parcelas)
        {
            if (parcela.Ciclo.Encerramento <= hoje)
                throw new InvalidOperationException("A fatura já fechou e não recebe novas compras.");
            FaturaCartao? existente = faturas.Values.SingleOrDefault(fatura =>
                fatura.CartaoId == cartao.Id && fatura.Ciclo.Fechamento == parcela.Ciclo.Fechamento);
            FaturaCartao fatura = existente is null
                ? FaturaCartao.Criar(parcela) : existente.ComParcela(parcela, hoje);
            faturas[fatura.Id] = fatura;
        }

        _compras = compras;
        _faturas = faturas;
    }

    public CompraCartao? BuscarPorId(Guid id) => _compras.GetValueOrDefault(id);

    public List<CompraCartao> ListarPorCartao(Guid cartaoId) => _compras.Values
        .Where(compra => compra.CartaoId == cartaoId).OrderBy(compra => compra.Data).ToList();

    FaturaCartao? IFaturaCartaoRepositorio.BuscarPorId(Guid id) => _faturas.GetValueOrDefault(id);

    List<FaturaCartao> IFaturaCartaoRepositorio.ListarPorCartao(Guid cartaoId) => _faturas.Values
        .Where(fatura => fatura.CartaoId == cartaoId).OrderBy(fatura => fatura.Ciclo.Fechamento).ToList();

    public void RegistrarPagamento(FaturaCartao original, FaturaCartao atualizada,
        Transacao transacao, Conta conta, CartaoCredito cartao)
    {
        if (_transacoes is null)
            throw new InvalidOperationException("O repositório de saídas não foi configurado.");
        if (!ReferenceEquals(_faturas.GetValueOrDefault(original.Id), original))
            throw new InvalidOperationException("A fatura foi alterada. Consulte novamente antes de pagar.");

        PagamentoFatura pagamento = atualizada.Pagamentos.Last();
        if (atualizada.Id != original.Id || atualizada.CartaoId != cartao.Id ||
            atualizada.Pagamentos.Count != original.Pagamentos.Count + 1 ||
            pagamento.TransacaoId != transacao.Id || pagamento.ContaId != conta.Id ||
            transacao.ContaId != conta.Id || transacao.Valor != pagamento.Valor ||
            transacao.Tipo != JKCASH.Domain.Enums.TipoTransacao.Saida)
            throw new ArgumentException("Pagamento e saída da conta não correspondem à fatura.");

        Dictionary<Guid, FaturaCartao> faturas = new(_faturas) { [original.Id] = atualizada };
        // Se a saída falhar, o contrato do repositório exige que nada tenha sido salvo.
        _transacoes.Adicionar(transacao, conta);
        _faturas = faturas;
    }
}
