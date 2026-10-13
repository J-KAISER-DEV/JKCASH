using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace JKCASH.Domain.ObjetosDeValor
{
    public sealed class Saldo
    {
        private Saldo() { }
        internal Saldo(
            decimal saldoInicial = 0m,
            decimal limiteChequeEspecial = 0m
            )
        {
            ValidarValorMonetario(
                saldoInicial, nameof(saldoInicial), true);

            ValidarValorMonetario(
                limiteChequeEspecial, nameof(limiteChequeEspecial), false);

            if (saldoInicial < -limiteChequeEspecial)
            {
                throw new ArgumentException(
                    "O saldo inicial ultrapassa o cheque especial. ");

            }
            
            SaldoInicial = saldoInicial;
            SaldoAtual = saldoInicial;
            LimiteChequeEspecial = limiteChequeEspecial;

        }
        public decimal SaldoInicial { get; private set; }
        public decimal SaldoAtual { get; private set; }
        public decimal LimiteChequeEspecial { get; private set; }
        public decimal ChequeEspecialUtilizado =>
            Math.Max(0m, -SaldoAtual);
        public decimal ChequeEspecialDisponivel =>
        LimiteChequeEspecial - ChequeEspecialUtilizado;
        public decimal DisponivelComChequeEspecial =>
            SaldoAtual + LimiteChequeEspecial;

        internal void Creditar(decimal valor)
        {
            ValidarValorPositivo(valor);
            SaldoAtual += valor;
        }
        internal void ValidarDebito(decimal valor)
        {
            ValidarValorPositivo(valor);

            if(valor > DisponivelComChequeEspecial)
            {
                throw new ArgumentException(
                    "Saldo e cheque especial insuficiente. ");
               
            }
        }

        internal void Debitar(decimal valor)
        {
            ValidarDebito(valor);
            SaldoAtual -= valor;
        }

        internal void AlterarLimiteChequeEspecial(
            decimal novoLimite)
        {
            ValidarValorMonetario(
                novoLimite, nameof(novoLimite), false);

            if(SaldoAtual < -novoLimite)
            {
                throw new InvalidOperationException(
                    "O novo limite e menor que o cheque especial utilizado. ");

            }
            LimiteChequeEspecial = novoLimite;
        }

        private static void ValidarValorPositivo(decimal valor)
        {
            ValidarValorMonetario(valor, nameof(valor), false);
            if( valor == 0m)
            {
                throw new ArgumentException(
                    "O valor deve ser maior que zero. ");
            }
        }
        private static void ValidarValorMonetario(
            decimal valor,
            string nome,
            bool permitirNegativo
            )
        {
            if((!permitirNegativo && valor < 0m) 
                || valor != decimal.Round(valor,2 ))
            {
                throw new ArgumentException(
                    $"O valor de {nome} é invalido. " +
                    "Utilize no maximo duas casa decimais.");
            }
        }
    }
}
