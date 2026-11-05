using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using JKCASH.Domain.Enums;
using JKCASH.Application.Excecoes;

namespace JKCASH.Infrastructure.Repositorios
{
    public sealed class TransacaoRepositorioEf : ITransacaoRepositorio
    {
        private readonly Func<JKCASHDbContext> _criarContexto;

        public TransacaoRepositorioEf(Func<JKCASHDbContext> criarContexto)
        {
            _criarContexto = criarContexto ?? throw new ArgumentNullException(nameof(criarContexto));
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
            DateTime inicio = dataInicial.Date;
            var consulta = contexto.Transacoes
                .AsNoTracking()
                .Where(transacao =>
                    contaIds.Contains(transacao.ContaId) &&
                    transacao.Data >= inicio);
            if (dataFinal.Date < DateTime.MaxValue.Date)
            {
                DateTime fimExclusivo = dataFinal.Date.AddDays(1);
                consulta = consulta.Where(transacao => transacao.Data < fimExclusivo);
            }
            return consulta
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
                if (transacao.NovoCadastroGasto is Gasto cadastro) contexto.Gastos.Add(cadastro);
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
