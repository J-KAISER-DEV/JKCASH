using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using JKCASH.Application.Excecoes;
using JKCASH.Domain.Enums;

namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositorioSql : ITransacaoRepositorio
    {
        private readonly Func<JKCASHDbContext> _criarContexto;

        public TransacaoRepositorioSql(Func<JKCASHDbContext>? criarContexto = null)
        {
            _criarContexto = criarContexto ?? ConexaoSql.CriarContexto;
        }

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

            using JKCASHDbContext contexto = _criarContexto();

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

            using JKCASHDbContext contexto = _criarContexto();
            OperacaoFinanceira.Executar(contexto, () =>
            {
                Conta persistida = contexto.Contas.SingleOrDefault(c => c.Id == conta.Id)
                    ?? throw new ConflitoFinanceiroException("A conta não está mais disponível. Atualize os dados.");
                decimal saldoAnterior = transacao.Tipo == TipoTransacao.Entrada
                    ? conta.Saldo.SaldoAtual - transacao.Valor
                    : conta.Saldo.SaldoAtual + transacao.Valor;
                OperacaoFinanceira.ConferirConta(persistida, conta, saldoAnterior);

                if (transacao.Tipo == TipoTransacao.Entrada) persistida.Creditar(transacao.Valor);
                else persistida.Debitar(transacao.Valor);
                contexto.Transacoes.Add(transacao);
            });
        }

        public Transacao? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return contexto.Transacoes.AsNoTracking()
                .SingleOrDefault(transacao => transacao.Id == id);
        }

        public List<Transacao> ListarTodas()
        {
            using JKCASHDbContext contexto = _criarContexto();

            return contexto.Transacoes.AsNoTracking()
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }

        public List<Transacao> ListarPorConta(Guid contaId)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return contexto.Transacoes.AsNoTracking()
                .Where(transacao => transacao.ContaId == contaId)
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }
    }
}
