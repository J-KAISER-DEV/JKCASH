using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Menus
{
    public class MenuTransacoes
    {
        private readonly IContaRepositorio _contas;
        private readonly ICategoriaRepositorio _categorias;
        private readonly ITransacaoRepositorio _repositorio;
        private readonly TransacaoServico _servico;

        public MenuTransacoes(
            IContaRepositorio contas,
            ICategoriaRepositorio categorias,
            ITransacaoRepositorio repositorio,
            TransacaoServico servico)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(servico);

            _contas = contas;
            _categorias = categorias;
            _repositorio = repositorio;
            _servico = servico;
        }

        public void Executar()
        {
            NavegacaoConsole.Executar("Transações", "Voltar",
                ("Registrar transação", RegistrarTransacao),
                ("Listar todas as transações", ListarTransacoes),
                ("Buscar transação por Id", BuscarTransacao),
                ("Listar transações por conta", ListarTransacoesPorConta));
        }

        public void RegistrarTransacao()
        {
            List<Conta> listaContas = _contas.ListarTodas();
            List<Categoria> listaCategorias = _categorias.ListarTodas();

            if (listaContas.Count == 0)
            {
                Console.WriteLine("Cadastre uma conta em Contas > Cadastrar conta.");
                return;
            }

            if (listaCategorias.Count == 0)
            {
                Console.WriteLine("Cadastre uma categoria em Categorias > Cadastrar categoria.");
                return;
            }

            Conta conta = MenuContas.SelecionarConta(listaContas);
            TipoTransacao tipo = LeituraConsole.LerOpcao<TipoTransacao>();
            decimal valor = LeituraConsole.LerDecimal("Valor (exemplo: 30,50): ");

            if (valor <= 0m || valor != decimal.Round(valor, 2))
            {
                throw new ArgumentException(
                    "O valor deve ser positivo e ter no máximo duas casas decimais.");
            }

            // Verifica a saída antes de pedir as demais informações; não debita aqui.
            if (tipo == TipoTransacao.Saida)
            {
                conta.ValidarSaida(valor);
            }
            ImportanciaTransacao importancia = LeituraConsole.LerOpcao<ImportanciaTransacao>();
            PlanejamentoTransacao planejamento = LeituraConsole.LerOpcao<PlanejamentoTransacao>();
            UrgenciaTransacao urgencia = LeituraConsole.LerOpcao<UrgenciaTransacao>();
            Categoria categoria = MenuCategorias.SelecionarCategoria(listaCategorias);
            string descricao = LeituraConsole.LerTexto("Descrição: ");
            DateTime data = LeituraConsole.LerData("Data (dd/MM/aaaa): ");
            string justificativa = LeituraConsole.LerTexto(
            "Por que você está realizando esta transação? ");

            Transacao transacao = _servico.Registrar(
                conta, tipo, valor, descricao, data, categoria.Id,importancia,planejamento,urgencia,justificativa);

            Console.WriteLine($"Transação registrada! Id: {transacao.Id}");
            Console.WriteLine($"Saldo atualizado: {conta.Saldo.SaldoAtual:C2}");
        }

        public void ListarTransacoes()
        {
            List<Transacao> lista = _repositorio.ListarTodas();
            Console.WriteLine($"Transações cadastradas: {lista.Count}");

            foreach (Transacao transacao in lista)
            {
                MostrarTransacao(transacao);
            }
        }

        public void BuscarTransacao()
        {
            string texto = LeituraConsole.LerTexto("Id da transação: ");

            if (!Guid.TryParse(texto, out Guid id))
            {
                throw new ArgumentException("O identificador digitado não é um Guid válido.");
            }

            Transacao? transacao = _repositorio.BuscarPorId(id);

            if (transacao is null)
            {
                Console.WriteLine("Transação não encontrada.");
                return;
            }

            MostrarTransacao(transacao);
        }

        public void ListarTransacoesPorConta()
        {
            List<Conta> lista = _contas.ListarTodas();

            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            Conta conta = MenuContas.SelecionarConta(lista);
            List<Transacao> resultado = _repositorio.ListarPorConta(conta.Id);

            Console.WriteLine($"Transações de {conta.Nome}: {resultado.Count}");

            foreach (Transacao transacao in resultado)
            {
                MostrarTransacao(transacao);
            }
        }

        private static void MostrarTransacao(Transacao transacao)
        {
            Console.WriteLine(
                $"{transacao.Data:dd/MM/yyyy} | {transacao.Tipo} | " +
                $"CategoriaId: {transacao.CategoriaId} | " +
                $"{transacao.Valor:C2} | {transacao.Descricao}");
            Console.WriteLine($"Id: {transacao.Id} | ContaId: {transacao.ContaId} | Importancia {transacao.Importancia} | Planejada? {transacao.Planejamento} | Urgencia: {transacao.Urgencia} Justificativa: {transacao.Justificativa}");
        }
    }
}
