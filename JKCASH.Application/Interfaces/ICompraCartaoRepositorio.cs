using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface ICompraCartaoRepositorio
    {
        void Adicionar(CompraCartao compra, CartaoCredito cartao);
    }
}
