using JKCASH.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKCASH.Infrastructure.Persistencia.Mapeamentos
{
    public class CompraCartaoMapeamento
        : IEntityTypeConfiguration<CompraCartao>
    {
        public void Configure(
            EntityTypeBuilder<CompraCartao> builder)
        {
            builder.ToTable("ComprasCartao", "dbo");

            builder.HasKey(compra => compra.Id);

            builder.Property(compra => compra.Id)
                .ValueGeneratedNever();

            builder.Property(compra => compra.Valor)
                .HasPrecision(18, 2);

            builder.Property(compra => compra.Descricao)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(compra => compra.Justificativa)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(compra => compra.Data)
                .HasColumnType("date");

            builder.HasOne<CartaoCredito>()
                .WithMany()
                .HasForeignKey(compra => compra.CartaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Categoria>()
                .WithMany()
                .HasForeignKey(compra => compra.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(compra => compra.Parcelas)
                .WithOne()
                .HasForeignKey(parcela => parcela.CompraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(compra => compra.Parcelas)
                .HasField("_parcelas")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}