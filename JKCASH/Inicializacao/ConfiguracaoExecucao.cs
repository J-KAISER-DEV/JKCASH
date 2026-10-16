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
    }
}
