using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public class CategoriaRepositorioLocal : ICategoriaRepositorio
{
    public void Adicionar(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

        contexto.Categorias.Add(categoria);

        contexto.SaveChanges();
    }

    public bool Existe(Guid id)
    {
        using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

        return contexto.Categorias
            .Any(categoria => categoria.Id == id);
    }

    public List<Categoria> ListarTodas()
    {
        using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

        return contexto.Categorias
            .AsNoTracking()
            .OrderBy(categoria => categoria.Nome)
            .ThenBy(categoria => categoria.Id)
            .ToList();
    }
}