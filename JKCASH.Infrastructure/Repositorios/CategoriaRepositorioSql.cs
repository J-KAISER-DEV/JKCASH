using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using JKCASH.Application.Interfaces;

namespace JKCASH.Infrastructure.Repositorios
{
    public class CategoriaRepositorioSql : ICategoriaRepositorio
    {
        public List<Categoria> ListarTodas()
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Categorias
                .AsNoTracking()
                .OrderBy(categoria => categoria.Nome)
                .ThenBy(categoria => categoria.Id)
                .ToList();
        }

        public bool Existe(Guid id)
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Categorias
                .Any(categoria => categoria.Id == id);
        }

        public void Adicionar(Categoria categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);

            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            contexto.Categorias.Add(categoria);

            contexto.SaveChanges();
        }

    }
}
