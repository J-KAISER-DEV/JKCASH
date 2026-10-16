using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;

namespace JKCASH.TestesManuais.Doubles
{
    public class CompraCartaoRepositorioComFalha : ICompraCartaoRepositorio
    {
        public CompraCartao? BuscarPorId(Guid id) => null;
        public List<CompraCartao> ListarPorCartao(Guid cartaoId) => new();
        public void Adicionar(
            CompraCartao compra,
            CartaoCredito cartao)
        {
            throw new InvalidOperationException(
                "Falha simulada ao guardar a compra.");
        }
    }
}
