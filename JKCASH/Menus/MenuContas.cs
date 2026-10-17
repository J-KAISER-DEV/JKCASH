using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Enums;

namespace JKCASH.Menus
{
    public class MenuContas
    {
        private readonly IContaRepositorio _repositorio;
        private readonly ContaServico _servico;
        private readonly TransferenciaServico _transferencias;
        private readonly ICategoriaRepositorio _categorias;

        public MenuContas(IContaRepositorio repositorio, ContaServico servico,
            TransferenciaServico transferencias, ICategoriaRepositorio categorias)
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(servico);
            ArgumentNullException.ThrowIfNull(transferencias);
            ArgumentNullException.ThrowIfNull(categorias);
            _repositorio = repositorio;
            _servico = servico;
            _transferencias = transferencias;
            _categorias = categorias;
        }

        public void Executar()
        {
            NavegacaoConsole.Executar("Contas", "Voltar",
                ("Cadastrar conta", CadastrarConta),
                ("Listar contas", ListarContas),
                ("Excluir conta", ExcluirConta),
                ("Transferir entre contas", TransferirEntreContas));
        }

        public void TransferirEntreContas()
        {
            List<Conta> contas = _repositorio.ListarTodas().Where(conta => conta.Ativa).ToList();
            if (contas.Count < 2)
            {
                Console.WriteLine("Cadastre pelo menos duas contas ativas para transferir.");
                return;
            }

            List<Categoria> categorias = _categorias.ListarTodas();
            if (categorias.Count == 0)
            {
                Console.WriteLine("Cadastre uma categoria em Categorias > Cadastrar categoria.");
                return;
            }

            Console.WriteLine("Conta de origem:");
            Conta origem = SelecionarConta(contas);
            decimal valor = LeituraConsole.LerDecimal("Valor da transferência: ");
            origem.ValidarSaida(valor);

            Console.WriteLine("Conta de destino:");
            Conta destino = SelecionarConta(contas.Where(conta => conta.Id != origem.Id).ToList());
            Categoria categoria = MenuCategorias.SelecionarCategoria(categorias);
            ImportanciaTransacao importancia = LeituraConsole.LerOpcao<ImportanciaTransacao>();
            PlanejamentoTransacao planejamento = LeituraConsole.LerOpcao<PlanejamentoTransacao>();
            UrgenciaTransacao urgencia = LeituraConsole.LerOpcao<UrgenciaTransacao>();
            string descricao = LeituraConsole.LerTexto("Descrição: ");
            string justificativa = LeituraConsole.LerTexto("Justificativa: ");

            Transferencia transferencia = _transferencias.Registrar(origem.Id, destino.Id, valor,
                categoria.Id, importancia, planejamento, urgencia, descricao, justificativa);

            // No SQL, o serviço carregou as contas em outro contexto.
            var atualizadas = _repositorio.ListarTodas();
            origem = atualizadas.Single(conta => conta.Id == origem.Id);
            destino = atualizadas.Single(conta => conta.Id == destino.Id);
            Console.WriteLine($"Transferência realizada! Id: {transferencia.Id}");
            Console.WriteLine($"{origem.Nome}: {origem.Saldo.SaldoAtual:C2}");
            Console.WriteLine($"{destino.Nome}: {destino.Saldo.SaldoAtual:C2}");
        }

        public void ExcluirConta()
        {
            List<Conta> lista = _repositorio.ListarTodas();
            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            Conta conta = SelecionarConta(lista);
            Console.WriteLine($"Excluir: {conta.Nome} | {conta.Banco} | Saldo: {conta.Saldo.SaldoAtual:C2}");
            Console.WriteLine($"Id: {conta.Id}");
            int quantidade = _servico.ContarTransacoes(conta.Id);
            Console.WriteLine($"Serão excluídas esta conta e suas {quantidade} transações. Essa ação é definitiva.");
            Console.WriteLine("Contas com cartões, pagamentos de fatura ou transferências vinculadas não podem ser excluídas.");
            Console.Write("Digite EXCLUIR para confirmar a exclusão da conta e das transações: ");
            if (!string.Equals(Console.ReadLine()?.Trim(), "EXCLUIR", StringComparison.Ordinal))
            {
                Console.WriteLine("Exclusão cancelada.");
                return;
            }

            _servico.Excluir(conta.Id, excluirTransacoes: true);
            Console.WriteLine("Conta e suas transações excluídas com sucesso.");
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
        public static List<Conta> SelecionarContas(List<Conta> lista)
        {
            if (lista.Count == 0)
                throw new InvalidOperationException("Nenhuma conta cadastrada.");

            Console.WriteLine("0 - Todas as contas");

            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1} - {lista[i].Nome} ({lista[i].Banco})");
            }

            string escolha = LeituraConsole.LerTexto(
                "Escolha uma ou mais contas (exemplo: 1,3) ou 0 para todas: ");

            if (escolha == "0")
                return lista.ToList();

            List<Conta> selecionadas = new();

            foreach (string parte in escolha.Split(','))
            {
                if (!int.TryParse(parte.Trim(), out int numero)
                    || numero < 1
                    || numero > lista.Count)
                {
                    throw new ArgumentException(
                        "Informe números da lista separados por vírgula.");
                }

                Conta conta = lista[numero - 1];

                if (!selecionadas.Contains(conta))
                {
                    selecionadas.Add(conta);
                }
            }

            return selecionadas;
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
