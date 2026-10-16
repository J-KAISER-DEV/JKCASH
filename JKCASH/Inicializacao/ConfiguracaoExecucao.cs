using JKCASH.Application.Interfaces;
using JKCASH.Infrastructure.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Inicializacao
{
    public static class ConfiguracaoExecucao
    {
        public static bool UsarMemoria(string[] args)
        {
            return args.Contains("--memoria", StringComparer.OrdinalIgnoreCase);
        }

        public static IContaRepositorio CriarRepositorioContas(bool usarMemoria)
        {
            if (usarMemoria)
            {
                return new ContaRepositorioEmMemoria();
            }

            return new ContaRepositorioSql();
        }

        public static ICategoriaRepositorio CriarRepositorioCategorias(bool usarMemoria)
        {
            if (usarMemoria)
            {
                return new CategoriaRepositorioEmMemoria();
            }

            return new CategoriaRepositorioSql();
        }

        public static ITransacaoRepositorio CriarRepositorioTransacoes(bool usarMemoria)
        {
            if (usarMemoria)
            {
                return new TransacaoRepositoEmMemoria();
            }

            return new TransacaoRepositorioSql();
        }

        public static ICartaoCreditoRepositorio CriarRepositorioCartoes(bool usarMemoria)
        {
            if (usarMemoria)
                return new CartaoCreditoRepositorioEmMemoria();

            return new CartaoCreditoRepositorioSql();
        }

        public static (ICompraCartaoRepositorio Compras, IFaturaCartaoRepositorio Faturas)
            CriarRepositorioMovimentacoesCartao(bool usarMemoria, ITransacaoRepositorio transacoes)
        {
            ArgumentNullException.ThrowIfNull(transacoes);

            // As duas interfaces usam a mesma instância para compartilhar o armazenamento.
            if (usarMemoria)
            {
                CompraCartaoRepositorio repositorio = new(transacoes);
                return (repositorio, repositorio);
            }

            CompraCartaoRepositorioSql repositorioSql = new();
            return (repositorioSql, repositorioSql);
        }
    }
}
