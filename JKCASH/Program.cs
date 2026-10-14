using System.Globalization;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Persistencia;
using JKCASH.Infrastructure.Repositorios;
using Microsoft.Data.SqlClient;
using JKCASH.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
// 1. INICIALIZAÇÃO — executada uma vez ao abrir o programa.
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

IContaRepositorio contas = new ContaRepositorioSql();
ICategoriaRepositorio categorias = new CategoriaRepositorioSql();
ITransacaoRepositorio transacoes = new TransacaoRepositorioSql();
TransacaoServico servico = new(transacoes,categorias);

Console.WriteLine("JKCASH — contas, categorias e transações salvas no SQL.");
TestarConexaoBanco();

// 2. MENU — repete até escolher Sair. Cada opção chama uma função abaixo.
while (true)
{
    MostrarMenu();
    Console.Write("Escolha: ");
    string? opcao = Console.ReadLine();

    if (opcao is null || opcao == "0")
    {
        Console.WriteLine("Programa encerrado.");
        return;
    }

    try
    {
        switch (opcao)
        {
            case "1":
                CadastrarConta(contas);
                break;
            case "2":
                ListarContas(contas);
                break;
            case "3":
                RegistrarTransacao(contas, categorias, servico);
                break;
            case "4":
                ListarTransacoes(transacoes);
                break;
            case "5":
                BuscarTransacao(transacoes);
                break;
            case "6":
                ListarTransacoesPorConta(contas, transacoes);
                break;
            case "7":
                CadastrarCategoria(categorias);
                break;
            case "8":
                ListarCategorias(categorias);
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (ArgumentException erro)
    {
        Console.WriteLine($"Não foi possível concluir: {erro.Message}");
    }
    catch (InvalidOperationException erro)
    {
        Console.WriteLine($"Não foi possível concluir: {erro.Message}");
    }
    catch (EndOfStreamException)
    {
        Console.WriteLine("Entrada encerrada.");
        return;
    }
    catch (DbUpdateException erro) when (
        erro.InnerException is SqlException sqlErro &&
        (sqlErro.Number == 2601 || sqlErro.Number == 2627))
    {
        Console.WriteLine("Já existe uma conta com esse nome e banco, ou uma categoria com esse nome.");
    }
    catch (DbUpdateException erro)
    {
        Console.WriteLine($"Não foi possível salvar no banco: {erro.GetBaseException().Message}");
    }
    catch(SqlException erro)
    {
        Console.WriteLine($"Nao foi possivel acessar o banco: {erro.Message}");
    }
}

static void MostrarMenu()
{
    Console.WriteLine();
    Console.WriteLine("1 - Cadastrar conta");
    Console.WriteLine("2 - Listar contas");
    Console.WriteLine("3 - Registrar transação");
    Console.WriteLine("4 - Listar transações");
    Console.WriteLine("5 - Buscar transação por Id");
    Console.WriteLine("6 - Listar transações por conta");
    Console.WriteLine("7 - Cadastrar categoria");
    Console.WriteLine("8 - Listar categorias");
    Console.WriteLine("0 - Sair");
}

// 3. CONTAS — cadastro e consulta.
static void CadastrarConta(IContaRepositorio repositorio)
{
    string nome = LerTexto("Nome da conta: ");
    string banco = LerTexto("Banco: ");
    TipoConta tipo = LerOpcao<TipoConta>();
    DestinacaoConta destinacao = LerOpcao<DestinacaoConta>();
    decimal saldo = LerDecimal("Saldo inicial (exemplo: 100,50): ");
    decimal limite = LerDecimal("Limite do cheque especial (0 se não houver): ");

    Conta conta = new(nome, banco, tipo, destinacao, saldo, limite);
    repositorio.Adicionar(conta);

    Console.WriteLine($"Conta cadastrada! Id: {conta.Id}");
}

static void ListarContas(IContaRepositorio repositorio)
{
    List<Conta> lista = repositorio.ListarTodas();

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

// Reutilizada no cadastro de transações e na consulta por conta.
static Conta SelecionarConta(List<Conta> lista)
{
    for (int i = 0; i < lista.Count; i++)
    {
        Console.WriteLine($"{i + 1} - {lista[i].Nome} ({lista[i].Banco})");
    }

    int numero = LerNumeroDaLista("Escolha o número da conta: ", lista.Count);
    return lista[numero - 1];
}

// 4. CATEGORIAS — cadastro, consulta e seleção.
static void CadastrarCategoria(ICategoriaRepositorio repositorio)
{
    string nome = LerTexto("Nome da categoria: ");
    Categoria categoria = new(nome);
    repositorio.Adicionar(categoria);

    Console.WriteLine($"Categoria cadastrada: {categoria.Nome}");
}

static void ListarCategorias(ICategoriaRepositorio repositorio)
{
    List<Categoria> lista = repositorio.ListarTodas();

    if (lista.Count == 0)
    {
        Console.WriteLine("Nenhuma categoria cadastrada.");
        return;
    }

    foreach (Categoria categoria in lista)
    {
        Console.WriteLine($"{categoria.Nome} | Id: {categoria.Id}");
    }
}

static Categoria SelecionarCategoria(List<Categoria> lista)
{
    for (int i = 0; i < lista.Count; i++)
    {
        Console.WriteLine($"{i + 1} - {lista[i].Nome}");
    }

    int numero = LerNumeroDaLista("Escolha o número da categoria: ", lista.Count);
    return lista[numero - 1];
}

// 5. TRANSAÇÕES — o menu coleta dados; o serviço registra e movimenta o saldo.
static void RegistrarTransacao(
    IContaRepositorio contas,
    ICategoriaRepositorio categorias,
    TransacaoServico servico)
{
    List<Conta> listaContas = contas.ListarTodas();
    List<Categoria> listaCategorias = categorias.ListarTodas();

    if (listaContas.Count == 0)
    {
        Console.WriteLine("Cadastre uma conta primeiro pela opção 1.");
        return;
    }

    if (listaCategorias.Count == 0)
    {
        Console.WriteLine("Cadastre uma categoria primeiro pela opção 7.");
        return;
    }

    Conta conta = SelecionarConta(listaContas);
    TipoTransacao tipo = LerOpcao<TipoTransacao>();
    decimal valor = LerDecimal("Valor (exemplo: 30,50): ");

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
    ImportanciaTransacao importancia = LerOpcao<ImportanciaTransacao>();
    PlanejamentoTransacao planejamento = LerOpcao<PlanejamentoTransacao>();
    UrgenciaTransacao urgencia = LerOpcao<UrgenciaTransacao>();
    Categoria categoria = SelecionarCategoria(listaCategorias);
    string descricao = LerTexto("Descrição: ");
    DateTime data = LerData("Data (dd/MM/aaaa): ");
    string justificativa = LerTexto(
    "Por que você está realizando esta transação? ");

    Transacao transacao = servico.Registrar(
        conta, tipo, valor, descricao, data, categoria.Id,importancia,planejamento,urgencia,justificativa);

    Console.WriteLine($"Transação registrada! Id: {transacao.Id}");
    Console.WriteLine($"Saldo atualizado: {conta.Saldo.SaldoAtual:C2}");
}

static void ListarTransacoes(ITransacaoRepositorio repositorio)
{
    List<Transacao> lista = repositorio.ListarTodas();
    Console.WriteLine($"Transações cadastradas: {lista.Count}");

    foreach (Transacao transacao in lista)
    {
        MostrarTransacao(transacao);
    }
}

static void BuscarTransacao(ITransacaoRepositorio repositorio)
{
    string texto = LerTexto("Id da transação: ");

    if (!Guid.TryParse(texto, out Guid id))
    {
        throw new ArgumentException("O identificador digitado não é um Guid válido.");
    }

    Transacao? transacao = repositorio.BuscarPorId(id);

    if (transacao is null)
    {
        Console.WriteLine("Transação não encontrada.");
        return;
    }

    MostrarTransacao(transacao);
}

static void ListarTransacoesPorConta(
    IContaRepositorio contas,
    ITransacaoRepositorio transacoes)
{
    List<Conta> lista = contas.ListarTodas();

    if (lista.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada.");
        return;
    }

    Conta conta = SelecionarConta(lista);
    List<Transacao> resultado = transacoes.ListarPorConta(conta.Id);

    Console.WriteLine($"Transações de {conta.Nome}: {resultado.Count}");

    foreach (Transacao transacao in resultado)
    {
        MostrarTransacao(transacao);
    }
}

static void MostrarTransacao(Transacao transacao)
{
    Console.WriteLine(
        $"{transacao.Data:dd/MM/yyyy} | {transacao.Tipo} | " +
        $"CategoriaId: {transacao.CategoriaId} | " +
        $"{transacao.Valor:C2} | {transacao.Descricao}");
    Console.WriteLine($"Id: {transacao.Id} | ContaId: {transacao.ContaId} | Importancia {transacao.Importancia} | Planejada? {transacao.Planejamento} | Urgencia: {transacao.Urgencia} Justificativa: {transacao.Justificativa}");
}

// 6. LEITURA — funções auxiliares usadas pelas opções do menu.
static string LerTexto(string mensagem)
{
    Console.Write(mensagem);
    return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
}

static decimal LerDecimal(string mensagem)
{
    string texto = LerTexto(mensagem);

    if (!decimal.TryParse(texto,
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
        CultureInfo.CurrentCulture, out decimal valor))
    {
        throw new ArgumentException("Digite um número válido usando vírgula para os centavos.");
    }

    return valor;
}

static DateTime LerData(string mensagem)
{
    string texto = LerTexto(mensagem);

    if (!DateTime.TryParseExact(texto, "dd/MM/yyyy",
        CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime data))
    {
        throw new ArgumentException("Informe uma data válida no formato dd/MM/aaaa.");
    }

    return data;
}

static int LerNumeroDaLista(string mensagem, int quantidade)
{
    string texto = LerTexto(mensagem);

    if (!int.TryParse(texto, out int numero) || numero < 1 || numero > quantidade)
    {
        throw new ArgumentException("Escolha um número da lista.");
    }

    return numero;
}

// T é o enum escolhido na chamada, por exemplo LerOpcao<TipoTransacao>().
static T LerOpcao<T>() where T : struct, Enum
{
    Console.WriteLine($"Opções de {typeof(T).Name}:");

    foreach (T opcao in Enum.GetValues<T>())
    {
        Console.WriteLine($"{Convert.ToInt32(opcao)} - {opcao}");
    }

    if (!int.TryParse(LerTexto("Escolha o número: "), out int numero))
    {
        throw new ArgumentException("Digite o número de uma opção.");
    }

    T escolha = (T)Enum.ToObject(typeof(T), numero);

    if (!Enum.IsDefined(escolha))
    {
        throw new ArgumentException("Escolha uma opção da lista.");
    }

    return escolha;
}

// 7. BANCO — verifica a conexão usada pelos repositórios SQL.
static void TestarConexaoBanco()
{
    try
    {
        ConexaoSql.Testar();
        Console.WriteLine("Conexão com JKCASHDb realizada!");
    }
    catch (SqlException erro)
    {
        Console.WriteLine($"Falha na conexão: {erro.Message}");
    }
}
