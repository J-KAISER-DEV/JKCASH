using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Menus
{
    public class MenuCartao
    {
        private readonly ICartaoCreditoRepositorio _repositorio;
        private readonly IContaRepositorio _contas;

        public MenuCartao(ICartaoCreditoRepositorio repositorio, IContaRepositorio contas)
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(contas);
            _repositorio = repositorio;
            _contas = contas;
        }

        public void CadastrarCartao()
        {
            List<Conta> contas = _contas.ListarTodas();
            if (contas.Count == 0)
            {
                Console.WriteLine("Cadastre uma conta primeiro pela opção 1.");
                return;
            }

            Console.WriteLine("Selecione a conta à qual o cartão pertence:");
            Conta conta = MenuContas.SelecionarConta(contas);
            string nome = LeituraConsole.LerTexto("Nome do cartao: ");
            decimal limite = LeituraConsole.LerDecimal("Limite do cartao: ");

            int diaFechamento = LeituraConsole.LerInteiro("Dia de fechamento: ");
            int diaVencimento = LeituraConsole.LerInteiro("Dia de vencimento: ");
            Console.WriteLine("Compras no próprio dia do fechamento:");
            Console.WriteLine("1 - Entram na próxima fatura");
            Console.WriteLine("2 - Entram na fatura que fecha nesse dia");
            bool proximaFatura = LeituraConsole.LerNumeroDaLista("Escolha: ", 2) == 1;

            CartaoCredito cartao = new(
                conta.Id,
                nome,
                limite,
                diaFechamento,
                diaVencimento,
                proximaFatura);

            _repositorio.Adicionar(cartao);

            Console.WriteLine($"Cartão cadastrado: Id:{cartao.Id} | Conta: {conta.Nome} | Banco: {conta.Banco}");
        }
        public void ListarCartoes()
        {
            List<CartaoCredito> cartoes = _repositorio.ListarTodos();

            if (cartoes.Count == 0)
            {
                Console.WriteLine("Nenhum cartão cadastrado.");
                return;
            }

            Dictionary<Guid, Conta> contas = _contas.ListarTodas()
                .ToDictionary(conta => conta.Id);

            foreach (CartaoCredito cartao in cartoes)
            {
                Console.WriteLine($"{cartao.Nome} | Id: {cartao.Id}");
                if (contas.TryGetValue(cartao.ContaId, out Conta? conta))
                {
                    Console.WriteLine($"Conta: {conta.Nome} | Banco: {conta.Banco}");
                }
                else
                {
                    Console.WriteLine($"Conta não encontrada | ContaId: {cartao.ContaId}");
                }
                Console.WriteLine(
                    $"Limite: {cartao.Limite:C2} | " +
                    $"Utilizado: {cartao.ValorUtilizado:C2} | " +
                    $"Disponível: {cartao.LimiteDisponivel:C2}");
                Console.WriteLine(
                    $"Fechamento: dia {cartao.DiaFechamento} | " +
                    $"Vencimento: dia {cartao.DiaVencimento}");
                Console.WriteLine("Compra no fechamento: " + (cartao.CompraNoFechamentoNaProximaFatura
                    ? "próxima fatura" : "fatura do próprio dia"));
                Console.WriteLine();
            }
        }

        public static CartaoCredito SelecionarCartao(List<CartaoCredito> cartoes)
        {
            for (int i = 0; i < cartoes.Count; i++)
                Console.WriteLine($"{i + 1} - {cartoes[i].Nome} | Disponível: {cartoes[i].LimiteDisponivel:C2} | Id: {cartoes[i].Id}");
            int numero = LeituraConsole.LerNumeroDaLista("Escolha o número do cartão: ", cartoes.Count);
            return cartoes[numero - 1];
        }
    }
}
