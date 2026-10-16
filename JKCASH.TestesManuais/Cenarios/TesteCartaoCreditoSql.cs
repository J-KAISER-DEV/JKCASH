using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteCartaoCreditoSql
    {
        public static void Executar()
        {
            // 1. PREPARAR
            ContaRepositorioSql contas = new();
            CartaoCreditoRepositorioSql cartoes = new();

            Conta? conta = contas.ListarTodas().FirstOrDefault();

            if (conta is null)
            {
                Console.WriteLine("Cadastre uma conta no SQL antes deste teste.");
                return;
            }

            CartaoCredito cartao = new(
                conta.Id, "Cartão de teste SQL", 1000m, 10, 20);

            // Prepara um estado diferente de zero para conferir sua recuperação.
            cartao.RegistrarCompra(200m);

            // 2. EXECUTAR E CONFERIR
            using (TransactionScope transacao = new())
            {
                cartoes.Adicionar(cartao);

                CartaoCredito? encontrado = cartoes.BuscarPorId(cartao.Id);

                if (encontrado is null)
                    throw new InvalidOperationException("O cartão não foi encontrado.");

                if (encontrado.ContaId != conta.Id
                    || encontrado.Nome != cartao.Nome
                    || encontrado.Limite != 1000m
                    || encontrado.ValorUtilizado != 200m
                    || encontrado.LimiteDisponivel != 800m
                    || encontrado.DiaFechamento != 10
                    || encontrado.DiaVencimento != 20
                    || encontrado.CompraNoFechamentoNaProximaFatura
                        != cartao.CompraNoFechamentoNaProximaFatura)
                {
                    throw new InvalidOperationException(
                        "Os dados recuperados diferem dos dados gravados.");
                }

                if (!cartoes.ListarTodos().Any(item => item.Id == cartao.Id))
                    throw new InvalidOperationException("O cartão não apareceu na lista.");

                Console.WriteLine("OK: inclusão, busca e listagem no SQL.");
            }

            // 3. CONFERIR A LIMPEZA
            if (cartoes.BuscarPorId(cartao.Id) is not null)
                throw new InvalidOperationException("O cartão de teste permaneceu no banco.");

            Console.WriteLine("OK: inclusão desfeita; cartão de teste removido.");
        }
    }
}
