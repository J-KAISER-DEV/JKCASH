using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Application.Excecoes;
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
            ImportanciaTransacao? importancia = null,
            PlanejamentoTransacao? planejamento = null,
            UrgenciaTransacao? urgencia = null,
            string? justificativa = null)
        {
            ArgumentNullException.ThrowIfNull(conta);
            if (!conta.Ativa)
            {
                throw new ConflitoFinanceiroException(
                    "A conta foi desativada. Atualize e selecione uma conta ativa. ");
            }
            if(!_categorias.Existe(categoriaId))
            {
                throw new ArgumentException(
                    "A categoria informada nao esta cadastrada",
                    nameof(categoriaId));
            }
            Transacao transacao = new(
                conta.Id,
                tipo,
                valor,
                descricao,
                data,
                categoriaId,
                importancia,
                planejamento,
                urgencia,
                justificativa);

            if (transacao.Tipo == TipoTransacao.Entrada)
            {
                conta.Creditar(transacao.Valor);
            }
            else
            {
                conta.Debitar(transacao.Valor);
            }
            try
            {
                _repositorio.Adicionar(transacao, conta);
            }
            catch
            {
                // Se não salvou, desfaz também a movimentação do objeto em memória.
                if (transacao.Tipo == TipoTransacao.Entrada)
                    conta.Debitar(transacao.Valor);
                else
                    conta.Creditar(transacao.Valor);

                throw;
            }
            return transacao;
        }
    }
}
