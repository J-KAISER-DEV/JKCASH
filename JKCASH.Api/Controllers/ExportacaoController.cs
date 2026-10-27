using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Backup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace JKCASH.Api.Controllers;

[ApiController]
[Route("api/exportacao/sqlserver")]
public sealed class ExportacaoController(ExportacaoSql exportacao, ILogger<ExportacaoController> log) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BackupPacote>> Receber(CancellationToken ct)
    {
        using var prazo = CancellationTokenSource.CreateLinkedTokenSource(ct);
        prazo.CancelAfter(TimeSpan.FromSeconds(50));
        try { return Ok(await exportacao.PrepararAsync(prazo.Token)); }
        catch (SqlException erro)
        {
            log.LogError(erro, "Falha ao consultar as tabelas existentes do SQL Server.");
            return Problem(statusCode: 503, detail:
                "Não foi possível ler os dados do SQL Server. Confira a conexão, as tabelas e as colunas da versão atual.");
        }
        catch (Exception erro) when (erro is InvalidDataException or SqliteException or ArgumentException or OverflowException)
        {
            log.LogError(erro, "Os dados SQL não puderam ser convertidos em uma cópia local válida.");
            return Problem(statusCode: 422, detail:
                "A origem contém dados incompatíveis ou vínculos inválidos. Nada foi importado; confira o log da API.");
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Problem(statusCode: 504, detail: "A preparação dos dados excedeu o tempo de espera. Tente novamente.");
        }
    }
}
