using System.Data;
using JKCASH.Application.Excecoes;
using JKCASH.Domain.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Persistencia;

internal static class OperacaoFinanceira
{
    public static void Executar(JKCASHDbContext contexto, Action movimentar)
    {
        try
        {
            using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
            movimentar();
            contexto.SaveChanges();
            transacao.Commit();
        }
        catch (Exception erro) when (ConflitoDeGravacao(erro))
        {
            // Sem repetição automática: o usuário deve revisar o estado atual da operação.
            throw new ConflitoFinanceiroException(
                "Os dados estão sendo movimentados em outra operação. Atualize os valores e tente novamente.", erro);
        }
    }

    public static void ConferirConta(Conta persistida, Conta recebida, decimal saldoAnterior)
    {
        if (persistida.Id != recebida.Id || persistida.Saldo.SaldoAtual != saldoAnterior ||
            persistida.Ativa != recebida.Ativa ||
            persistida.Saldo.LimiteChequeEspecial != recebida.Saldo.LimiteChequeEspecial)
            throw new ConflitoFinanceiroException(
                "O saldo ou as condições da conta foram alterados. Consulte os dados atuais antes de confirmar novamente.");
    }

    private static bool ConflitoDeGravacao(Exception erro) => erro switch
    {
        DbUpdateConcurrencyException => true,
        SqlException sql => sql.Number is 1205 or 1222 or 3960,
        SqliteException sqlite => sqlite.SqliteErrorCode is 5 or 6,
        // O provedor SQL Server também pode encapsular deadlocks em InvalidOperationException.
        _ => erro.InnerException is Exception causa && ConflitoDeGravacao(causa)
    };
}
