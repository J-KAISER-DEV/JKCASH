using JKCASH.Domain.Enums;
using JKCASH.Domain.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Entidades
{
    public sealed class Conta
    {
        private Conta() { }
        public Conta(
            string nome,
            string banco,
            TipoConta tipo,
            DestinacaoConta destinacao = DestinacaoConta.UsoDiario,
            decimal saldoInicial = 0m,
            decimal limiteChequeEspecial = 0m)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException(
                    "Informe o nome da conta. ");
            }

            if (string.IsNullOrWhiteSpace(banco))
            {
                throw new ArgumentNullException(
                    "Informe o banco. ");
            }

            if (!Enum.IsDefined(typeof(TipoConta), tipo))
            {
                throw new ArgumentException(
                    "Tipo de conta invalido. ");

            }
            if (!Enum.IsDefined(typeof(DestinacaoConta), destinacao))
            {
                throw new ArgumentException(
                    "Destinação de conta invalido. ");

            }
            Id = Guid.NewGuid();
            Nome = nome.Trim();
            Banco = banco.Trim();
            Tipo = tipo;
            Destinacao = destinacao;
            Saldo = new Saldo(
                saldoInicial,
                limiteChequeEspecial);
            Ativa = true;

        }
        

        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Banco { get; private set; } = string.Empty;
        public TipoConta Tipo { get; private set; }
        public DestinacaoConta Destinacao { get; private set; }
        public Saldo Saldo { get; private set; } = null!;
        public bool Ativa {  get; private set; }

        public void AlterarNome(string novoNome)
        {
            if(string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException(
                    "Informe o nome da conta. ");
            }
            Nome = novoNome;
        }
        public void Desativar()
        {
            Ativa = false;
        }
        public void AlterarBanco(string novoBanco)
        {
            if (string.IsNullOrWhiteSpace(novoBanco))
            {
                throw new ArgumentException(
                    "Informe o banco.");
            }

            Banco = novoBanco.Trim();
        }

        public void AlterarLimiteChequeEspecial(decimal novoLimite)
        {
            Saldo.AlterarLimiteChequeEspecial(novoLimite);
        }
        public void Reativar()
        {
            Ativa = true;
        }

    }
}
