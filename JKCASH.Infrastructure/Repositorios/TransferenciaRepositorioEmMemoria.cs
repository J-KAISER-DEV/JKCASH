using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class TransferenciaRepositorioEmMemoria : ITransferenciaRepositorio
{
    private readonly IContaRepositorio _contas;
    private readonly TransacaoRepositoEmMemoria _transacoes;

    public TransferenciaRepositorioEmMemoria(IContaRepositorio contas, TransacaoRepositoEmMemoria transacoes)
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(transacoes);
        _contas = contas;
        _transacoes = transacoes;
    }

    public void Registrar(Transferencia transferencia, Conta origem, Conta destino)
    {
        ArgumentNullException.ThrowIfNull(transferencia);
        ArgumentNullException.ThrowIfNull(origem);
        ArgumentNullException.ThrowIfNull(destino);
        var contas = _contas.ListarTodas();
        if (transferencia.OrigemId != origem.Id || transferencia.DestinoId != destino.Id
            || !contas.Any(conta => ReferenceEquals(conta, origem))
            || !contas.Any(conta => ReferenceEquals(conta, destino)))
            throw new ArgumentException("As contas não correspondem às contas da transferência nesta execução.");

        _transacoes.AdicionarTransferencia(transferencia);
    }
}
