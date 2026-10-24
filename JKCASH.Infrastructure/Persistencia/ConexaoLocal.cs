using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
namespace JKCASH.Infrastructure.Persistencia
{
    public static class ConexaoLocal
    {
        public static string PastaDados => Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "JKCASH");
        public static string CaminhoBanco => Path.Combine(PastaDados, "jkcash.db");

        public static JKCASHDbContext CriarContexto()
        {
            // Mantém os relacionamentos protegidos também no SQLite.
            var conexao = new SqliteConnectionStringBuilder
            {
                DataSource = CaminhoBanco,
                ForeignKeys = true
            };

            var opcoes = new DbContextOptionsBuilder<JKCASHDbContext>()
                .UseSqlite(conexao.ToString()).Options;
            return new JKCASHDbContext(opcoes);
        }

        public static void CriarPasta()
        {
            Directory.CreateDirectory(PastaDados);
        }

        public static void Inicializar()
        {
            CriarPasta();
            using JKCASHDbContext contexto = CriarContexto();
            // Aplica somente as migrações pendentes, preservando os registros existentes.
            contexto.Database.Migrate();
        }
    }
}
