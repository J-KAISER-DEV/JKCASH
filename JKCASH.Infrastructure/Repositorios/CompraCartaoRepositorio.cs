using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class CompraCartaoRepositorio : ICompraCartaoRepositorio
    {
        private readonly Dictionary<Guid, CompraCartao> _compras = new();

        public void Adicionar(CompraCartao compra, CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(compra);
            ArgumentNullException.ThrowIfNull(cartao);

            if (compra.CartaoId != cartao.Id)
            {
                throw new ArgumentException(
                    "A compra pertence a outro cartão",
                    nameof(compra));
            }

            _compras.Add(compra.Id, compra);
        }
    }
}
