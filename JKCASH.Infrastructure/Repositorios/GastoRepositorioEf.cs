using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class GastoRepositorioEf(Func<JKCASHDbContext> criarContexto) : IGastoRepositorio
{
    public List<Gasto> ListarTodos()
    {
        using var contexto = criarContexto();
        return contexto.Gastos.AsNoTracking().OrderBy(g => g.Nome).ThenBy(g => g.Id).ToList();
    }
    public void Salvar(Gasto gasto)
    {
        using var contexto = criarContexto();
        if (contexto.Gastos.Any(g => g.Id == gasto.Id)) contexto.Gastos.Update(gasto);
        else contexto.Gastos.Add(gasto);
        contexto.SaveChanges();
    }
}
