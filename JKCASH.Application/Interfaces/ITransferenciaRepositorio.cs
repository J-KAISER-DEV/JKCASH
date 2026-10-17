using JKCASH.Domain.Entidades;

namespace JKCASH.Application.Interfaces;

public interface ITransferenciaRepositorio
{
    // Os dois saldos e os dois registros devem ser salvos juntos ou nada é salvo.
    void Registrar(Transferencia transferencia, Conta origem, Conta destino);
}
