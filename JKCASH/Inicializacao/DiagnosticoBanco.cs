using JKCASH.Infrastructure.Persistencia;
using Microsoft.Data.SqlClient;

namespace JKCASH.Inicializacao
{
    public static class DiagnosticoBanco
    {
        public static void TestarConexao()
        {
            try
            {
                ConexaoSql.Testar();
                Console.WriteLine("Conexão com JKCASHDb realizada!");
            }
            catch (SqlException erro)
            {
                Console.WriteLine($"Falha na conexão: {erro.Message}");
            }
        }
    }
}
