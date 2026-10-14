using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios
{
    public class ContaRepositorioSql : IContaRepositorio
    {
        public List<Conta> ListarTodas()
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.Contas
                .AsNoTracking()
                .OrderBy(conta => conta.Nome)
                .ThenBy(conta => conta.Id)
                .ToList();
        }

        public void Adicionar(Conta conta)
        {
            ArgumentNullException.ThrowIfNull(conta);

            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            contexto.Contas.Add(conta);
            contexto.SaveChanges();
        }
    }
}
