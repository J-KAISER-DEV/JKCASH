using JKCASH.Application.Excecoes;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class TransferenciaRepositorioEf : ITransferenciaRepositorio
{
    private readonly Func<JKCASHDbContext> _criarContexto;

    public TransferenciaRepositorioEf(Func<JKCASHDbContext> criarContexto)
    {
        _criarContexto = criarContexto ?? throw new ArgumentNullException(nameof(criarContexto));
    }
    public void Registrar(Transferencia transferencia, Conta origem, Conta destino)
    {
        ArgumentNullException.ThrowIfNull(transferencia);
        ArgumentNullException.ThrowIfNull(origem);
        ArgumentNullException.ThrowIfNull(destino);
        if (transferencia.OrigemId != origem.Id || transferencia.DestinoId != destino.Id)
            throw new ArgumentException("As contas não correspondem às contas da transferência.");

        using var contexto = _criarContexto();
        OperacaoFinanceira.Executar(contexto, () =>
        {
            Guid[] ids = [origem.Id, destino.Id];
            var contas = contexto.Contas.Where(conta => ids.Contains(conta.Id))
                .OrderBy(conta => conta.Id).ToList();
            Conta origemSalva = contas.SingleOrDefault(conta => conta.Id == origem.Id)
                ?? throw new ConflitoFinanceiroException("A conta de origem não está mais disponível. Atualize as contas.");
            Conta destinoSalvo = contas.SingleOrDefault(conta => conta.Id == destino.Id)
                ?? throw new ConflitoFinanceiroException("A conta de destino não está mais disponível. Atualize as contas.");

            OperacaoFinanceira.ConferirConta(origemSalva, origem, origem.Saldo.SaldoAtual + transferencia.Valor);
            OperacaoFinanceira.ConferirConta(destinoSalvo, destino, destino.Saldo.SaldoAtual - transferencia.Valor);

            origemSalva.Debitar(transferencia.Valor);
            destinoSalvo.Creditar(transferencia.Valor);
            contexto.Transacoes.AddRange(transferencia.Saida, transferencia.Entrada);
        });
    }
}
