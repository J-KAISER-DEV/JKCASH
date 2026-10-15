using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
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
            Validacao.GuidNaoVazio(contaId, nameof(contaId));
            Validacao.EnumValido(tipo, nameof(tipo));
            
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));
            Validacao.TextoObrigatorio(descricao, nameof(descricao),200);

            Validacao.GuidNaoVazio(categoriaId, nameof(categoriaId));
            Validacao.EnumValido(importancia, nameof(importancia));
            Validacao.EnumValido(planejamento, nameof(planejamento));
            Validacao.EnumValido(urgencia, nameof(urgencia));

            Validacao.TextoObrigatorio(justificativa, nameof(justificativa),500);
          
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
