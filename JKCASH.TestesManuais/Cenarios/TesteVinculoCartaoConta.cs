using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Repositorios;

namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteVinculoCartaoConta
    {
        public static void Executar()
        {
            Console.WriteLine("\n=== Vínculo entre cartão e conta ===");
            Conta conta = new("Conta principal", "Banco de teste", TipoConta.Corrente,
                saldoInicial: 500m);
            Conta outraConta = new("Reserva", "Outro banco", TipoConta.Poupança);
            CartaoCreditoRepositorioEmMemoria cartoes = new();

            // Uma conta pode existir sem cartões.
            if (cartoes.ListarTodos().Any(cartao => cartao.ContaId == conta.Id))
                throw new InvalidOperationException("A conta deveria iniciar sem cartões.");

            CartaoCredito primeiro = new(conta.Id, "Principal", 1000m, 10, 20);
            CartaoCredito segundo = new(conta.Id, "Adicional", 2000m, 5, 15);
            CartaoCredito terceiro = new(outraConta.Id, "Reserva", 800m, 1, 10);
            cartoes.Adicionar(primeiro);
            cartoes.Adicionar(segundo);
            cartoes.Adicionar(terceiro);

            if (cartoes.ListarTodos().Count(cartao => cartao.ContaId == conta.Id) != 2 ||
                cartoes.BuscarPorId(terceiro.Id)?.ContaId != outraConta.Id)
                throw new InvalidOperationException("Os vínculos com as contas não foram preservados.");

            primeiro.RegistrarCompra(150m);
            if (conta.Saldo.SaldoAtual != 500m || segundo.ValorUtilizado != 0m)
                throw new InvalidOperationException("A compra alterou o saldo da conta ou outro cartão.");

            bool contaVaziaRecusada = false;
            try
            {
                _ = new CartaoCredito(Guid.Empty, "Inválido", 1000m, 10, 20);
            }
            catch (ArgumentException erro) when (erro.ParamName == "contaId")
            {
                contaVaziaRecusada = true;
            }

            if (!contaVaziaRecusada)
                throw new InvalidOperationException("O cartão aceitou um identificador de conta vazio.");

            Console.WriteLine("Vínculos preservados, múltiplos cartões permitidos e conta vazia recusada.");
            Console.WriteLine("Compra preservou o saldo da conta e o limite do outro cartão.");
        }
    }
}
