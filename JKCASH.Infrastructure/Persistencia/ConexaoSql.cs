using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace JKCASH.Infrastructure.Persistencia
{
    public static class ConexaoSql
    {

        public static JKCASHDbContext CriarContexto()
        {
            var opcoes = new DbContextOptionsBuilder<JKCASHDbContext>()
                .UseSqlServer(Configuracao)
                .Options;

            return new JKCASHDbContext(opcoes);
        }

        private static string Configuracao =>
            "Server=localhost;" +
                "Database=JKCASHdb;" +
                "Integrated Security=True;" +
                "Encrypt=True;" +
                "TrustServerCertificate=True;";


        public static SqlConnection Criar()
        {
      
            return new SqlConnection(Configuracao);

        }
        // Entrega a conexão aberta; quem chama deve liberá-la com using.
        public static SqlConnection Abrir()
        {
            SqlConnection conexao = Criar();

            try
            {
                conexao.Open();
                return conexao;
            }
            catch
            {
                // Se Open falhar, não haverá um chamador para liberar o objeto.
                conexao.Dispose();
                throw;
            }
        }

        public static void Testar()
        {
            using SqlConnection conexao = Abrir();
        }
    }
}
