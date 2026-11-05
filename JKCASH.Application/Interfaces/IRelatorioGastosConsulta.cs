using JKCASH.Application.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
namespace JKCASH.Application.Interfaces
{
    public interface IRelatorioGastosConsulta
    {
        IReadOnlyList<string> ListarNomesGastos();

        IReadOnlyList<ResumoGasto> ConsultarPorGasto(IReadOnlyCollection<Guid> contaIds,
            DateTime dataInicial, DateTime dataFinal, FiltrosRelatorioGastos? filtros = null);

        IReadOnlyList<ResumoGasto> ConsultarComprasPorGasto(IReadOnlyCollection<Guid> cartaoIds,
            DateTime dataInicial, DateTime dataFinal, FiltrosRelatorioGastos? filtros = null);

        IReadOnlyList<GastoPorCategoria> ConsultarPorCategoria(
        IReadOnlyCollection<Guid> contaIds,
        DateTime dataInicial,
        DateTime dataFinal, FiltrosRelatorioGastos? filtros = null);

        IReadOnlyList<GastoPorCategoria> ConsultarComprasPorCategoria(
            IReadOnlyCollection<Guid> cartaoIds,
            DateTime dataInicial,
            DateTime dataFinal, FiltrosRelatorioGastos? filtros = null);
    }
}
