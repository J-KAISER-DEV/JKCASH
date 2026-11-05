using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Consultas;

public sealed partial class RelatorioGastosConsulta : IRelatorioGastosConsulta
{
    private readonly Func<JKCASHDbContext> _criarContexto;

    public RelatorioGastosConsulta(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }

    public IReadOnlyList<GastoPorCategoria> ConsultarPorCategoria(
        IReadOnlyCollection<Guid> contaIds, DateTime dataInicial,
        DateTime dataFinal, FiltrosRelatorioGastos? filtros = null) =>
        Consultar(contaIds, dataInicial, dataFinal, filtros, comprasNoCartao: false);

    public IReadOnlyList<GastoPorCategoria> ConsultarComprasPorCategoria(
        IReadOnlyCollection<Guid> cartaoIds, DateTime dataInicial,
        DateTime dataFinal, FiltrosRelatorioGastos? filtros = null) =>
        Consultar(cartaoIds, dataInicial, dataFinal, filtros, comprasNoCartao: true);

    private IReadOnlyList<GastoPorCategoria> Consultar(
        IReadOnlyCollection<Guid> origens, DateTime dataInicial,
        DateTime dataFinal, FiltrosRelatorioGastos? filtros, bool comprasNoCartao)
    {
        ArgumentNullException.ThrowIfNull(origens);
        if (origens.Count == 0)
            throw new ArgumentException(comprasNoCartao
                ? "Selecione pelo menos um cartão."
                : "Selecione pelo menos uma conta.");

        DateTime inicio = dataInicial.Date;
        DateTime fim = dataFinal.Date;

        if (inicio > fim)
            throw new ArgumentException(
                "A data inicial não pode ser posterior à data final.");

        Guid[] ids = origens.Distinct().ToArray();
        using JKCASHDbContext contexto = _criarContexto();

        var despesas = comprasNoCartao
            ? contexto.ComprasCartao.AsNoTracking()
                .Where(c => ids.Contains(c.CartaoId))
                .Select(c => new
                {
                    c.Data, c.CategoriaId, c.Valor,
                    Importancia = (ImportanciaTransacao?)c.Importancia,
                    Urgencia = (UrgenciaTransacao?)c.Urgencia
                })
            : contexto.Transacoes.AsNoTracking()
                .Where(t => ids.Contains(t.ContaId) &&
                    t.Tipo == TipoTransacao.Saida && t.TransferenciaId == null)
                .Select(t => new { t.Data, t.CategoriaId, t.Valor, t.Importancia, t.Urgencia });

        despesas = despesas.Where(t => t.Data >= inicio);

        if (fim < DateTime.MaxValue.Date)
        {
            DateTime fimExclusivo = fim.AddDays(1);
            despesas = despesas.Where(t => t.Data < fimExclusivo);
        }

        if (filtros is not null)
        {
            Guid[] categorias = filtros.Categorias.Distinct().ToArray();
            ImportanciaTransacao[] importancias = filtros.Importancias.Distinct().ToArray();
            UrgenciaTransacao[] urgencias = filtros.Urgencias.Distinct().ToArray();

            if (categorias.Length > 0)
                despesas = despesas.Where(t => categorias.Contains(t.CategoriaId));

            if (importancias.Length > 0)
                despesas = despesas.Where(t => t.Importancia.HasValue &&
                    importancias.Contains(t.Importancia.Value));

            if (urgencias.Length > 0)
                despesas = despesas.Where(t => t.Urgencia.HasValue &&
                    urgencias.Contains(t.Urgencia.Value));
        }

        var grupos =
            from despesa in despesas
            join categoria in contexto.Categorias
                on despesa.CategoriaId equals categoria.Id into categorias
            from categoria in categorias.DefaultIfEmpty()
            group despesa by new
            {
                despesa.CategoriaId,
                Nome = categoria == null
                    ? "Categoria não encontrada"
                    : categoria.Nome
            };

        List<GastoPorCategoria> resultado;

        if (contexto.Database.IsSqlite())
        {
            resultado = grupos
                .Select(g => new
                {
                    g.Key.CategoriaId,
                    g.Key.Nome,
                    Quantidade = g.Count(),
                    Centavos = g.Sum(t => (long)t.Valor)
                })
                .ToList()
                .Select(g => new GastoPorCategoria(
                    g.CategoriaId, g.Nome, g.Quantidade, g.Centavos / 100m))
                .ToList();
        }
        else
        {
            resultado = grupos
                .Select(g => new GastoPorCategoria(
                    g.Key.CategoriaId, g.Key.Nome,
                    g.Count(), g.Sum(t => t.Valor)))
                .ToList();
        }

        return resultado.OrderByDescending(g => g.Total)
            .ThenBy(g => g.Categoria)
            .ToList();
    }
}
