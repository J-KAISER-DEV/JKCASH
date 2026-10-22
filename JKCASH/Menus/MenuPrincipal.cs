namespace JKCASH.Menus
{
    public class MenuPrincipal
    {
        private readonly MenuContas _contas;
        private readonly MenuCartao _cartoes;
        private readonly MenuCategorias _categorias;
        private readonly MenuTransacoes _transacoes;
        private readonly MenuFaturas _faturas;
        private readonly MenuExclusoes? _exclusoes;

        public MenuPrincipal(
            MenuContas contas,
            MenuCategorias categorias,
            MenuTransacoes transacoes,
            MenuCartao cartoes,
            MenuFaturas faturas,
            MenuExclusoes? exclusoes = null)
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
            _exclusoes = exclusoes;
        }

        public void Executar()
        {
            List<(string Texto, Action Acao)> opcoes = new()
            {
                ("Contas", _contas.Executar),
                ("Transações", _transacoes.Executar),
                ("Cartões", _cartoes.Executar),
                ("Faturas", _faturas.Executar),
                ("Categorias", _categorias.Executar)
            };
            if (_exclusoes is not null) opcoes.Add(("Excluir registros", _exclusoes.Executar));
            NavegacaoConsole.Executar("JKCASH - Menu principal", "Sair", opcoes.ToArray());

            Console.WriteLine("Programa encerrado.");
        }
    }
}
