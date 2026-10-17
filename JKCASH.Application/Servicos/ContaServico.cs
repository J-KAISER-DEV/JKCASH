using JKCASH.Application.Interfaces;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Application.Servicos
{
    public class ContaServico
    {
        private readonly IContaRepositorio _contas;
        private readonly ITransacaoRepositorio _transacoes;
        private readonly ICartaoCreditoRepositorio _cartoes;
        private readonly IFaturaCartaoRepositorio _faturas;

        public ContaServico(IContaRepositorio contas,
            ITransacaoRepositorio transacoes, ICartaoCreditoRepositorio cartoes,
            IFaturaCartaoRepositorio faturas)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(transacoes);
            ArgumentNullException.ThrowIfNull(cartoes);
            ArgumentNullException.ThrowIfNull(faturas);
            _contas = contas;
            _transacoes = transacoes;
            _cartoes = cartoes;
            _faturas = faturas;
        }

        public int ContarTransacoes(Guid contaId) => _transacoes.ListarPorConta(contaId).Count;

        public void Excluir(Guid contaId, bool excluirTransacoes = false)
        {
            Validacao.GuidNaoVazio(contaId, nameof(contaId));
            if (!_contas.ListarTodas().Any(conta => conta.Id == contaId))
                throw new InvalidOperationException("Conta não encontrada.");

            if (_transacoes.ListarPorConta(contaId).Any(t => t.TransferenciaId != null))
                throw new InvalidOperationException(
                    "A conta participa de transferências. A exclusão apagaria parte do histórico de outra conta.");

            if (!excluirTransacoes && ContarTransacoes(contaId) > 0)
                throw new InvalidOperationException(
                    "A conta possui transações e não pode ser excluída. O histórico será preservado.");

            if (_cartoes.ListarTodos().Any(cartao => cartao.ContaId == contaId))
                throw new InvalidOperationException(
                    "A conta possui cartões vinculados e não pode ser excluída.");

            // A conta pagadora pode ser diferente da conta do cartão.
            var idsTransacoes = _transacoes.ListarPorConta(contaId)
                .Select(transacao => transacao.Id).ToHashSet();
            bool possuiPagamentos = _cartoes.ListarTodos()
                .SelectMany(cartao => _faturas.ListarPorCartao(cartao.Id))
                .SelectMany(fatura => fatura.Pagamentos)
                .Any(pagamento => pagamento.ContaId == contaId || idsTransacoes.Contains(pagamento.TransacaoId));
            if (possuiPagamentos)
                throw new InvalidOperationException(
                    "A conta possui pagamentos de fatura vinculados e não pode ser excluída.");

            if (excluirTransacoes)
                _contas.RemoverComTransacoes(contaId);
            else
                _contas.Remover(contaId);
        }
    }
}
