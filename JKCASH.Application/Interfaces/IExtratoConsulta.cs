using JKCASH.Application.Modelos;

namespace JKCASH.Application.Interfaces;

public interface IExtratoConsulta
{
    ExtratoPagina Consultar(IReadOnlyCollection<Guid> contaIds, DateTime dataInicial,
        DateTime dataFinal, int pagina = 1, int tamanhoPagina = 50);

    ExtratoPagina ConsultarCompras(IReadOnlyCollection<Guid> cartaoIds, DateTime dataInicial,
        DateTime dataFinal, int pagina = 1, int tamanhoPagina = 50);
}
