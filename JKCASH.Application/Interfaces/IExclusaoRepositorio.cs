using JKCASH.Application.Modelos;

namespace JKCASH.Application.Interfaces;

public interface IExclusaoRepositorio
{
    List<ItemExclusao> Listar(TipoRegistro tipo);
    EstadoFinanceiro Carregar(TipoRegistro tipo, Guid id);
    void Salvar(EstadoFinanceiro atualizado, PreviaExclusao previa);
}
