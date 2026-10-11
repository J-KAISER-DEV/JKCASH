using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Entidades
{
    public sealed class CartaoCredito
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public decimal Limite { get; private set; }
        public decimal ValorUtilizado { get; private set; }

        public decimal LimiteDisponivel => Limite - ValorUtilizado;

        public CartaoCredito(string nome, decimal limite)
        {
            if(string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException(
                    "Informe o nome do cartao. ", nameof(nome));

            }
            if(limite <= 0 || limite != decimal.Round(limite,2))
            {
                throw new ArgumentException(
                    "O limite deve ser positivo e ter no maximo duas casas decimais",
                    nameof(limite));
            }
            Id = Guid.NewGuid();
            Nome = nome.Trim();
            Limite = limite;
            ValorUtilizado = 0m;



        }
    }
}
