using JKCASH.Domain.Entidades;

namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteCartaoCredito
    {
        public static void Executar()
        {
            Console.WriteLine("\n=== Entidade CartaoCredito ===");
            CompraDentroDoLimite();
            CompraAcimaDoLimite();

        }

        private static void CompraDentroDoLimite()
        {
            // PREPARAR
            CartaoCredito cartao = new("Cartão de teste", 1000m, 10, 20);

            // EXECUTAR
            cartao.RegistrarCompra(200m);

            // CONFERIR
            Console.WriteLine("Cenário: compra dentro do limite");
            Console.WriteLine($"Utilizado: {cartao.ValorUtilizado:C2}");
            Console.WriteLine($"Disponível: {cartao.LimiteDisponivel:C2}");
            Console.WriteLine($"Utilizado correto: {cartao.ValorUtilizado == 200m}");
            Console.WriteLine($"Disponível correto: {cartao.LimiteDisponivel == 800m}");
        }

        private static void CompraAcimaDoLimite()
        {
            // PREPARAR: este cenário tem seu próprio cartão.
            CartaoCredito cartao = new("Cartão de teste", 1000m, 10, 20);
            cartao.RegistrarCompra(200m);
            bool compraRecusada = false;

            // EXECUTAR
            try
            {
                cartao.RegistrarCompra(801m);
            }
            catch (ArgumentException erro)
            {
                compraRecusada = true;
                Console.WriteLine($"Compra recusada: {erro.Message}");
            }

            // CONFERIR
            Console.WriteLine("Cenário: compra acima do limite");
            Console.WriteLine($"Recusa correta: {compraRecusada}");
            Console.WriteLine($"Utilizado preservado: {cartao.ValorUtilizado == 200m}");
            Console.WriteLine($"Disponível preservado: {cartao.LimiteDisponivel == 800m}");
        }
    }
}
