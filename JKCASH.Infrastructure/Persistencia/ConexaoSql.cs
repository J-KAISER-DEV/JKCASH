using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
namespace JKCASH.Infrastructure.Persistencia
{
    public static class ConexaoSql
    {
        public static void Testar()
        {
            string configuracao =
                "Server=localhost;" +
                "Database=JKCASHdb;" +
                "Integrated Security=True;" +
                "Encrypt=True;" +
                "TrustServerCertificate=True";

            using SqlConnection conexao = 
                new SqlConnection(configuracao);

            conexao.Open();
        }
    }
}
