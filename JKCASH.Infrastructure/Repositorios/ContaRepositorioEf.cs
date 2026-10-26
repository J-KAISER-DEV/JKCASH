using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JKCASH.Infrastructure.Repositorios
{
    public sealed class ContaRepositorioEf : IContaRepositorio
    {
        private readonly Func<JKCASHDbContext> _criarContexto;

        public ContaRepositorioEf(Func<JKCASHDbContext> criarContexto)
        {
            ArgumentNullException.ThrowIfNull(criarContexto);
            _criarContexto = criarContexto;
        }
        public void Remover(Guid id)
            => Remover(id, excluirTransacoes: false);

        public void RemoverComTransacoes(Guid id)
            => Remover(id, excluirTransacoes: true);

        private void Remover(Guid id, bool excluirTransacoes)
        {
            using JKCASHDbContext contexto = _criarContexto();
            using var operacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
            Conta conta = contexto.Contas.SingleOrDefault(conta => conta.Id == id)
                ?? throw new InvalidOperationException("Conta não encontrada.");

            if (contexto.CartoesCredito.Any(cartao => cartao.ContaId == id)
                || contexto.PagamentosFatura.Any(pagamento => pagamento.ContaId == id
                    || contexto.Transacoes.Any(t => t.Id == pagamento.TransacaoId && t.ContaId == id)))
                throw new InvalidOperationException(
                    "A conta possui cartões ou pagamentos de fatura vinculados e não pode ser excluída.");

            var transacoes = contexto.Transacoes.Where(transacao => transacao.ContaId == id).ToList();
            if (transacoes.Any(transacao => transacao.TransferenciaId != null))
                throw new InvalidOperationException(
                    "A conta participa de transferências. A exclusão apagaria parte do histórico de outra conta.");
            if (transacoes.Count > 0 && !excluirTransacoes)
                throw new InvalidOperationException("A conta possui transações. Confirme a exclusão do histórico também.");

            contexto.Transacoes.RemoveRange(transacoes);

            contexto.Contas.Remove(conta);
            try
            {
                // SaveChanges usa uma transação: os DELETEs são confirmados juntos.
                contexto.SaveChanges();
                operacao.Commit();
            }
            catch (DbUpdateException erro) when (erro.InnerException is SqliteException { SqliteExtendedErrorCode: 787 } or SqlException { Number: 547 })
            {
                // A chave estrangeira também protege contra vínculos criados durante a exclusão.
                throw new InvalidOperationException(
                    "A conta possui vínculos no banco e não pode ser excluída.", erro);
            }
            catch (DbUpdateConcurrencyException erro)
            {
                throw new InvalidOperationException("A conta já foi alterada ou excluída. Atualize a lista.", erro);
            }
        }

        public List<Conta> ListarTodas()
        {
            using JKCASHDbContext contexto = _criarContexto();

            return contexto.Contas
                .AsNoTracking()
                .OrderBy(conta => conta.Nome)
                .ThenBy(conta => conta.Id)
                .ToList();
        }

        public void Adicionar(Conta conta)
        {
            ArgumentNullException.ThrowIfNull(conta);

            using JKCASHDbContext contexto = _criarContexto();

            contexto.Contas.Add(conta);
            contexto.SaveChanges();
        }
    }
}
