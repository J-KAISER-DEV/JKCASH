using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
using JKCASH.Domain.ObjetosDeValor;
namespace JKCASH.Domain.Entidades
{
    public sealed class CompraCartao
    {
        public Guid Id { get; private set; }
        public Guid CartaoId { get; private set; }
        public decimal Valor { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public DateTime Data {  get; private set; }
        public Guid CategoriaId { get; private set; }
        public ImportanciaTransacao Importancia {  get; private set; }
        public PlanejamentoTransacao Planejamento { get; private set; }
        public UrgenciaTransacao Urgencia { get; private set; }
        public string Justificativa { get; private set; } = string.Empty;
        public int QuantidadeParcelas { get; private set; }
        public IReadOnlyList<ParcelaCartao> Parcelas =>
            _parcelas.AsReadOnly();
        private readonly List<ParcelaCartao> _parcelas = new();
        public DetalhesGasto? Gasto { get; private set; }
        public Gasto? NovoCadastroGasto { get; private set; }
        private CompraCartao() { }
        
        public CompraCartao(
            CartaoCredito cartao,
            decimal valor,
            string descricao,
            DateTime data,
            Guid categoriaId,
            ImportanciaTransacao importancia,
            PlanejamentoTransacao planejamento,
            UrgenciaTransacao urgencia,
            string justificativa,
            int quantidadeParcelas = 1, Gasto? gasto = null, bool salvarGasto = false, bool novoGasto = false)
        {
            if (gasto is not null)
            {
                categoriaId = gasto.CategoriaId; importancia = gasto.Importancia;
                planejamento = gasto.Planejamento; urgencia = UrgenciaTransacao.SemUrgencia;
                justificativa = gasto.Motivo;
                Gasto = new DetalhesGasto(gasto, salvarGasto);
                NovoCadastroGasto = salvarGasto && novoGasto ? gasto : null;
            }
            ArgumentNullException.ThrowIfNull(cartao);
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));
            Validacao.TextoObrigatorio(descricao, nameof(descricao),200);
            Validacao.GuidNaoVazio(categoriaId, nameof(categoriaId));
            Validacao.EnumValido(importancia, nameof(importancia));
            Validacao.EnumValido(planejamento, nameof(planejamento));
            Validacao.EnumValido(urgencia, nameof(urgencia));
            if (gasto is null) Validacao.TextoObrigatorio(justificativa, nameof(justificativa),500);
            // Até 120 parcelas, sempre com valor de pelo menos um centavo.
            Validacao.InteiroNoIntervalo(quantidadeParcelas, 1, 120, nameof(quantidadeParcelas));
            if (valor < quantidadeParcelas * 0.01m)
                throw new ArgumentException("Cada parcela deve ter pelo menos um centavo.", nameof(quantidadeParcelas));
            Id = Guid.NewGuid();
            CartaoId = cartao.Id;
            Valor = valor;
            Descricao = descricao;
            Data = data.Date;
            CategoriaId = categoriaId;
            Importancia = importancia;
            Planejamento = planejamento;
            Urgencia = urgencia;
            Justificativa = justificativa;
            QuantidadeParcelas = quantidadeParcelas;

            CicloFatura primeiroCiclo = CicloFatura.DaCompra(cartao, Data);
            List<ParcelaCartao> parcelas = new();
            decimal valorBase = decimal.Round(valor / quantidadeParcelas, 2, MidpointRounding.ToZero);
            for (int numero = 1; numero <= quantidadeParcelas; numero++)
            {
                // O resíduo vai para a última parcela: a soma sempre coincide com a compra.
                decimal valorParcela = numero == quantidadeParcelas
                    ? valor - valorBase * (quantidadeParcelas - 1) : valorBase;
                CicloFatura ciclo = CicloFatura.DoMes(cartao,
                    new DateTime(primeiroCiclo.Fechamento.Year, primeiroCiclo.Fechamento.Month, 1)
                        .AddMonths(numero - 1));
                parcelas.Add(new ParcelaCartao(Id, CartaoId, numero, quantidadeParcelas, valorParcela, ciclo));
            }
            _parcelas = parcelas;
        }



    }

}
