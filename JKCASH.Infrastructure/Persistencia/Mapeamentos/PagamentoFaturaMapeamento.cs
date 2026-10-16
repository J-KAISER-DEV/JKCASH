using JKCASH.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKCASH.Infrastructure.Persistencia.Mapeamentos
{
    public class PagamentoFaturaMapeamento
        : IEntityTypeConfiguration<PagamentoFatura>
    {
        public void Configure(
            EntityTypeBuilder<PagamentoFatura> builder)
        {
            builder.ToTable("PagamentosFatura", "dbo");

            builder.HasKey(pagamento => pagamento.Id);

            builder.Property(pagamento => pagamento.Id)
                .ValueGeneratedNever();

            builder.Property(pagamento => pagamento.Valor)
                .HasPrecision(18, 2);

            builder.Property(pagamento => pagamento.Data)
                .HasColumnType("date");

            builder.HasOne<Conta>()
                .WithMany()
                .HasForeignKey(pagamento => pagamento.ContaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Transacao>()
                .WithOne()
                .HasForeignKey<PagamentoFatura>(
                    pagamento => pagamento.TransacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pagamento => pagamento.TransacaoId)
                .IsUnique()
                .HasDatabaseName("UQ_PagamentosFatura_Transacao");
        }
    }
}