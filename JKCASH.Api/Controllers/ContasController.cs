using JKCASH.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using JKCASH.Api.Modelos;
namespace JKCASH.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController : ControllerBase
    {
        private readonly IContaRepositorio _repositorio;

        public ContasController(IContaRepositorio repoitorio)
        {
            _repositorio = repoitorio;


        }
        [HttpGet]
        public IActionResult Listar()
        {
            var contas = _repositorio.ListarTodas();

            var resposta = contas.Select(conta => new ContaResposta
            {
                Id = conta.Id,
                Nome = conta.Nome,
                Banco = conta.Banco,
                Tipo = conta.Tipo,
                Destinacao = conta.Destinacao,
                SaldoInicial = conta.Saldo.SaldoInicial,
                SaldoAtual = conta.Saldo.SaldoAtual,
                LimiteChequeEspecial = conta.Saldo.LimiteChequeEspecial,
                Ativa = conta.Ativa
            }).ToList();
            return Ok(resposta);
        }

    }
}
   

    

