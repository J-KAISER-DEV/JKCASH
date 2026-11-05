using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Backup;

// A cópia nativa inclui todas as tabelas e os dados ainda presentes no WAL.
public static class BancoBackup
{
    public const int VersaoFormato = 1;
    public const int LimiteBytes = 16 * 1024 * 1024;

    public static SqliteConnection Abrir(string caminho, SqliteOpenMode modo)
    {
        var conexao = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = caminho, Mode = modo, Pooling = false,
            ForeignKeys = true, DefaultTimeout = 5
        }.ToString());
        try { conexao.Open(); return conexao; }
        catch { conexao.Dispose(); throw; }
    }

    public static void Copiar(string origem, string destino)
    {
        using var fonte = Abrir(origem, SqliteOpenMode.ReadOnly);
        using var copia = Abrir(destino, SqliteOpenMode.ReadWriteCreate);
        fonte.BackupDatabase(copia);
    }

    public static ConferenciaBackup Conferir(string caminho)
    {
        if (new FileInfo(caminho).Length is < 100 or > LimiteBytes)
            throw new InvalidDataException("O tamanho do banco de backup é inválido (limite: 16 MB).");

        using var conexao = Abrir(caminho, SqliteOpenMode.ReadOnly);
        using var integridade = conexao.CreateCommand();
        integridade.CommandText = "PRAGMA integrity_check;";
        if (!string.Equals(integridade.ExecuteScalar()?.ToString(), "ok", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("A integridade do backup não pôde ser confirmada.");
        using var vinculos = conexao.CreateCommand();
        vinculos.CommandText = "PRAGMA foreign_key_check;";
        using (var leitor = vinculos.ExecuteReader())
            if (leitor.Read()) throw new InvalidDataException("O backup contém vínculos inválidos.");

        var opcoes = new DbContextOptionsBuilder<JKCASHDbContext>().UseSqlite(conexao).Options;
        using var contexto = new JKCASHDbContext(opcoes);
        var conhecidas = contexto.Database.GetMigrations().ToArray();
        var aplicadas = contexto.Database.GetAppliedMigrations().ToArray();
        if (aplicadas.Length == 0 || !aplicadas.SequenceEqual(conhecidas.Take(aplicadas.Length)))
            throw new InvalidDataException("O backup usa uma versão de banco desconhecida. Atualize o aplicativo.");

        string[] tabelas = ["Contas", "Categorias", "Transacoes", "CartoesCredito",
            "ComprasCartao", "ParcelasCartao", "FaturasCartao", "PagamentosFatura"];
        // Backups antigos continuam aceitos antes da atualização da cópia.
        using var existeGastos = conexao.CreateCommand();
        existeGastos.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Gastos';";
        if ((long)existeGastos.ExecuteScalar()! > 0) tabelas = [.. tabelas, "Gastos"];
        Dictionary<string, long> quantidades = new();
        foreach (string tabela in tabelas)
        {
            using var consulta = conexao.CreateCommand();
            // Nomes fixos definidos acima, nunca recebidos da rede.
            consulta.CommandText = $"SELECT COUNT(*) FROM \"{tabela}\";";
            quantidades[tabela] = (long)consulta.ExecuteScalar()!;
        }
        return new ConferenciaBackup(quantidades);
    }

    public static void AtualizarCopia(string caminho)
    {
        Conferir(caminho);
        var opcoes = new DbContextOptionsBuilder<JKCASHDbContext>()
            .UseSqlite(new SqliteConnectionStringBuilder
            { DataSource = caminho, Pooling = false }.ToString()).Options;
        using (var contexto = new JKCASHDbContext(opcoes)) contexto.Database.Migrate();
        // A cópia que será instalada precisa ser autossuficiente, sem arquivos WAL.
        PrepararTroca(caminho);
        Conferir(caminho);
    }

    public static void PrepararTroca(string caminho)
    {
        using var conexao = Abrir(caminho, SqliteOpenMode.ReadWrite);
        using var checkpoint = conexao.CreateCommand();
        checkpoint.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
        using (var leitor = checkpoint.ExecuteReader())
            if (leitor.Read() && leitor.GetInt32(0) != 0)
                throw new IOException("O banco está em uso. Feche outras instâncias antes de restaurar.");
        using var modo = conexao.CreateCommand();
        modo.CommandText = "PRAGMA journal_mode=DELETE;";
        if (!string.Equals(modo.ExecuteScalar()?.ToString(), "delete", StringComparison.OrdinalIgnoreCase))
            throw new IOException("Não foi possível preparar o banco para a troca segura.");
    }
}
