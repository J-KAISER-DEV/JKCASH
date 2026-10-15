using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
namespace JKCASH.Domain.Entidades
{
    public sealed class CartaoCredito
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public decimal Limite { get; private set; }
        public decimal ValorUtilizado { get; private set; }
        
        public int DiaFechamento { get; private set; }
        public int DiaVencimento { get; private set; }

        public decimal LimiteDisponivel => Limite - ValorUtilizado;

        public CartaoCredito(
            string nome,
            decimal limite,
            int diaFechamento,
            int diaVencimento)
        {
            Validacao.TextoObrigatorio(nome, nameof(nome));
            Validacao.ValorMonetarioPositivo(limite, nameof(limite));
            Validacao.InteiroNoIntervalo(
                diaFechamento,1, 31, nameof(diaFechamento));
            Validacao.InteiroNoIntervalo(
                diaVencimento, 1, 31, nameof(diaVencimento));
            
         
            Id = Guid.NewGuid();
            Nome = nome.Trim();
            Limite = limite;
            ValorUtilizado = 0m;
            DiaFechamento = diaFechamento;
            DiaVencimento = diaVencimento;

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
    }
}
