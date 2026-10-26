using JKCASH.Application.Modelos;

namespace JKCASH.Application.Interfaces;

public interface IDashboardConsulta
{
    DashboardResumo Consultar(Guid? contaId, DateTime mes);
}
