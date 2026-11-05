using JKCASH.Domain.ObjetosDeValor;
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
        public Guid? TransferenciaId { get; private set; }
        public TipoTransacao Tipo {  get; private set; }
        public decimal Valor { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public DateTime Data {  get; private set; }
        public Guid CategoriaId { get; private set; }
        public ImportanciaTransacao? Importancia { get; private set; }
        public PlanejamentoTransacao? Planejamento { get; private set; }
        public UrgenciaTransacao? Urgencia { get; private set; }
        public string? Justificativa { get; private set; }

        public DetalhesGasto? Gasto { get; private set; }
        public Gasto? NovoCadastroGasto { get; private set; }

        private Transacao() { }

        internal static Transacao CriarTransferencia(
            Guid transferenciaId, Guid contaId, TipoTransacao tipo, decimal valor,
            string descricao, DateTime data, Guid categoriaId,
            ImportanciaTransacao importancia, PlanejamentoTransacao planejamento,
            UrgenciaTransacao urgencia, string justificativa)
        {
            Validacao.GuidNaoVazio(transferenciaId, nameof(transferenciaId));
            Transacao transacao = new(contaId, tipo, valor, descricao, data,
                categoriaId, importancia, planejamento, urgencia, justificativa);
            // Transferências preservam a avaliação nos dois registros do par.
            ValidarAvaliacao(importancia, planejamento, urgencia, justificativa);
            transacao.Importancia = importancia;
            transacao.Planejamento = planejamento;
            transacao.Urgencia = urgencia;
            transacao.Justificativa = justificativa;
            transacao.TransferenciaId = transferenciaId;
            return transacao;
        }

        public Transacao(
        Guid contaId,
        TipoTransacao tipo,
        decimal valor,
        string descricao,
        DateTime data,
        Guid categoriaId,
        ImportanciaTransacao? importancia = null,
        PlanejamentoTransacao? planejamento = null,
        UrgenciaTransacao? urgencia = null,
        string? justificativa = null, Gasto? gasto = null, bool salvarGasto = false, bool novoGasto = false)
        {
            if (gasto is not null)
            {
                if (tipo != TipoTransacao.Saida) throw new ArgumentException("Gastos só podem ser vinculados a saídas.");
                categoriaId = gasto.CategoriaId; importancia = gasto.Importancia;
                planejamento = gasto.Planejamento; urgencia = UrgenciaTransacao.SemUrgencia;
                justificativa = gasto.Motivo;
                Gasto = new DetalhesGasto(gasto, salvarGasto);
                NovoCadastroGasto = salvarGasto && novoGasto ? gasto : null;
            }
            Validacao.GuidNaoVazio(contaId, nameof(contaId));
            Validacao.EnumValido(tipo, nameof(tipo));
            
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));
            Validacao.TextoObrigatorio(descricao, nameof(descricao),200);

            Validacao.GuidNaoVazio(categoriaId, nameof(categoriaId));
            if (tipo == TipoTransacao.Saida && gasto is null)
                ValidarAvaliacao(importancia, planejamento, urgencia, justificativa);
          
            Id = Guid.NewGuid();
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            Descricao = descricao;
            Data = data;
           CategoriaId = categoriaId;
            Importancia = tipo == TipoTransacao.Saida ? importancia : null;
            Planejamento = tipo == TipoTransacao.Saida ? planejamento : null;
            Urgencia = tipo == TipoTransacao.Saida ? urgencia : null;
            Justificativa = tipo == TipoTransacao.Saida ? justificativa : null;
        }

        private static void ValidarAvaliacao(ImportanciaTransacao? importancia,
            PlanejamentoTransacao? planejamento, UrgenciaTransacao? urgencia, string? justificativa)
        {
            if (!importancia.HasValue || !planejamento.HasValue || !urgencia.HasValue)
                throw new ArgumentException("Informe importância, planejamento e urgência da saída.");
            Validacao.EnumValido(importancia.Value, nameof(importancia));
            Validacao.EnumValido(planejamento.Value, nameof(planejamento));
            Validacao.EnumValido(urgencia.Value, nameof(urgencia));
            Validacao.TextoObrigatorio(justificativa ?? "", nameof(justificativa), 500);
        }
    }
}
