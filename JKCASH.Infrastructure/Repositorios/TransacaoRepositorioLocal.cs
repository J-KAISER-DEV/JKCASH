using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using JKCASH.Domain.Enums;
using System.Data;

namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositorioLocal : ITransacaoRepositorio
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

            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

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

            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            using var operacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
            Conta persistida = contexto.Contas.SingleOrDefault(c => c.Id == conta.Id)
                ?? throw new InvalidOperationException("Conta não encontrada.");

            // O serviço já movimentou o objeto recebido. O banco deve conter o saldo anterior.
            decimal saldoAnterior = transacao.Tipo == TipoTransacao.Entrada
                ? conta.Saldo.SaldoAtual - transacao.Valor
                : conta.Saldo.SaldoAtual + transacao.Valor;
            if (persistida.Saldo.SaldoAtual != saldoAnterior)
                throw new InvalidOperationException("A conta foi movimentada. Consulte o saldo e tente novamente.");

            if (transacao.Tipo == TipoTransacao.Entrada)
                persistida.Creditar(transacao.Valor);
            else
                persistida.Debitar(transacao.Valor);

            contexto.Transacoes.Add(transacao);

            // Um único SaveChanges salva ambos em uma transação do banco.
            // Se a gravação falhar, nenhuma das duas alterações é confirmada.
            contexto.SaveChanges();
            operacao.Commit();
        }

        public Transacao? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .SingleOrDefault(transacao => transacao.Id == id);
        }

        public List<Transacao> ListarTodas()
        {
            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }

        public List<Transacao> ListarPorConta(Guid contaId)
        {
            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            return contexto.Transacoes.AsNoTracking()
                .Where(transacao => transacao.ContaId == contaId)
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }
    }
}
