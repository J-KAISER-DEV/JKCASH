using JKCASH.Domain.Entidades;

namespace JKCASH.Application.Interfaces;

public interface IGastoRepositorio
{
    List<Gasto> ListarTodos();
    void Salvar(Gasto gasto);
}
