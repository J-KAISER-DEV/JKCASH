using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
namespace JKCASH.Domain.Entidades
{
    public sealed class CartaoCredito
    {

        public Guid Id { get; private set; }
        public Guid ContaId { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public decimal Limite { get; private set; }
        public decimal ValorUtilizado { get; private set; }
        
        public int DiaFechamento { get; private set; }
        public int DiaVencimento { get; private set; }
        public bool CompraNoFechamentoNaProximaFatura { get; private set; }

        public decimal LimiteDisponivel => Limite - ValorUtilizado;

        private CartaoCredito() { }
        public CartaoCredito(
            Guid contaId,
            string nome,
            decimal limite,
            int diaFechamento,
            int diaVencimento,
            bool compraNoFechamentoNaProximaFatura = true)
        {
            Validacao.GuidNaoVazio(contaId, nameof(contaId));
            Validacao.TextoObrigatorio(nome, nameof(nome),100);
            Validacao.ValorMonetarioPositivo(limite, nameof(limite));
            Validacao.InteiroNoIntervalo(
                diaFechamento,1, 31, nameof(diaFechamento));
            Validacao.InteiroNoIntervalo(
                diaVencimento, 1, 31, nameof(diaVencimento));
            
         
            Id = Guid.NewGuid();
            ContaId = contaId;
            Nome = nome.Trim();
            Limite = limite;
            ValorUtilizado = 0m;
            DiaFechamento = diaFechamento;
            DiaVencimento = diaVencimento;
            CompraNoFechamentoNaProximaFatura = compraNoFechamentoNaProximaFatura;

        }
        public static CartaoCredito Restaurar(Guid id, Guid contaId, string nome, decimal limite,
            decimal valorUtilizado, int diaFechamento, int diaVencimento, bool proximaFatura)
        {
            Validacao.GuidNaoVazio(id, nameof(id));
            Validacao.ValorMonetario(valorUtilizado, nameof(valorUtilizado), false);
            if (valorUtilizado > limite) throw new ArgumentException("O valor utilizado ultrapassa o limite do cartão.");
            CartaoCredito cartao = new(contaId, nome, limite, diaFechamento, diaVencimento, proximaFatura);
            cartao.Id = id;
            cartao.ValorUtilizado = valorUtilizado;
            return cartao;
        }

        public void RegistrarCompra(decimal valor)
        {
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));
            if (valor > LimiteDisponivel)
            {
                throw new ArgumentException(
                    "Limite insuficiente",nameof(valor));
            }
            ValorUtilizado += valor;
        }

        public void EstornarCompra(decimal valor)
        {
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));

            if (valor > ValorUtilizado)
            {
                throw new ArgumentException
                    ("O estorno nao pode ultrapassar o valor utilizado",
                    nameof(valor));
            }
            ValorUtilizado -= valor;
        }

        public void RegistrarPagamento(decimal valor)
        {
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));
            if (valor > ValorUtilizado)
                throw new ArgumentException("O pagamento ultrapassa o valor utilizado do cartão.", nameof(valor));

            ValorUtilizado -= valor;
        }
    }
}
