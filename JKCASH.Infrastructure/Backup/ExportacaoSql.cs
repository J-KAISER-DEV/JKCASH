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
            string[] ordem = ["Contas", "Categorias", "Gastos", "CartoesCredito", "Transacoes",
                "ComprasCartao", "FaturasCartao", "ParcelasCartao", "PagamentosFatura"];

            var configuracaoSql = new SqlConnectionStringBuilder(configuracao) { ConnectTimeout = 5 };
            await using var origem = new SqlConnection(configuracaoSql.ToString());
            await origem.OpenAsync(ct);
            // Consulta e exportação das tabelas representam um único estado serializável.
            using var leitura = origem.BeginTransaction(IsolationLevel.Serializable);
            using var gravacao = destino.BeginTransaction();
            var conversorMoeda = new ConversorMoeda();

            foreach (string nome in ordem)
            {
                ct.ThrowIfCancellationRequested();
                var tabela = modelo.Tables.Single(t => t.Name == nome);
                var colunas = tabela.Columns.ToArray();
                // Aceita também a origem SQL anterior ao cadastro de gastos.
                using var esquema = origem.CreateCommand();
                esquema.Transaction = leitura;
                esquema.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tabela";
                esquema.Parameters.AddWithValue("@schema", tabela.Schema ?? "dbo");
                esquema.Parameters.AddWithValue("@tabela", nome);
                var existentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                await using (var leitorEsquema = await esquema.ExecuteReaderAsync(ct))
                    while (await leitorEsquema.ReadAsync(ct)) existentes.Add(leitorEsquema.GetString(0));
                if (nome == "Gastos" && existentes.Count == 0) continue;
                string Selecao(string coluna) => existentes.Contains(coluna) ? QuotarSql(coluna)
                    : coluna.StartsWith("Gasto", StringComparison.Ordinal) ? "NULL AS " + QuotarSql(coluna)
                    : throw new InvalidDataException($"Coluna obrigatória ausente: {nome}.{coluna}.");
                // Os nomes vêm do modelo do aplicativo, nunca da requisição HTTP.
                using var consultar = origem.CreateCommand();
                consultar.Transaction = leitura;
                consultar.CommandTimeout = 20;
                consultar.CommandText = $"SELECT {string.Join(", ", colunas.Select(c => Selecao(c.Name)))} " +
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
