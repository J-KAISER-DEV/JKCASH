using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
namespace JKCASH.Infrastructure.Repositorios
{
    public class CartaoCreditoRepositorioSql : ICartaoCreditoRepositorio
    {
        public void Adicionar(CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(cartao);

            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            contexto.CartoesCredito.Add(cartao);
            contexto.SaveChanges();
        }

        public CartaoCredito? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();

            return contexto.CartoesCredito
                .AsNoTracking()
                .SingleOrDefault(cartao => cartao.Id == id);

        }

        public List<CartaoCredito> ListarTodos()
        {
            using JKCASHDbContext contexto = ConexaoSql.CriarContexto();
            
            return contexto.CartoesCredito
                .AsNoTracking()
                .OrderBy(cartao => cartao.Nome)
                .ThenBy(cartao => cartao.Id)
                .ToList();
        }
    }
}
