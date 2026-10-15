using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Validacoes
{
    public static class Validacao
    {
        public static void ValorMonetarioPositivo
            (decimal valor,
            string nomeParametro
            )
        {
            ValorMonetario(valor, nomeParametro);
            if (valor == 0m)
            {
                throw new ArgumentException(
                    "O valor deve ser maior que zero.",
                    nomeParametro);
            }
        }

        public static void EnumValido<T>
            (T valor,
            string nomeParametro
            ) where T : struct, Enum
        {
            if(!Enum.IsDefined(typeof(T), valor))
            {
                throw new ArgumentException(
                    "Escolha uma opção valida",
                    nomeParametro);
            }
        }
        public static void TextoObrigatorio(
            string? valor,
            string nomeParametro)
        {
            if(string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException(
                    "Informe um texto que nao esteja vazio",
                    nomeParametro);
            }
        }

        public static void GuidNaoVazio(
            Guid valor,
            string nomeParametro)
        {
            if(valor == Guid.Empty)
            {
                throw new ArgumentException(
                    "Informe um identificador valido",
                    nomeParametro);
            }
        }

        public static void ValorMonetario(
            decimal valor,
            string nomeParametro,
            bool permitirNegativo = false)
        {
            if((!permitirNegativo && valor <0m) || valor != decimal.Round(valor, 2))
            {
                throw new ArgumentException(
                    "Valor monetario invalido. Utilize no maximo duas casas decimais",
                    nomeParametro);
            }
        }

        public static void InteiroNoIntervalo(
            int valor,
            int minimo,
            int maximo,
            string nomeParametro)
        {
            if(valor < minimo || valor > maximo)
            {
                throw new ArgumentException(
                    $"O valor deve estar entre {minimo} e {maximo}.",
                    nomeParametro);
            }
        }
        public static void TextoObrigatorio(
            string? valor,
            string nomeParametro,
            int tamanhoMaximo)
        {
            TextoObrigatorio(valor, nomeParametro);

            if(valor is not null && valor.Length > tamanhoMaximo)
            {
                throw new ArgumentException(
                    $"O texto deve ter no maximo {tamanhoMaximo} caracteres.",
                    nomeParametro);
            }
        }

    }
}
