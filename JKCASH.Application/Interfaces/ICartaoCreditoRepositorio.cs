using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface ICartaoCreditoRepositorio
    {
        void Adicionar(CartaoCredito cartao);
        CartaoCredito? BuscarPorId(Guid id);
        List<CartaoCredito> ListarTodos();
    }
}
