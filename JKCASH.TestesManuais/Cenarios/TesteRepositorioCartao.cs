using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Repositorios;

namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteRepositorioCartao
    {
        public static void Executar()
        {
            Console.WriteLine("\n=== Repositório de cartões em memória ===");

            // PREPARAR
            ICartaoCreditoRepositorio cartoes = new CartaoCreditoRepositorioEmMemoria();
            CartaoCredito cartao = new(Guid.NewGuid(), "Cartão faculdade", 1000m, 10, 20);

            // EXECUTAR
            cartoes.Adicionar(cartao);
            CartaoCredito? encontrado = cartoes.BuscarPorId(cartao.Id);

            // CONFERIR
            Console.WriteLine($"Cartão encontrado: {encontrado?.Nome}");
            Console.WriteLine($"Mesmo Id: {encontrado?.Id == cartao.Id}");
            Console.WriteLine($"Quantidade: {cartoes.ListarTodos().Count}");
            Console.WriteLine(
                $"Busca inexistente retornou null: {cartoes.BuscarPorId(Guid.NewGuid()) is null}");
        }
    }
}
