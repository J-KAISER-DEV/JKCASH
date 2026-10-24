using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
namespace JKCASH.Infrastructure.Repositorios
{
    public class CartaoCreditoRepositorioLocal : ICartaoCreditoRepositorio
    {
        public void Adicionar(CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(cartao);

            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            contexto.CartoesCredito.Add(cartao);
            contexto.SaveChanges();
        }

        public CartaoCredito? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();

            return contexto.CartoesCredito
                .AsNoTracking()
                .SingleOrDefault(cartao => cartao.Id == id);

        }

        public List<CartaoCredito> ListarTodos()
        {
            using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();
            
            return contexto.CartoesCredito
                .AsNoTracking()
                .OrderBy(cartao => cartao.Nome)
                .ThenBy(cartao => cartao.Id)
                .ToList();
        }
    }
}
