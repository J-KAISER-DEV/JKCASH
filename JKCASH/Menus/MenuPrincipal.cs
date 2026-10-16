namespace JKCASH.Menus
{
    public class MenuPrincipal
    {
        private readonly MenuContas _contas;
        private readonly MenuCartao _cartoes;
        private readonly MenuCategorias _categorias;
        private readonly MenuTransacoes _transacoes;
        private readonly MenuFaturas _faturas;

        public MenuPrincipal(
            MenuContas contas,
            MenuCategorias categorias,
            MenuTransacoes transacoes,
            MenuCartao cartoes,
            MenuFaturas faturas)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(transacoes);
            ArgumentNullException.ThrowIfNull(cartoes);
            ArgumentNullException.ThrowIfNull(faturas);

            _contas = contas;
            _categorias = categorias;
            _transacoes = transacoes;
            _cartoes = cartoes;
            _faturas = faturas;
        }

        public void Executar()
        {
            NavegacaoConsole.Executar("JKCASH - Menu principal", "Sair",
                ("Contas", _contas.Executar),
                ("Transações", _transacoes.Executar),
                ("Cartões", _cartoes.Executar),
                ("Faturas", _faturas.Executar),
                ("Categorias", _categorias.Executar));

            Console.WriteLine("Programa encerrado.");
        }
    }
}
