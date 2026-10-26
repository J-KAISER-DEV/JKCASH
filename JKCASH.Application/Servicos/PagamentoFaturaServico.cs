using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Application.Excecoes;
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
            ?? throw new ConflitoFinanceiroException("A fatura não está mais disponível. Atualize os dados.");
        CartaoCredito cartao = _cartoes.BuscarPorId(fatura.CartaoId)
            ?? throw new ConflitoFinanceiroException("O cartão não está mais disponível. Atualize os dados.");
        Conta conta = _contas.ListarTodas().SingleOrDefault(item => item.Id == contaId)
            ?? throw new ConflitoFinanceiroException("A conta não está mais disponível. Atualize os dados.");
        if (!conta.Ativa)
            throw new ConflitoFinanceiroException("A conta pagadora foi desativada. Atualize e selecione uma conta ativa.");
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

        bool limiteLiberado = false;
        conta.Debitar(valor);
        try
        {
            cartao.RegistrarPagamento(valor);
            limiteLiberado = true;
            _faturas.RegistrarPagamento(fatura, atualizada, saida, conta, cartao);
        }
        catch
        {
            if (limiteLiberado) cartao.RegistrarCompra(valor);
            conta.Creditar(valor);
            throw;
        }
        return pagamento;
    }
}
