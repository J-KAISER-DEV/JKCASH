using JKCASH.Application.Modelos;

namespace JKCASH.Application.Interfaces;

public interface IExclusaoRepositorio
{
    EstadoFinanceiro Carregar();
    void Salvar(EstadoFinanceiro atualizado, string versaoOriginal);
}
