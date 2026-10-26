using JKCASH.Application.Modelos;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Menus
{
    public class MenuTransacoes
    {
        private readonly RelatorioTransacoesServico _relatorios;
        private readonly IContaRepositorio _contas;
        private readonly ICategoriaRepositorio _categorias;
        private readonly ITransacaoRepositorio _repositorio;
        private readonly TransacaoServico _servico;

        public MenuTransacoes(
            IContaRepositorio contas,
            ICategoriaRepositorio categorias,
            ITransacaoRepositorio repositorio,
            TransacaoServico servico,
            RelatorioTransacoesServico relatorios)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(repositorio);
            ArgumentNullException.ThrowIfNull(servico);
            ArgumentNullException.ThrowIfNull(relatorios);

            _contas = contas;
            _categorias = categorias;
            _repositorio = repositorio;
            _servico = servico;
            _relatorios = relatorios;
        }

        public void MostrarExtrato()
        {
            List<Conta> contas = _contas.ListarTodas();

            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            List<Conta> selecionadas = MenuContas.SelecionarContas(contas);

            DateTime inicio = LeituraConsole.LerData("Data inicial: ");
            DateTime fim = LeituraConsole.LerData("Data final: ");

            List<Guid> contaIds = selecionadas
                .Select(conta => conta.Id)
                .ToList();

            List<Transacao> transacoes = _repositorio.ListarPorPeriodo(
                contaIds, inicio, fim);

            Console.WriteLine(
                $"Extrato de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy}");

            foreach (Conta conta in selecionadas)
            {
                Console.WriteLine($"Conta: {conta.Nome} ({conta.Banco})");
            }

            if (transacoes.Count == 0)
            {
                Console.WriteLine("Nenhuma transação encontrada no período.");
                return;
            }

            for (int i = 0; i < transacoes.Count; i++)
            {
                Transacao transacao = transacoes[i];

                Conta conta = selecionadas.First(
                    conta => conta.Id == transacao.ContaId);

                Console.WriteLine(
                    $"{i + 1} - {transacao.Data:dd/MM/yyyy} | " +
                    $"{conta.Nome} | {(transacao.TransferenciaId != null ? "Transferência - " : "")}{transacao.Tipo} | " +
                    $"{transacao.Valor:C2} | {transacao.Descricao}");
            }

            decimal entradas = transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Entrada && transacao.TransferenciaId == null)
                .Sum(transacao => transacao.Valor);

            decimal saidas = transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Saida && transacao.TransferenciaId == null)
                .Sum(transacao => transacao.Valor);

            Console.WriteLine($"Total de entradas: {entradas:C2}");
            Console.WriteLine($"Total de saídas: {saidas:C2}");
            Console.WriteLine($"Resultado do período: {entradas - saidas:C2}");

            if (transacoes.Any(transacao => transacao.TransferenciaId != null))
            {
                decimal recebidas = transacoes.Where(t => t.TransferenciaId != null && t.Tipo == TipoTransacao.Entrada).Sum(t => t.Valor);
                decimal enviadas = transacoes.Where(t => t.TransferenciaId != null && t.Tipo == TipoTransacao.Saida).Sum(t => t.Valor);
                Console.WriteLine($"Transferências recebidas: {recebidas:C2}");
                Console.WriteLine($"Transferências enviadas: {enviadas:C2}");
                Console.WriteLine("Transferências internas são mostradas separadamente das receitas e despesas.");
            }

            ConsultarDetalhesDoExtrato(transacoes);
        }
        private  void ConsultarDetalhesDoExtrato(
    List<Transacao> transacoes)
        {
            while (true)
            {
                string texto = LeituraConsole.LerTexto(
                    "Número da transação para ver detalhes (0 para voltar): ");

                if (!int.TryParse(texto, out int numero)
                    || numero < 0
                    || numero > transacoes.Count)
                {
                    Console.WriteLine("Escolha um número da lista.");
                    continue;
                }

                if (numero == 0)
                    return;

                Transacao transacao = transacoes[numero - 1];

                Console.WriteLine("\n=== Detalhes da transação ===");
                MostrarTransacao(transacao);
                Console.WriteLine();
            }
        }
        public void Executar()
        {
            NavegacaoConsole.Executar("Transações", "Voltar",
                ("Registrar transação", RegistrarTransacao),
                ("Listar todas as transações", ListarTransacoes),
                ("Buscar transação por Id", BuscarTransacao),
                ("Listar transações por conta", ListarTransacoesPorConta),
                ("Extrato por período e contas", MostrarExtrato),
                ("Resumo de saídas por categoria", MostrarResumoPorCategoria),
                ("Resumo de saídas por planejamento", MostrarResumoPorPlanejamento),
                ("Resumo de saídas por importância", MostrarResumoPorImportancia),
                ("Resumo de saídas por urgência", MostrarResumoPorUrgencia));
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
            ImportanciaTransacao? importancia = null;
            PlanejamentoTransacao? planejamento = null;
            UrgenciaTransacao? urgencia = null;
            if (tipo == TipoTransacao.Saida)
            {
                importancia = LeituraConsole.LerOpcao<ImportanciaTransacao>();
                planejamento = LeituraConsole.LerOpcao<PlanejamentoTransacao>();
                urgencia = LeituraConsole.LerOpcao<UrgenciaTransacao>();
            }
            Categoria categoria = MenuCategorias.SelecionarCategoria(listaCategorias);
            string descricao = LeituraConsole.LerTexto("Descrição: ");
            DateTime data = LeituraConsole.LerData("Data (dd/MM/aaaa): ");
            string? justificativa = tipo == TipoTransacao.Saida
                ? LeituraConsole.LerTexto("Por que você está realizando esta saída? ") : null;
            Transacao previa = new(conta.Id, tipo, valor, descricao, data, categoria.Id,
                importancia, planejamento, urgencia, justificativa);
            if (!ConfirmacaoConsole.Confirmar("Registrar transação",
                ResumoOperacao.Transacao(previa, conta.Nome, categoria.Nome))) return;

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

        public void MostrarResumoPorCategoria()
        {
            List<Conta> contas = _contas.ListarTodas();

            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            List<Conta> selecionadas = MenuContas.SelecionarContas(contas);

            DateTime inicio = LeituraConsole.LerData("Data inicial: ");
            DateTime fim = LeituraConsole.LerData("Data final: ");

            List<Guid> contaIds = selecionadas
                .Select(conta => conta.Id)
                .ToList();

            Dictionary<Guid, decimal> resumo =
                _relatorios.ResumirSaidasPorCategoria(contaIds, inicio, fim);

            Console.WriteLine(
                $"Saídas por categoria de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy}");

            foreach (Conta conta in selecionadas)
            {
                Console.WriteLine($"Conta: {conta.Nome} ({conta.Banco})");
            }

            if (resumo.Count == 0)
            {
                Console.WriteLine("Nenhuma saída encontrada no período.");
                return;
            }

            List<Categoria> categorias = _categorias.ListarTodas();

            foreach (var item in resumo.OrderByDescending(item => item.Value))
            {
                Categoria? categoria = categorias.FirstOrDefault(
                    categoria => categoria.Id == item.Key);

                Console.WriteLine(
                    $"{categoria?.Nome ?? "Categoria não encontrada"}: {item.Value:C2}");
            }

            Console.WriteLine($"Total de saídas: {resumo.Values.Sum():C2}");
        }

        public void MostrarResumoPorPlanejamento()
        {
            List<Conta> contas = _contas.ListarTodas();

            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            List<Conta> selecionadas = MenuContas.SelecionarContas(contas);
            DateTime inicio = LeituraConsole.LerData("Data inicial: ");
            DateTime fim = LeituraConsole.LerData("Data final: ");

            List<Guid> contaIds = selecionadas
                .Select(conta => conta.Id)
                .ToList();

            Dictionary<string, decimal> resumo =
                _relatorios.ResumirSaidasPorPlanejamento(contaIds, inicio, fim);

            Console.WriteLine(
                $"Saídas por planejamento de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy}");

            foreach (Conta conta in selecionadas)
            {
                Console.WriteLine($"Conta: {conta.Nome} ({conta.Banco})");
            }

            if (resumo.Count == 0)
            {
                Console.WriteLine("Nenhuma saída encontrada no período.");
                return;
            }

            foreach (var item in resumo.OrderByDescending(item => item.Value))
            {
                string planejamento = item.Key == nameof(PlanejamentoTransacao.NaoPlanejada)
                    ? "Não planejada"
                    : item.Key;

                Console.WriteLine($"{planejamento}: {item.Value:C2}");
            }

            Console.WriteLine($"Total de saídas: {resumo.Values.Sum():C2}");
        }

        public void MostrarResumoPorImportancia()
        {
            List<Conta> contas = _contas.ListarTodas();

            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            List<Conta> selecionadas = MenuContas.SelecionarContas(contas);
            DateTime inicio = LeituraConsole.LerData("Data inicial: ");
            DateTime fim = LeituraConsole.LerData("Data final: ");

            List<Guid> contaIds = selecionadas.Select(conta => conta.Id).ToList();
            Dictionary<string, decimal> resumo =
                _relatorios.ResumirSaidasPorImportancia(contaIds, inicio, fim);

            Console.WriteLine(
                $"Saídas por importância de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy}");

            foreach (Conta conta in selecionadas)
            {
                Console.WriteLine($"Conta: {conta.Nome} ({conta.Banco})");
            }

            if (resumo.Count == 0)
            {
                Console.WriteLine("Nenhuma saída encontrada no período.");
                return;
            }

            foreach (var item in resumo.OrderByDescending(item => item.Value))
            {
                string importancia = item.Key == nameof(ImportanciaTransacao.Media)
                    ? "Média"
                    : item.Key;
                Console.WriteLine($"{importancia}: {item.Value:C2}");
            }

            Console.WriteLine($"Total de saídas: {resumo.Values.Sum():C2}");
        }

        public void MostrarResumoPorUrgencia()
        {
            List<Conta> contas = _contas.ListarTodas();

            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta cadastrada.");
                return;
            }

            List<Conta> selecionadas = MenuContas.SelecionarContas(contas);
            DateTime inicio = LeituraConsole.LerData("Data inicial: ");
            DateTime fim = LeituraConsole.LerData("Data final: ");

            List<Guid> contaIds = selecionadas.Select(conta => conta.Id).ToList();
            Dictionary<string, decimal> resumo =
                _relatorios.ResumirSaidasPorUrgencia(contaIds, inicio, fim);

            Console.WriteLine(
                $"Saídas por urgência de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy}");

            foreach (Conta conta in selecionadas)
            {
                Console.WriteLine($"Conta: {conta.Nome} ({conta.Banco})");
            }

            if (resumo.Count == 0)
            {
                Console.WriteLine("Nenhuma saída encontrada no período.");
                return;
            }

            foreach (var item in resumo.OrderByDescending(item => item.Value))
            {
                string urgencia = item.Key == nameof(UrgenciaTransacao.SemUrgencia)
                    ? "Sem urgência"
                    : item.Key;
                Console.WriteLine($"{urgencia}: {item.Value:C2}");
            }

            Console.WriteLine($"Total de saídas: {resumo.Values.Sum():C2}");
        }

        private void MostrarTransacao(Transacao transacao)
        {
            Conta? conta = _contas.ListarTodas()
                .FirstOrDefault(conta => conta.Id == transacao.ContaId);

            Categoria? categoria = _categorias.ListarTodas()
                .FirstOrDefault(categoria => categoria.Id == transacao.CategoriaId);

            Console.WriteLine($"Id: {transacao.Id}");
            Console.WriteLine($"Data: {transacao.Data:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"Conta: {conta?.Nome ?? "Não encontrada"}");
            Console.WriteLine($"Banco: {conta?.Banco ?? "Não encontrado"}");
            Console.WriteLine($"ContaId: {transacao.ContaId}");
            Console.WriteLine($"Tipo: {transacao.Tipo}");
            if (transacao.TransferenciaId is Guid transferenciaId)
                Console.WriteLine($"Transferência interna: {transferenciaId}");
            Console.WriteLine($"Valor: {transacao.Valor:C2}");
            Console.WriteLine($"Categoria: {categoria?.Nome ?? "Não encontrada"}");
            Console.WriteLine($"CategoriaId: {transacao.CategoriaId}");
            Console.WriteLine($"Descrição: {transacao.Descricao}");
            if (transacao.Tipo == TipoTransacao.Entrada && transacao.TransferenciaId is null)
            {
                Console.WriteLine("Avaliação: Não se aplica.");
                return;
            }
            Console.WriteLine($"Importância: {transacao.Importancia}");
            Console.WriteLine(
                $"Planejamento: {transacao.Planejamento?.ToString() ?? "Não informado"}");
            Console.WriteLine(
                $"Urgência: {transacao.Urgencia?.ToString() ?? "Não informada"}");
            Console.WriteLine($"Justificativa: {transacao.Justificativa}");
            Console.WriteLine();
        }
    }
}
