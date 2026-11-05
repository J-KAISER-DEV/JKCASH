using JKCASH.Domain.Entidades;
using JKCASH.Domain.ObjetosDeValor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKCASH.Infrastructure.Persistencia.Mapeamentos;

internal static class GastoMapeamento
{
    public static void Configurar(ModelBuilder modelo)
    {
        var gasto = modelo.Entity<Gasto>();
        gasto.ToTable("Gastos", "dbo");
        gasto.HasKey(g => g.Id);
        gasto.Property(g => g.Id).ValueGeneratedNever();
        gasto.Property(g => g.Nome).HasMaxLength(200).IsRequired();
        gasto.Property(g => g.Motivo).HasMaxLength(500).IsRequired();
        gasto.HasOne<Categoria>().WithMany().HasForeignKey(g => g.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        modelo.Entity<Transacao>().Ignore(t => t.NovoCadastroGasto);
        modelo.Entity<CompraCartao>().Ignore(t => t.NovoCadastroGasto);
        modelo.Entity<Transacao>().OwnsOne(t => t.Gasto, MapearDetalhes);
        modelo.Entity<CompraCartao>().OwnsOne(t => t.Gasto, MapearDetalhes);
    }
    private static void MapearDetalhes<T>(OwnedNavigationBuilder<T, DetalhesGasto> detalhes) where T : class
    {
        detalhes.Property(g => g.Nome).HasColumnName("GastoNome").HasMaxLength(200).IsRequired();
        detalhes.Property(g => g.CadastroId).HasColumnName("GastoCadastroId");
        detalhes.Property(g => g.Natureza).HasColumnName("GastoNatureza");
        detalhes.Property(g => g.Necessidade).HasColumnName("GastoNecessidade");
        detalhes.Property(g => g.Frequencia).HasColumnName("GastoFrequencia");
        detalhes.HasOne<Gasto>().WithMany().HasForeignKey(g => g.CadastroId).OnDelete(DeleteBehavior.Restrict);
    }
}
