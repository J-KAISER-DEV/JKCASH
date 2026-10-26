using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class CategoriaRepositorioEf : ICategoriaRepositorio
{
    // Recebe uma função, não um contexto aberto. Cada método cria e descarta seu contexto.
    private readonly Func<JKCASHDbContext> _criarContexto;

    public CategoriaRepositorioEf(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }

    public void Adicionar(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);
        using JKCASHDbContext contexto = _criarContexto();
        contexto.Categorias.Add(categoria);
        contexto.SaveChanges();
    }

    public bool Existe(Guid id)
    {
        using JKCASHDbContext contexto = _criarContexto();
        return contexto.Categorias.Any(categoria => categoria.Id == id);
    }

    public List<Categoria> ListarTodas()
    {
        using JKCASHDbContext contexto = _criarContexto();
        return contexto.Categorias.AsNoTracking()
            .OrderBy(categoria => categoria.Nome)
            .ThenBy(categoria => categoria.Id)
            .ToList();
    }
}
