using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class CartaoCreditoRepositorioEf : ICartaoCreditoRepositorio
{
    // Guarda a função que criará um contexto para cada operação.
    private readonly Func<JKCASHDbContext> _criarContexto;

    public CartaoCreditoRepositorioEf(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }

    public void Adicionar(CartaoCredito cartao)
    {
        ArgumentNullException.ThrowIfNull(cartao);
        using JKCASHDbContext contexto = _criarContexto();
        contexto.CartoesCredito.Add(cartao);
        contexto.SaveChanges();
    }

    public CartaoCredito? BuscarPorId(Guid id)
    {
        using JKCASHDbContext contexto = _criarContexto();
        return contexto.CartoesCredito.AsNoTracking()
            .SingleOrDefault(cartao => cartao.Id == id);
    }

    public List<CartaoCredito> ListarTodos()
    {
        using JKCASHDbContext contexto = _criarContexto();
        return contexto.CartoesCredito.AsNoTracking()
            .OrderBy(cartao => cartao.Nome)
            .ThenBy(cartao => cartao.Id)
            .ToList();
    }
}
