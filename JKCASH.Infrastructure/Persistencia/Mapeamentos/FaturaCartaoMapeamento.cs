using JKCASH.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKCASH.Infrastructure.Persistencia.Mapeamentos
{
    public class FaturaCartaoMapeamento
        : IEntityTypeConfiguration<FaturaCartao>
    {
        public void Configure(
            EntityTypeBuilder<FaturaCartao> builder)
        {
            builder.ToTable("FaturasCartao", "dbo");

            builder.HasKey(fatura => fatura.Id);

            builder.Property(fatura => fatura.Id)
                .ValueGeneratedNever();

            builder.HasOne<CartaoCredito>()
                .WithMany()
                .HasForeignKey(fatura => fatura.CartaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(fatura => fatura.Ciclo, ciclo =>
            {
                ciclo.Property(c => c.Fechamento)
                    .HasColumnName("Fechamento")
                    .HasColumnType("date");

                ciclo.Property(c => c.Vencimento)
                    .HasColumnName("Vencimento")
                    .HasColumnType("date");

                ciclo.Property(c => c.Encerramento)
                    .HasColumnName("Encerramento")
                    .HasColumnType("date");
            });

            builder.Navigation(fatura => fatura.Ciclo)
                .IsRequired();

            builder.HasMany(fatura => fatura.Parcelas)
                .WithOne(parcela => parcela.Fatura)
                .HasForeignKey(parcela => parcela.FaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(fatura => fatura.Parcelas)
                .HasField("_parcelas")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(fatura => fatura.Pagamentos)
                .WithOne()
                .HasForeignKey(pagamento => pagamento.FaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(fatura => fatura.Pagamentos)
                .HasField("_pagamentos")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(fatura => fatura.Total);
            builder.Ignore(fatura => fatura.TotalPago);
            builder.Ignore(fatura => fatura.SaldoPendente);
            builder.Ignore(fatura => fatura.SituacaoPagamento);
        }
    }
}