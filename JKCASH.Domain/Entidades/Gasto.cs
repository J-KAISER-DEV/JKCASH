using JKCASH.Domain.Enums;
using JKCASH.Domain.Validacoes;

namespace JKCASH.Domain.Entidades;

public enum NaturezaGasto { Produto = 1, Servico = 2, Assinatura = 3, Boleto = 4, Conta = 5 }
public enum NecessidadeGasto { Baixa = 1, Media = 2, Alta = 3 }
public enum FrequenciaGasto { Eventual = 1, Mensal = 2 }

// Cadastro reutilizável. Valor, data e pagamento pertencem ao lançamento.
public sealed class Gasto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = "";
    public Guid CategoriaId { get; private set; }
    public NaturezaGasto Natureza { get; private set; }
    public ImportanciaTransacao Importancia { get; private set; }
    public NecessidadeGasto Necessidade { get; private set; }
    public PlanejamentoTransacao Planejamento { get; private set; }
    public FrequenciaGasto Frequencia { get; private set; }
    public string Motivo { get; private set; } = "";

    private Gasto() { }

    public Gasto(string nome, Guid categoriaId, NaturezaGasto natureza,
        ImportanciaTransacao importancia, NecessidadeGasto necessidade,
        PlanejamentoTransacao planejamento, FrequenciaGasto frequencia, string? motivo = null,
        Guid? id = null)
    {
        Validacao.TextoObrigatorio(nome, nameof(nome), 200);
        Validacao.GuidNaoVazio(categoriaId, nameof(categoriaId));
        Validacao.EnumValido(natureza, nameof(natureza));
        Validacao.EnumValido(importancia, nameof(importancia));
        Validacao.EnumValido(necessidade, nameof(necessidade));
        Validacao.EnumValido(planejamento, nameof(planejamento));
        Validacao.EnumValido(frequencia, nameof(frequencia));
        motivo = motivo?.Trim() ?? "";
        if ((necessidade == NecessidadeGasto.Baixa || importancia == ImportanciaTransacao.Baixa)
            && string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Informe o motivo do gasto quando a necessidade ou a importância for baixa.", nameof(motivo));
        if (motivo.Length > 500) throw new ArgumentException("O motivo deve ter até 500 caracteres.", nameof(motivo));
        Id = id ?? Guid.NewGuid();
        Validacao.GuidNaoVazio(Id, nameof(id));
        Nome = nome.Trim(); CategoriaId = categoriaId; Natureza = natureza;
        Importancia = importancia; Necessidade = necessidade; Planejamento = planejamento;
        Frequencia = frequencia; Motivo = motivo;
    }
}
