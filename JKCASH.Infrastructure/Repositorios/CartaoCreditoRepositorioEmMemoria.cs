using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class CartaoCreditoRepositorioEmMemoria : ICartaoCreditoRepositorio
    {
        private Dictionary<Guid, CartaoCredito> _cartoes = new();
        internal void Substituir(Dictionary<Guid, CartaoCredito> cartoes) => _cartoes = cartoes;

        public void Adicionar(CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(cartao);

            _cartoes.Add(cartao.Id, cartao);
        }

        public CartaoCredito? BuscarPorId(Guid id)
        {
            if (_cartoes.TryGetValue(id, out CartaoCredito? cartao))
            {
                return cartao;
            }
            return null;
        }

        public List<CartaoCredito> ListarTodos()
        {
            return _cartoes.Values.ToList();
        }
    }
}
