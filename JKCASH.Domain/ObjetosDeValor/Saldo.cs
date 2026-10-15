using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using JKCASH.Domain.Validacoes;
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
            Validacao.ValorMonetario(
                saldoInicial, nameof(saldoInicial), true);

            Validacao.ValorMonetario(
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

        internal static Saldo Restaurar(
            decimal saldoInicial,
            decimal saldoAtual,
            decimal limiteChequeEspecial)
        {
            Validacao.ValorMonetario(
                saldoInicial, nameof(saldoInicial), true);

            Validacao.ValorMonetario(
                saldoAtual, nameof(saldoAtual), true);

            Validacao.ValorMonetario(
                limiteChequeEspecial, nameof(limiteChequeEspecial), false);

            if(saldoAtual < -limiteChequeEspecial)
            {
                throw new ArgumentException(
                    "O saldo atual ultrapassa o cheque especial",
                    nameof(saldoAtual));
            }
            Saldo saldo = new Saldo();

            saldo.SaldoInicial = saldoInicial;
            saldo.SaldoAtual = saldoAtual;
            saldo.LimiteChequeEspecial = limiteChequeEspecial;

            return saldo;

        }
        internal void Creditar(decimal valor)
        {
            Validacao.ValorMonetarioPositivo(valor,nameof(valor));
            SaldoAtual += valor;
        }
        internal void ValidarDebito(decimal valor)
        {
            Validacao.ValorMonetarioPositivo(valor, nameof(valor));

            if (valor > DisponivelComChequeEspecial)
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
            Validacao.ValorMonetario(
                novoLimite, nameof(novoLimite), false);

            if(SaldoAtual < -novoLimite)
            {
                throw new InvalidOperationException(
                    "O novo limite e menor que o cheque especial utilizado. ");

            }
            LimiteChequeEspecial = novoLimite;
        }


      
    }
}
