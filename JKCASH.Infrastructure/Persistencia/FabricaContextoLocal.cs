using Microsoft.EntityFrameworkCore.Design;

namespace JKCASH.Infrastructure.Persistencia;

public class FabricaContextoLocal
    : IDesignTimeDbContextFactory<JKCASHDbContext>
{
    public JKCASHDbContext CreateDbContext(string[] args)
    {
        ConexaoLocal.CriarPasta();

        return ConexaoLocal.CriarContexto();
    }
}