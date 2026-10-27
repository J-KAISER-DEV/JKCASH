using JKCASH.Application.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace JKCASH.Api.Controllers;

[ApiController]
[Route("api/status")]
public sealed class EstadoController : ControllerBase
{
    // Verifica a API sem consultar contas, alterar dados ou iniciar um backup.
    [HttpGet]
    public ActionResult<EstadoApiResposta> Consultar() =>
        Ok(new EstadoApiResposta("JKCASH", "disponivel"));
}
