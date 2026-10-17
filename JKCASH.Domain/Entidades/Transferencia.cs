using JKCASH.Domain.Enums;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

// Os dois registros pertencem à mesma transferência e são criados juntos.
public sealed class Transferencia
{
    public Guid Id { get; }
    public Transacao Saida { get; }
    public Transacao Entrada { get; }
    public Guid OrigemId => Saida.ContaId;
    public Guid DestinoId => Entrada.ContaId;
    public decimal Valor => Saida.Valor;

    public Transferencia(Guid origemId, Guid destinoId, decimal valor,
        string descricao, DateTime data, Guid categoriaId,
        ImportanciaTransacao importancia, PlanejamentoTransacao planejamento,
        UrgenciaTransacao urgencia, string justificativa)
    {
        Validacao.GuidNaoVazio(origemId, nameof(origemId));
        Validacao.GuidNaoVazio(destinoId, nameof(destinoId));
        if (origemId == destinoId)
            throw new ArgumentException("As contas de origem e destino devem ser diferentes.");

        Id = Guid.NewGuid();
        Saida = Transacao.CriarTransferencia(Id, origemId, TipoTransacao.Saida,
            valor, descricao, data, categoriaId, importancia, planejamento, urgencia, justificativa);
        Entrada = Transacao.CriarTransferencia(Id, destinoId, TipoTransacao.Entrada,
            valor, descricao, data, categoriaId, importancia, planejamento, urgencia, justificativa);
    }
}
