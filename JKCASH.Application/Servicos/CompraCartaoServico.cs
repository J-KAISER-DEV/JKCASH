using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Servicos
{
    public class CompraCartaoServico
    {
        private readonly ICategoriaRepositorio _categorias;
        private readonly ICartaoCreditoRepositorio _repositorio;
        private readonly ICompraCartaoRepositorio _compras;

        public CompraCartaoServico(ICartaoCreditoRepositorio repositorio,
            ICategoriaRepositorio categorias,
            ICompraCartaoRepositorio compras)
        {
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(compras);

            _repositorio = repositorio;
            _categorias = categorias;
            _compras = compras;
        }

        public CompraCartao Registrar(
            Guid cartaoId,
            decimal valor,
            string descricao,
            DateTime data,
            Guid categoriaId,
            ImportanciaTransacao importancia,
            PlanejamentoTransacao planejamento,
            UrgenciaTransacao urgencia,
            string justificativa)
        {
            CartaoCredito? cartao =
                _repositorio.BuscarPorId(cartaoId);

            if (cartao is null)
            {
                throw new ArgumentException(
                    "O cartao informado nao esta cadastrado",
                    nameof(cartaoId));
            }

            if (!_categorias.Existe(categoriaId))
            {
                throw new ArgumentException(
                    "A categoria informada nao esta cadastrada.",
                    nameof(categoriaId));
            }

            CompraCartao compra = new(
                cartao.Id,
                valor,
                descricao,
                data,
                categoriaId,
                importancia,
                planejamento,
                urgencia,
                justificativa);

            cartao.RegistrarCompra(compra.Valor);
            try
            {
                _compras.Adicionar(compra, cartao);
            }
            catch
            {
                cartao.EstornarCompra(compra.Valor);
                throw;
            }
            return compra;
        }
    }
}
