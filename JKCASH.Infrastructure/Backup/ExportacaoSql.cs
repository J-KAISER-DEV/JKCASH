using System.Data;
using System.Security.Cryptography;
using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Persistencia;
using JKCASH.Infrastructure.Persistencia.Conversores;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JKCASH.Infrastructure.Backup;

// Lê os registros existentes; não usa os serviços de movimentação nem grava na origem.
public sealed class ExportacaoSql(string configuracao)
{
    public async Task<BackupPacote> PrepararAsync(CancellationToken ct)
    {
        string caminho = Path.Combine(Path.GetTempPath(), $"jkcash-exportacao-{Guid.NewGuid():N}.db");
        try
        {
            var opcoes = new DbContextOptionsBuilder<JKCASHDbContext>().UseSqlite(
                new SqliteConnectionStringBuilder { DataSource = caminho, Pooling = false }.ToString()).Options;
            await using var local = new JKCASHDbContext(opcoes);
            await local.Database.MigrateAsync(ct);
            await local.Database.OpenConnectionAsync(ct);
            var destino = (SqliteConnection)local.Database.GetDbConnection();
            var modelo = local.Model.GetRelationalModel();
            string[] ordem = ["Contas", "Categorias", "CartoesCredito", "Transacoes",
                "ComprasCartao", "FaturasCartao", "ParcelasCartao", "PagamentosFatura"];

            var configuracaoSql = new SqlConnectionStringBuilder(configuracao) { ConnectTimeout = 5 };
            await using var origem = new SqlConnection(configuracaoSql.ToString());
            await origem.OpenAsync(ct);
            // Consulta e exportação das oito tabelas representam um único estado serializável.
            using var leitura = origem.BeginTransaction(IsolationLevel.Serializable);
            using var gravacao = destino.BeginTransaction();
            var conversorMoeda = new ConversorMoeda();

            foreach (string nome in ordem)
            {
                ct.ThrowIfCancellationRequested();
                var tabela = modelo.Tables.Single(t => t.Name == nome);
                var colunas = tabela.Columns.ToArray();
                // Os nomes vêm do modelo do aplicativo, nunca da requisição HTTP.
                using var consultar = origem.CreateCommand();
                consultar.Transaction = leitura;
                consultar.CommandTimeout = 20;
                consultar.CommandText = $"SELECT {string.Join(", ", colunas.Select(c => QuotarSql(c.Name)))} " +
                    $"FROM {QuotarSql(tabela.Schema ?? "dbo")}.{QuotarSql(nome)};";
                using var inserir = destino.CreateCommand();
                inserir.Transaction = gravacao;
                inserir.CommandText = $"INSERT INTO \"{nome}\" ({string.Join(", ", colunas.Select(c => $"\"{c.Name}\""))}) " +
                    $"VALUES ({string.Join(", ", colunas.Select((_, i) => "$p" + i))});";
                for (int i = 0; i < colunas.Length; i++) inserir.Parameters.Add(new SqliteParameter("$p" + i, DBNull.Value));
                await using var linhas = await consultar.ExecuteReaderAsync(ct);
                while (await linhas.ReadAsync(ct))
                {
                    ct.ThrowIfCancellationRequested();
                    for (int i = 0; i < colunas.Length; i++)
                    {
                        object valor = linhas.GetValue(i);
                        // SQL Server guarda decimal; SQLite guarda centavos INTEGER.
                        // As outras conversões (Guid, data, bool) são feitas pelo provedor SQLite.
                        if (valor is not DBNull && colunas[i].PropertyMappings.Any(m => m.Property.ClrType == typeof(decimal)))
                        {
                            if (valor is not decimal dinheiro)
                                throw new InvalidDataException($"Tipo monetário incompatível em {nome}.{colunas[i].Name}.");
                            valor = conversorMoeda.ConvertToProvider(dinheiro)!;
                        }
                        inserir.Parameters[i].Value = valor;
                    }
                    await inserir.ExecuteNonQueryAsync(ct);
                }
            }
            await gravacao.CommitAsync(ct);
            await leitura.CommitAsync(ct);
            await local.Database.CloseConnectionAsync();
            BancoBackup.PrepararTroca(caminho);
            BancoBackup.Conferir(caminho);
            byte[] banco = await File.ReadAllBytesAsync(caminho, ct);
            return new BackupPacote(new BackupResumo(Guid.NewGuid(), DateTimeOffset.UtcNow,
                $"SQL Server ({Environment.MachineName})", BancoBackup.VersaoFormato, banco.LongLength,
                Convert.ToHexString(SHA256.HashData(banco))), banco);
        }
        finally
        {
            foreach (string sufixo in new[] { "", "-wal", "-shm" })
                try { File.Delete(caminho + sufixo); }
                catch (IOException erro) { System.Diagnostics.Debug.WriteLine(erro); }
        }
    }

    private static string QuotarSql(string nome) => "[" + nome.Replace("]", "]]") + "]";
}
