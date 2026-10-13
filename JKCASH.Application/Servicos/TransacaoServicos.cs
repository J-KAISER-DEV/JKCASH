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
        private readonly ITransacaoRepositorio _repositorio;

        public TransacaoServico(ITransacaoRepositorio repositorio)
        {
            ArgumentNullException.ThrowIfNull(repositorio);

            _repositorio = repositorio;
        }

        public Transacao Registrar(
            Conta conta,
            TipoTransacao tipo,
            decimal valor,
            string descricao,
            DateTime data,
            CategoriaTransacao categoria)
        {
            ArgumentNullException.ThrowIfNull(conta);

            Transacao transacao = new(
                conta.Id,
                tipo,
                valor,
                descricao,
                data,
                categoria);

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