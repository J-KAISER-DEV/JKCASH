using System.Data;
using JKCASH.Application.Modelos;
using Microsoft.Data.SqlClient;

namespace JKCASH.Infrastructure.Backup;

public sealed class BackupRepositorioSql(string configuracao)
{
    private readonly string _configuracao = new SqlConnectionStringBuilder(configuracao)
        { ConnectTimeout = 5 }.ToString();
    private readonly SemaphoreSlim _preparo = new(1, 1);
    private bool _preparado;

    private async Task<SqlConnection> AbrirAsync(CancellationToken ct)
    {
        var conexao = new SqlConnection(_configuracao);
        try
        {
            await conexao.OpenAsync(ct);
            await _preparo.WaitAsync(ct);
            try
            {
                if (!_preparado)
                {
                    using var comando = conexao.CreateCommand();
                    comando.CommandText = """
                        IF OBJECT_ID(N'dbo.BackupsAplicacao', N'U') IS NULL
                        CREATE TABLE dbo.BackupsAplicacao (
                            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                            CriadoEmUtc DATETIMEOFFSET NOT NULL,
                            Maquina NVARCHAR(100) NOT NULL,
                            VersaoFormato INT NOT NULL,
                            TamanhoBytes BIGINT NOT NULL,
                            Sha256 CHAR(64) NOT NULL,
                            Banco VARBINARY(MAX) NOT NULL
                        );
                        """;
                    await comando.ExecuteNonQueryAsync(ct);
                    _preparado = true;
                }
            }
            finally { _preparo.Release(); }
            return conexao;
        }
        catch { await conexao.DisposeAsync(); throw; }
    }

    public async Task<List<BackupResumo>> ListarAsync(CancellationToken ct)
    {
        await using var conexao = await AbrirAsync(ct);
        using var comando = conexao.CreateCommand();
        comando.CommandText = """
            SELECT TOP (100) Id, CriadoEmUtc, Maquina, VersaoFormato, TamanhoBytes, Sha256
            FROM dbo.BackupsAplicacao ORDER BY CriadoEmUtc DESC, Id;
            """;
        using var leitor = await comando.ExecuteReaderAsync(ct);
        List<BackupResumo> itens = [];
        while (await leitor.ReadAsync(ct)) itens.Add(LerResumo(leitor));
        return itens;
    }

    public async Task<BackupPacote?> BuscarAsync(Guid id, CancellationToken ct)
    {
        await using var conexao = await AbrirAsync(ct);
        using var comando = conexao.CreateCommand();
        comando.CommandText = """
            SELECT Id, CriadoEmUtc, Maquina, VersaoFormato, TamanhoBytes, Sha256, Banco
            FROM dbo.BackupsAplicacao WHERE Id = @id;
            """;
        comando.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
        using var leitor = await comando.ExecuteReaderAsync(ct);
        return await leitor.ReadAsync(ct) ? new(LerResumo(leitor), (byte[])leitor[6]) : null;
    }

    public async Task<BackupResumo> SalvarAsync(BackupEnvio envio, CancellationToken ct)
    {
        await using var conexao = await AbrirAsync(ct);
        using var transacao = conexao.BeginTransaction(IsolationLevel.Serializable);
        using var existente = conexao.CreateCommand();
        existente.Transaction = transacao;
        existente.CommandText = """
            SELECT Id, CriadoEmUtc, Maquina, VersaoFormato, TamanhoBytes, Sha256
            FROM dbo.BackupsAplicacao WITH (UPDLOCK, HOLDLOCK) WHERE Id = @id;
            """;
        existente.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = envio.Id;
        BackupResumo? resumo;
        using (var leitor = await existente.ExecuteReaderAsync(ct))
            resumo = await leitor.ReadAsync(ct) ? LerResumo(leitor) : null;
        if (resumo is not null)
        {
            if (!string.Equals(resumo.Sha256, envio.Sha256, StringComparison.OrdinalIgnoreCase) ||
                resumo.VersaoFormato != envio.VersaoFormato || resumo.TamanhoBytes != envio.Banco.LongLength ||
                resumo.Maquina != envio.Maquina)
                throw new InvalidOperationException("O identificador já pertence a outro backup.");
        }
        else
        {
            resumo = new(envio.Id, DateTimeOffset.UtcNow, envio.Maquina, envio.VersaoFormato,
                envio.Banco.LongLength, envio.Sha256.ToUpperInvariant());
            using var inserir = conexao.CreateCommand();
            inserir.Transaction = transacao;
            inserir.CommandText = """
                INSERT INTO dbo.BackupsAplicacao
                (Id, CriadoEmUtc, Maquina, VersaoFormato, TamanhoBytes, Sha256, Banco)
                VALUES (@id, @data, @maquina, @versao, @tamanho, @hash, @banco);
                """;
            inserir.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = resumo.Id;
            inserir.Parameters.Add("@data", SqlDbType.DateTimeOffset).Value = resumo.CriadoEmUtc;
            inserir.Parameters.Add("@maquina", SqlDbType.NVarChar, 100).Value = resumo.Maquina;
            inserir.Parameters.Add("@versao", SqlDbType.Int).Value = resumo.VersaoFormato;
            inserir.Parameters.Add("@tamanho", SqlDbType.BigInt).Value = resumo.TamanhoBytes;
            inserir.Parameters.Add("@hash", SqlDbType.Char, 64).Value = resumo.Sha256;
            inserir.Parameters.Add("@banco", SqlDbType.VarBinary, -1).Value = envio.Banco;
            await inserir.ExecuteNonQueryAsync(ct);
        }
        await transacao.CommitAsync(ct);
        return resumo;
    }

    private static BackupResumo LerResumo(SqlDataReader leitor) => new(leitor.GetGuid(0),
        leitor.GetFieldValue<DateTimeOffset>(1), leitor.GetString(2), leitor.GetInt32(3),
        leitor.GetInt64(4), leitor.GetString(5));
}
