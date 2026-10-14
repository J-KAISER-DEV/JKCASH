using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
namespace JKCASH.Infrastructure.Persistencia
{
    public class JKCASHDbContext : DbContext
    {
        public JKCASHDbContext(
            DbContextOptions<JKCASHDbContext> opcoes )
            : base(opcoes ) { }

        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Conta> Contas => Set<Conta>();
        public DbSet<Transacao> Transacoes => Set<Transacao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>()
                .ToTable("Categorias", "dbo");

            modelBuilder.Entity<Categoria>()
                .HasKey(categoria => categoria.Id);

            modelBuilder.Entity<Categoria>()
                .Property(categoria => categoria.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Categoria>()
                .Property(categoria => categoria.Nome)
                .HasMaxLength(100)
                .IsRequired();

            var conta = modelBuilder.Entity<Conta>();
            conta.ToTable("Contas", "dbo");
            conta.HasKey(c => c.Id);
            conta.Property(c => c.Id).ValueGeneratedNever();
            conta.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            conta.Property(c => c.Banco).HasMaxLength(100).IsRequired();

            // Saldo pertence à conta. Seus valores ficam na mesma tabela Contas.
            conta.OwnsOne(c => c.Saldo, saldo =>
            {
                saldo.Property(s => s.SaldoInicial)
                    .HasColumnName("SaldoInicial").HasPrecision(18, 2);
                saldo.Property(s => s.SaldoAtual)
                    .HasColumnName("SaldoAtual").HasPrecision(18, 2);
                saldo.Property(s => s.LimiteChequeEspecial)
                    .HasColumnName("LimiteChequeEspecial").HasPrecision(18, 2);

                // Estes valores são calculados pelo C#, não são colunas.
                saldo.Ignore(s => s.ChequeEspecialUtilizado);
                saldo.Ignore(s => s.ChequeEspecialDisponivel);
                saldo.Ignore(s => s.DisponivelComChequeEspecial);
            });
            conta.Navigation(c => c.Saldo).IsRequired();

            var transacao = modelBuilder.Entity<Transacao>();
            transacao.ToTable("Transacoes", "dbo");
            transacao.HasKey(t => t.Id);
            transacao.Property(t => t.Id).ValueGeneratedNever();
            transacao.Property(t => t.Valor).HasPrecision(18, 2);
            transacao.Property(t => t.Descricao).HasMaxLength(200).IsRequired();
            transacao.Property(t => t.Justificativa).HasMaxLength(500).IsRequired();
            transacao.Property(t => t.Data).HasColumnType("datetime2");
            transacao.HasOne<Conta>().WithMany()
                .HasForeignKey(t => t.ContaId).OnDelete(DeleteBehavior.Restrict);
            transacao.HasOne<Categoria>().WithMany()
                .HasForeignKey(t => t.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
