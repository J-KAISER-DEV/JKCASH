using JKCASH.Application.Interfaces;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Infrastructure.Persistencia;
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

        public static IContaRepositorio CriarRepositorioContas(bool usarMemoria, ITransacaoRepositorio transacoes)
        {
            if (usarMemoria)
            {
                if (transacoes is not TransacaoRepositoEmMemoria transacoesEmMemoria)
                    throw new ArgumentException("Use o repositório de transações em memória neste modo.", nameof(transacoes));
                return new ContaRepositorioEmMemoria(transacoesEmMemoria);
            }

            return new ContaRepositorioEf(ConexaoSql.CriarContexto);
        }

        public static ICategoriaRepositorio CriarRepositorioCategorias(bool usarMemoria)
        {
            if (usarMemoria)
            {
                return new CategoriaRepositorioEmMemoria();
            }

            return new CategoriaRepositorioEf(ConexaoSql.CriarContexto);
        }

        public static ITransacaoRepositorio CriarRepositorioTransacoes(bool usarMemoria)
        {
            if (usarMemoria)
            {
                return new TransacaoRepositoEmMemoria();
            }

            return new TransacaoRepositorioEf(ConexaoSql.CriarContexto);
        }

        public static ICartaoCreditoRepositorio CriarRepositorioCartoes(bool usarMemoria)
        {
            if (usarMemoria)
                return new CartaoCreditoRepositorioEmMemoria();

            return new CartaoCreditoRepositorioEf(ConexaoSql.CriarContexto);
        }

        public static IExclusaoRepositorio CriarRepositorioExclusoes(bool usarMemoria,
            IContaRepositorio contas, ICategoriaRepositorio categorias, ITransacaoRepositorio transacoes,
            ICartaoCreditoRepositorio cartoes, ICompraCartaoRepositorio compras)
        {
            if (!usarMemoria) return new ExclusaoRepositorioEf(ConexaoSql.CriarContexto);
            return new ExclusaoRepositorioEmMemoria((ContaRepositorioEmMemoria)contas,
                (CategoriaRepositorioEmMemoria)categorias, (TransacaoRepositoEmMemoria)transacoes,
                (CartaoCreditoRepositorioEmMemoria)cartoes, (CompraCartaoRepositorio)compras);
        }

        public static ITransferenciaRepositorio CriarRepositorioTransferencias(
            bool usarMemoria, IContaRepositorio contas, ITransacaoRepositorio transacoes)
        {
            if (usarMemoria)
            {
                if (transacoes is not TransacaoRepositoEmMemoria transacoesEmMemoria)
                    throw new ArgumentException("Use o repositório de transações em memória neste modo.", nameof(transacoes));
                return new TransferenciaRepositorioEmMemoria(contas, transacoesEmMemoria);
            }

            return new TransferenciaRepositorioEf(ConexaoSql.CriarContexto);
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

            CompraCartaoRepositorioEf repositorioSql = new(ConexaoSql.CriarContexto);
            return (repositorioSql, repositorioSql);
        }
    }
}
