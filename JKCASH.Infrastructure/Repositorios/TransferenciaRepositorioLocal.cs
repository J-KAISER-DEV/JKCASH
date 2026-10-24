using System.Data;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class TransferenciaRepositorioLocal : ITransferenciaRepositorio
{
    public void Registrar(Transferencia transferencia, Conta origem, Conta destino)
    {
        ArgumentNullException.ThrowIfNull(transferencia);
        ArgumentNullException.ThrowIfNull(origem);
        ArgumentNullException.ThrowIfNull(destino);
        if (transferencia.OrigemId != origem.Id || transferencia.DestinoId != destino.Id)
            throw new ArgumentException("As contas não correspondem às contas da transferência.");

        using var contexto = ConexaoLocal.CriarContexto();
        using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
        Guid[] ids = [origem.Id, destino.Id];
        var contas = contexto.Contas.Where(conta => ids.Contains(conta.Id))
            .OrderBy(conta => conta.Id).ToList();
        Conta origemSalva = contas.SingleOrDefault(conta => conta.Id == origem.Id)
            ?? throw new InvalidOperationException("Conta de origem não encontrada.");
        Conta destinoSalvo = contas.SingleOrDefault(conta => conta.Id == destino.Id)
            ?? throw new InvalidOperationException("Conta de destino não encontrada.");

        // O serviço já movimentou os objetos recebidos; o banco deve ter a versão anterior.
        if (origemSalva.Saldo.SaldoAtual != origem.Saldo.SaldoAtual + transferencia.Valor
            || destinoSalvo.Saldo.SaldoAtual != destino.Saldo.SaldoAtual - transferencia.Valor)
            throw new InvalidOperationException("Uma das contas foi movimentada. Consulte os saldos e tente novamente.");

        origemSalva.Debitar(transferencia.Valor);
        destinoSalvo.Creditar(transferencia.Valor);
        contexto.Transacoes.AddRange(transferencia.Saida, transferencia.Entrada);
        contexto.SaveChanges();
        transacao.Commit();
    }
}
