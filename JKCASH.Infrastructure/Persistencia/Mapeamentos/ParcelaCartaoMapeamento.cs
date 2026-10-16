using JKCASH.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKCASH.Infrastructure.Persistencia.Mapeamentos
{
    public class ParcelaCartaoMapeamento
        : IEntityTypeConfiguration<ParcelaCartao>
    {
        public void Configure(
            EntityTypeBuilder<ParcelaCartao> builder)
        {
            builder.ToTable("ParcelasCartao", "dbo");

            builder.HasKey(parcela => parcela.Id);

            builder.Property(parcela => parcela.Id)
                .ValueGeneratedNever();

            builder.Property(parcela => parcela.Valor)
                .HasPrecision(18, 2);

            builder.HasOne<CartaoCredito>()
                .WithMany()
                .HasForeignKey(parcela => parcela.CartaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(parcela => new
            {
                parcela.CompraId,
                parcela.Numero
            })
                .IsUnique()
                .HasDatabaseName("UQ_ParcelasCartao_Compra_Numero");

            builder.Ignore(parcela => parcela.Ciclo);
        }
    }
}