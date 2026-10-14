using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Servicos
{
    public class TransacaoServico
    {
        private readonly ICategoriaRepositorio _categorias;
        private readonly ITransacaoRepositorio _repositorio;

        public TransacaoServico(ITransacaoRepositorio repositorio,
            ICategoriaRepositorio categorias)
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(categorias);

            _repositorio = repositorio;
            _categorias = categorias;
        }

        public Transacao Registrar(
            Conta conta,
            TipoTransacao tipo,
            decimal valor,
            string descricao,
            DateTime data,
            Guid categoriaId,
            ImportanciaTransacao importancia,
            string justificativa)
        {
            if(!_categorias.Existe(categoriaId))
            {
                throw new ArgumentException(
                    "A categoria informada nao esta cadastrada",
                    nameof(categoriaId));
            }
            ArgumentNullException.ThrowIfNull(conta);

            Transacao transacao = new(
                conta.Id,
                tipo,
                valor,
                descricao,
                data,
                categoriaId,
                importancia,
                justificativa);

            if (transacao.Tipo == TipoTransacao.Entrada)
            {
                conta.Creditar(transacao.Valor);
            }
            else
            {
                conta.Debitar(transacao.Valor);
            }
            _repositorio.Adicionar(transacao);
            return transacao;
        }
    }
}
