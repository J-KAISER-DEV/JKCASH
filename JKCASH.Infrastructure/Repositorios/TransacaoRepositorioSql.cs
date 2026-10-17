using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositorioSql : ITransacaoRepositorio
    {
        public List<Transacao> ListarPorPeriodo(
    List<Guid> contaIds,
    DateTime dataInicial,
    DateTime dataFinal)
        {
            ArgumentNullException.ThrowIfNull(contaIds);

            if (dataInicial.Date > dataFinal.Date)
            {
                throw new ArgumentException(
                    "A data inicial não pode ser posterior à data final.");
            }

            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Transacoes
                .AsNoTracking()
                .Where(transacao =>
                    contaIds.Contains(transacao.ContaId) &&
                    transacao.Data.Date >= dataInicial.Date &&
                    transacao.Data.Date <= dataFinal.Date)
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }
        public void Adicionar(Transacao transacao, Conta conta)
        {
            ArgumentNullException.ThrowIfNull(transacao);
            ArgumentNullException.ThrowIfNull(conta);

            if (transacao.ContaId != conta.Id)
                throw new ArgumentException("A transação pertence a outra conta.");

            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            // A conta veio de uma consulta anterior, realizada por outro contexto.
            contexto.Contas.Attach(conta);
            contexto.Entry(conta.Saldo)
                .Property(saldo => saldo.SaldoAtual).IsModified = true;
            contexto.Transacoes.Add(transacao);

            // Um único SaveChanges salva ambos em uma transação do banco.
            // Se a gravação falhar, nenhuma das duas alterações é confirmada.
            contexto.SaveChanges();
        }

        public Transacao? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .SingleOrDefault(transacao => transacao.Id == id);
        }

        public List<Transacao> ListarTodas()
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }

        public List<Transacao> ListarPorConta(Guid contaId)
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .Where(transacao => transacao.ContaId == contaId)
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }
    }
}
