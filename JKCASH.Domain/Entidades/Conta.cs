using JKCASH.Domain.Enums;
using JKCASH.Domain.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
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
            Validacao.TextoObrigatorio(nome,nameof (nome),100);
            Validacao.TextoObrigatorio(banco,nameof (banco),100);
            Validacao.EnumValido(tipo, nameof (tipo));
            Validacao.EnumValido(destinacao, nameof(destinacao));
            Id = Guid.NewGuid();
            Tipo = tipo;
            Destinacao = destinacao;
            Saldo = new Saldo(
                saldoInicial,
                limiteChequeEspecial);
            Ativa = true;
            Nome = nome.Trim();
            Banco = banco.Trim();

        }
        

        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Banco { get; private set; } = string.Empty;
        public TipoConta Tipo { get; private set; }
        public DestinacaoConta Destinacao { get; private set; }
        public Saldo Saldo { get; private set; } = null!;
        public bool Ativa {  get; private set; }

        public static Conta Restaurar(
            Guid id,
            string nome,
            string banco,
            TipoConta tipo,
            DestinacaoConta destinacao,
            decimal saldoInicial,
            decimal saldoAtual,
            decimal limiteChequeEspecial,
            bool ativa)
        {
            Validacao.GuidNaoVazio(id, nameof(id));
            
            Validacao.EnumValido(tipo, nameof(tipo));
            Validacao.EnumValido(destinacao, nameof(destinacao));
            Conta conta = new Conta();

            conta.Id = id;

            Validacao.TextoObrigatorio(nome, nameof(nome),100);
            conta.Nome = nome.Trim();

            Validacao.TextoObrigatorio(banco, nameof(banco),100);
            conta.Banco = banco.Trim();

            conta.Tipo = tipo;
            conta.Destinacao = destinacao;

            conta.Saldo =
            Saldo.Restaurar(
                saldoInicial,
                saldoAtual,
                limiteChequeEspecial);

            conta.Ativa = ativa;
            return conta;

        }


       private void ValidarContaAtiva()
        {
            if(!Ativa)
            {
                throw new InvalidOperationException(
                    "Não é possivel movimentar uma conta desativada. ");
            }
        }

        public void ValidarSaida(decimal valor)
        {
            ValidarContaAtiva();
            Saldo.ValidarDebito(valor);
        }
        public void Creditar(decimal valor)
        {
            ValidarContaAtiva();
            Saldo.Creditar(valor);
        }

        public void Debitar(decimal valor)
        {
            ValidarContaAtiva();
            Saldo.Debitar(valor);
        }
        public void AlterarNome(string novoNome)
        {
            Validacao.TextoObrigatorio(novoNome, nameof(novoNome),100);
            Nome = novoNome.Trim();
        }
        public void Desativar()
        {
            Ativa = false;
        }
        public void AlterarBanco(string novoBanco)
        {

            Validacao.TextoObrigatorio(novoBanco, nameof(novoBanco),100);
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
