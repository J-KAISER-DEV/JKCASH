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
        private readonly TimeProvider _relogio;

        public CompraCartaoServico(ICartaoCreditoRepositorio repositorio,
            ICategoriaRepositorio categorias,
            ICompraCartaoRepositorio compras,
            TimeProvider? relogio = null)
        {
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(compras);

            _repositorio = repositorio;
            _categorias = categorias;
            _compras = compras;
            _relogio = relogio ?? TimeProvider.System;
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
            string justificativa,
            int quantidadeParcelas = 1)
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
                cartao,
                valor,
                descricao,
                data,
                categoriaId,
                importancia,
                planejamento,
                urgencia,
                justificativa,
                quantidadeParcelas);

            DateTime hoje = _relogio.GetLocalNow().Date;
            if (compra.Data > hoje)
                throw new ArgumentException("Não é possível registrar uma compra futura.", nameof(data));
            if (compra.Parcelas[0].Ciclo.Encerramento <= hoje)
                throw new ArgumentException("O ciclo dessa compra já fechou. Nesta versão, registre compras do ciclo aberto.", nameof(data));

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
