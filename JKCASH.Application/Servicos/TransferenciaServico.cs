using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Application.Servicos;

public sealed class TransferenciaServico
{
    private readonly IContaRepositorio _contas;
    private readonly ICategoriaRepositorio _categorias;
    private readonly ITransferenciaRepositorio _transferencias;
    private readonly TimeProvider _relogio;

    public TransferenciaServico(IContaRepositorio contas, ICategoriaRepositorio categorias,
        ITransferenciaRepositorio transferencias, TimeProvider? relogio = null)
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(transferencias);
        _contas = contas;
        _categorias = categorias;
        _transferencias = transferencias;
        _relogio = relogio ?? TimeProvider.System;
    }

    public Transferencia Registrar(Guid origemId, Guid destinoId, decimal valor,
        Guid categoriaId, ImportanciaTransacao importancia,
        PlanejamentoTransacao planejamento, UrgenciaTransacao urgencia,
        string descricao, string justificativa)
    {
        Validacao.GuidNaoVazio(origemId, nameof(origemId));
        Validacao.GuidNaoVazio(destinoId, nameof(destinoId));
        if (origemId == destinoId)
            throw new ArgumentException("As contas de origem e destino devem ser diferentes.");

        var contas = _contas.ListarTodas();
        Conta origem = contas.SingleOrDefault(conta => conta.Id == origemId)
            ?? throw new ArgumentException("Conta de origem não encontrada.", nameof(origemId));
        Conta destino = contas.SingleOrDefault(conta => conta.Id == destinoId)
            ?? throw new ArgumentException("Conta de destino não encontrada.", nameof(destinoId));
        origem.ValidarSaida(valor);
        if (!destino.Ativa)
            throw new InvalidOperationException("A conta de destino está desativada.");
        if (!_categorias.Existe(categoriaId))
            throw new ArgumentException("Categoria não cadastrada.", nameof(categoriaId));

        Transferencia transferencia = new(origemId, destinoId, valor, descricao,
            _relogio.GetLocalNow().DateTime, categoriaId, importancia,
            planejamento, urgencia, justificativa);

        bool destinoCreditado = false;
        origem.Debitar(valor);
        try
        {
            destino.Creditar(valor);
            destinoCreditado = true;
            _transferencias.Registrar(transferencia, origem, destino);
        }
        catch
        {
            // Restaura também os objetos usados pela execução em memória.
            if (destinoCreditado) destino.Debitar(valor);
            origem.Creditar(valor);
            throw;
        }

        return transferencia;
    }
}
