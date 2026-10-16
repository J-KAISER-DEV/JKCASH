using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Menus
{
    public class MenuContas
    {
        private readonly IContaRepositorio _repositorio;

        public MenuContas(IContaRepositorio repositorio)
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            _repositorio = repositorio;
        }

        public void Executar()
        {
            NavegacaoConsole.Executar("Contas", "Voltar",
                ("Cadastrar conta", CadastrarConta),
                ("Listar contas", ListarContas));
        }

        public void CadastrarConta()
        {
            string nome = LeituraConsole.LerTexto("Nome da conta: ");
            string banco = LeituraConsole.LerTexto("Banco: ");
            TipoConta tipo = LeituraConsole.LerOpcao<TipoConta>();
            DestinacaoConta destinacao = LeituraConsole.LerOpcao<DestinacaoConta>();
            decimal saldo = LeituraConsole.LerDecimal("Saldo inicial (exemplo: 100,50): ");
            decimal limite = LeituraConsole.LerDecimal("Limite do cheque especial (0 se não houver): ");

            Conta conta = new(nome, banco, tipo, destinacao, saldo, limite);
            _repositorio.Adicionar(conta);

            Console.WriteLine($"Conta cadastrada! Id: {conta.Id}");
        }

        public void ListarContas()
        {
            List<Conta> lista = _repositorio.ListarTodas();

            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            foreach (Conta conta in lista)
            {
                Console.WriteLine($"{conta.Nome} | {conta.Banco} | Saldo: {conta.Saldo.SaldoAtual:C2}");
                Console.WriteLine($"Id: {conta.Id}");
            }
        }

        public static Conta SelecionarConta(List<Conta> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {lista[i].Nome} ({lista[i].Banco})");
            }

            int numero = LeituraConsole.LerNumeroDaLista("Escolha o número da conta: ", lista.Count);
            return lista[numero - 1];
        }
    }
}
