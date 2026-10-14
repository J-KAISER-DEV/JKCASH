using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Entidades
{

    public sealed class Transacao
    {
        public Guid Id { get; private set; }
        public Guid ContaId { get; private set; }
        public TipoTransacao Tipo {  get; private set; }
        public decimal Valor { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public DateTime Data {  get; private set; }
        public Guid CategoriaId { get; private set; }
        public ImportanciaTransacao Importancia { get; private set; }
        public PlanejamentoTransacao? Planejamento { get; private set; }
        public UrgenciaTransacao? Urgencia { get; private set; }
        public string Justificativa { get; private set; } = string.Empty;

        private Transacao() { }

        public Transacao(
        Guid contaId,
        TipoTransacao tipo,
        decimal valor,
        string descricao,
        DateTime data,
        Guid categoriaId,
        ImportanciaTransacao importancia,
        PlanejamentoTransacao planejamento,
        UrgenciaTransacao urgencia,
        string justificativa)
        {
            if (contaId == Guid.Empty)
            {
                throw new ArgumentNullException(
                    "Informe o identificador da conta. ",nameof(contaId));
            }

            if(!Enum.IsDefined(typeof(TipoTransacao),tipo))
            {
                throw new ArgumentException(
                    "Tipo de transação invalido. ", nameof(tipo));
            }

            if(valor <= 0m || valor != decimal.Round(valor,2))
            {
                throw new ArgumentException(
                    "O valor deve ser positivo e ter no maximo duas casa decimais. ",
                    nameof(valor));
            }
            if(string.IsNullOrWhiteSpace(descricao))
            {
                throw new ArgumentException(
                    "Informe a descrição", nameof (descricao));
            }
            if(categoriaId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Informe o identificador da categoria", nameof(categoriaId));
            }
            if(!Enum.IsDefined(typeof(ImportanciaTransacao),importancia))
            {
                throw new ArgumentException(
                    "Importancia invalida",nameof(importancia));
            }

            if (!Enum.IsDefined(typeof(PlanejamentoTransacao), planejamento))
            {
                throw new ArgumentException(
                    "Planejamento invalido", nameof(planejamento));
            }

            if (!Enum.IsDefined(typeof(UrgenciaTransacao), urgencia))
            {
                throw new ArgumentException(
                    "Urgencia invalida", nameof(urgencia));
            }

            if (string.IsNullOrWhiteSpace(justificativa))
            {
                throw new ArgumentException(
                    "informe a justificativa da transação",
                    nameof(justificativa));
            }
            Id = Guid.NewGuid();
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            Descricao = descricao;
            Data = data;
           CategoriaId = categoriaId;
            Importancia = importancia;
            Planejamento = planejamento;
            Urgencia = urgencia;
            Justificativa = justificativa;
        }
    }
}
