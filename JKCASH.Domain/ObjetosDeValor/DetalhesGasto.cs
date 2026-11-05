using JKCASH.Domain.Entidades;

namespace JKCASH.Domain.ObjetosDeValor;

// Cópia histórica: mudanças no cadastro não alteram um lançamento confirmado.
public sealed class DetalhesGasto
{
    public Guid? CadastroId { get; private set; }
    public string Nome { get; private set; } = "";
    public NaturezaGasto Natureza { get; private set; }
    public NecessidadeGasto Necessidade { get; private set; }
    public FrequenciaGasto Frequencia { get; private set; }
    private DetalhesGasto() { }
    public DetalhesGasto(Gasto gasto, bool reutilizar)
    {
        CadastroId = reutilizar ? gasto.Id : null;
        Nome = gasto.Nome; Natureza = gasto.Natureza;
        Necessidade = gasto.Necessidade; Frequencia = gasto.Frequencia;
    }
}
