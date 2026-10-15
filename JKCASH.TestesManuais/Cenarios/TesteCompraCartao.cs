using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.TestesManuais.Doubles;
namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteCompraCartao
    {

        public static void CompraValida()
        {
            CartaoCreditoRepositorioEmMemoria cartoes = new();
            CategoriaRepositorioEmMemoria categorias = new();
            CompraCartaoRepositorio compras = new();

            CartaoCredito cartao = new(
                "Cartão faculdade", 1000m, 10, 20);

            Categoria categoria = new("Estudos");

            cartoes.Adicionar(cartao);
            categorias.Adicionar(categoria);

            CompraCartaoServico servico = new(
                cartoes, categorias, compras);

            // 2. EXECUTAR
            CompraCartao compra = servico.Registrar(
                cartao.Id,
                150m,
                "Compra de livros",
                DateTime.Today,
                categoria.Id,
                ImportanciaTransacao.Alta,
                PlanejamentoTransacao.Planejada,
                UrgenciaTransacao.SemUrgencia,
                "Livros para a faculdade");

            // 3. CONFERIR
            Console.WriteLine("Cenário: compra válida");

            Console.WriteLine(
                $"Cartão correto: {compra.CartaoId == cartao.Id}");

            Console.WriteLine(
                $"Valor correto: {compra.Valor == 150m}");

            Console.WriteLine(
                $"Utilizado correto: {cartao.ValorUtilizado == 150m}");

            Console.WriteLine(
                $"Disponível correto: {cartao.LimiteDisponivel == 850m}");
        }
        public static void Executar()
        {
            Console.WriteLine("\n=== Compra no cartão: entidade e serviço ===");
            CriacaoDaCompra();
            CompraValida();
            FalhaAoGuardarCompra();
        }

        private static void CriacaoDaCompra()
        {
            // PREPARAR
            CartaoCredito cartao = new("Cartão de teste", 1000m, 10, 20);
            Categoria categoria = new("Estudos");

            // EXECUTAR: criar o objeto não consome limite.
            CompraCartao compra = new(
                cartao.Id, 150m, "Compra de livros", DateTime.Today,
                categoria.Id, ImportanciaTransacao.Alta,
                PlanejamentoTransacao.Planejada, UrgenciaTransacao.SemUrgencia,
                "Livros para a faculdade");

            // CONFERIR
            Console.WriteLine("Cenário: criação da entidade CompraCartao");
            Console.WriteLine($"Compra: {compra.Descricao}");
            Console.WriteLine($"Valor: {compra.Valor:C2}");
            Console.WriteLine($"Cartão correto: {compra.CartaoId == cartao.Id}");
            Console.WriteLine($"Id criado: {compra.Id != Guid.Empty}");
            Console.WriteLine($"Limite preservado: {cartao.LimiteDisponivel == 1000m}");
        }
        private static void FalhaAoGuardarCompra()
        {
            // 1. PREPARAR
            CartaoCreditoRepositorioEmMemoria cartoes = new();
            CategoriaRepositorioEmMemoria categorias = new();
            CompraCartaoRepositorioComFalha compras = new();

            CartaoCredito cartao = new(
                "Cartão de teste", 1000m, 10, 20);

            Categoria categoria = new("Estudos");

            cartoes.Adicionar(cartao);
            categorias.Adicionar(categoria);

            CompraCartaoServico servico = new(
                cartoes, categorias, compras);

            bool falhaRecebida = false;

            // 2. EXECUTAR
            try
            {
                servico.Registrar(
                    cartao.Id,
                    150m,
                    "Compra de livros",
                    DateTime.Today,
                    categoria.Id,
                    ImportanciaTransacao.Alta,
                    PlanejamentoTransacao.Planejada,
                    UrgenciaTransacao.SemUrgencia,
                    "Livros para a faculdade");
            }
            catch (InvalidOperationException erro)
                when (erro.Message == "Falha simulada ao guardar a compra.")
            {
                falhaRecebida = true;
                Console.WriteLine(erro.Message);
            }

            // 3. CONFERIR
            Console.WriteLine("Cenário: falha ao guardar compra");
            Console.WriteLine($"Falha recebida: {falhaRecebida}");
            Console.WriteLine(
                $"Utilizado restaurado: {cartao.ValorUtilizado == 0m}");
            Console.WriteLine(
                $"Disponível restaurado: {cartao.LimiteDisponivel == 1000m}");
        }
    }
}
