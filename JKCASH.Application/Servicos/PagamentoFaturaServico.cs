using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Servicos;

public sealed class PagamentoFaturaServico
{
    private readonly IFaturaCartaoRepositorio _faturas;
    private readonly ICartaoCreditoRepositorio _cartoes;
    private readonly IContaRepositorio _contas;
    private readonly ICategoriaRepositorio _categorias;
    private readonly TimeProvider _relogio;

    public PagamentoFaturaServico(IFaturaCartaoRepositorio faturas, ICartaoCreditoRepositorio cartoes,
        IContaRepositorio contas, ICategoriaRepositorio categorias, TimeProvider? relogio = null)
    {
        ArgumentNullException.ThrowIfNull(faturas);
        ArgumentNullException.ThrowIfNull(cartoes);
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        _faturas = faturas;
        _cartoes = cartoes;
        _contas = contas;
        _categorias = categorias;
        _relogio = relogio ?? TimeProvider.System;
    }

    public PagamentoFatura Registrar(Guid faturaId, Guid contaId, decimal valor, Guid categoriaId)
    {
        FaturaCartao fatura = _faturas.BuscarPorId(faturaId)
            ?? throw new ArgumentException("Fatura não encontrada.", nameof(faturaId));
        CartaoCredito cartao = _cartoes.BuscarPorId(fatura.CartaoId)
            ?? throw new ArgumentException("Cartão não encontrado.");
        Conta conta = _contas.ListarTodas().SingleOrDefault(item => item.Id == contaId)
            ?? throw new ArgumentException("Conta não encontrada.", nameof(contaId));
        if (!_categorias.Existe(categoriaId))
            throw new ArgumentException("Categoria não cadastrada.", nameof(categoriaId));

        DateTime hoje = _relogio.GetLocalNow().Date;
        Transacao saida = new(conta.Id, TipoTransacao.Saida, valor,
            $"Pagamento de fatura {fatura.Ciclo.Vencimento:MM/yyyy}", hoje, categoriaId,
            ImportanciaTransacao.Essencial, PlanejamentoTransacao.Planejada,
            UrgenciaTransacao.SemUrgencia, "Pagamento confirmado da fatura do cartão de crédito.");
        PagamentoFatura pagamento = new(fatura.Id, conta.Id, saida.Id, valor, hoje);
        FaturaCartao atualizada = fatura.ComPagamento(pagamento);
        conta.ValidarSaida(valor);
        if (valor > cartao.ValorUtilizado)
            throw new InvalidOperationException("O pagamento ultrapassa o valor utilizado do cartão.");

        conta.Debitar(valor);
        cartao.RegistrarPagamento(valor);
        try
        {
            _faturas.RegistrarPagamento(fatura, atualizada, saida, conta, cartao);
        }
        catch
        {
            cartao.RegistrarCompra(valor);
            conta.Creditar(valor);
            throw;
        }
        return pagamento;
    }
}
