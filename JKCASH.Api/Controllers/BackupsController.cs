using System.Security.Cryptography;
using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Backup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace JKCASH.Api.Controllers;

[ApiController]
[Route("api/backups")]
public sealed class BackupsController(BackupRepositorioSql repositorio, ILogger<BackupsController> log) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BackupResumo>>> Listar(CancellationToken ct)
    {
        try { return Ok(await repositorio.ListarAsync(ct)); }
        catch (SqlException erro) { return FalhaBanco(erro); }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BackupPacote>> Receber(Guid id, CancellationToken ct)
    {
        try
        {
            var pacote = await repositorio.BuscarAsync(id, ct);
            return pacote is null ? NotFound() : Ok(pacote);
        }
        catch (SqlException erro) { return FalhaBanco(erro); }
    }

    [HttpPut("{id:guid}")]
    [RequestSizeLimit(24_000_000)]
    public async Task<ActionResult<BackupResumo>> Enviar(Guid id, BackupEnvio envio, CancellationToken ct)
    {
        if (id == Guid.Empty || envio.Id != id || envio.VersaoFormato != BancoBackup.VersaoFormato ||
            string.IsNullOrWhiteSpace(envio.Maquina) || envio.Maquina.Length > 100 ||
            envio.Banco is null || envio.Banco.Length is < 100 or > BancoBackup.LimiteBytes ||
            !string.Equals(Convert.ToHexString(SHA256.HashData(envio.Banco)), envio.Sha256, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails { Detail = "Os dados ou a integridade do backup são inválidos." });

        string caminho = Path.Combine(Path.GetTempPath(), $"jkcash-validacao-{Guid.NewGuid():N}.db");
        try
        {
            await System.IO.File.WriteAllBytesAsync(caminho, envio.Banco, ct);
            await Task.Run(() => BancoBackup.Conferir(caminho), ct);
            return Ok(await repositorio.SalvarAsync(envio, ct));
        }
        catch (Exception erro) when (erro is InvalidDataException or SqliteException)
        {
            log.LogWarning(erro, "Backup inválido recebido: {Id}", id);
            return BadRequest(new ProblemDetails { Detail = "O arquivo não é um backup compatível e íntegro do JKCASH." });
        }
        catch (InvalidOperationException erro) { return Conflict(new ProblemDetails { Detail = erro.Message }); }
        catch (SqlException erro) { return FalhaBanco(erro); }
        finally { System.IO.File.Delete(caminho); }
    }

    private ObjectResult FalhaBanco(SqlException erro)
    {
        log.LogError(erro, "Não foi possível acessar o armazenamento de backups.");
        return Problem(statusCode: StatusCodes.Status503ServiceUnavailable,
            detail: "O armazenamento de backups está indisponível. Confira o SQL Server e tente novamente.");
    }
}
