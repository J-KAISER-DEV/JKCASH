using System.Globalization;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Application.Servicos;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
// Uma única instância de cada repositório mantém os dados entre as opções do menu.
ContaRepositorioEmMemoria contas = new();
TransacaoRepositoEmMemoria transacoes = new();
TransacaoServico servico = new(transacoes);
Console.WriteLine("JKCASH — dados em memória, apagados ao encerrar.");
Conta contaTeste = new(
    nome: "Conta de teste",
    banco: "Banco de teste",
    tipo: TipoConta.Corrente,
    saldoInicial: 100m
);
static void ListarTransacoesPorConta(
    ContaRepositorioEmMemoria contas,
    TransacaoRepositoEmMemoria transacoes)
{
    List<Conta> lista = contas.ListarTodas();

    if (lista.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada.");
        return;
    }

    for (int i = 0; i < lista.Count; i++)
    {
        Console.WriteLine($"{i + 1} - {lista[i].Nome}");
    }

    if (!int.TryParse(
        LerTexto("Escolha o número da conta: "),
        out int numero)
        || numero < 1
        || numero > lista.Count)
    {
        throw new ArgumentException("Escolha uma conta da lista.");
    }

    Conta conta = lista[numero - 1];

    List<Transacao> resultado =
        transacoes.ListarPorConta(conta.Id);

    Console.WriteLine(
        $"Transações de {conta.Nome}: {resultado.Count}");

    foreach (Transacao transacao in resultado)
    {
        MostrarTransacao(transacao);
    }
}
contaTeste.Creditar(50m);
contaTeste.Debitar(30m);

Console.WriteLine($"Saldo do teste: {contaTeste.Saldo.SaldoAtual:C2}");
// while repete o menu. return encerra o programa.
while (true)
{
    Console.WriteLine("\n1 - Cadastrar conta\n2 - Listar contas\n3 - Registrar transação\n4 - Listar transações\n5 - Buscar transação por Id\n0 - Sair");
    Console.Write("Escolha: ");
    string? opcao = Console.ReadLine();
    if (opcao is null || opcao == "0")
    {
        Console.WriteLine("Programa encerrado.");
        return;
    }
    try
    {
        // switch escolhe a ação; break termina cada opção.
        switch (opcao)
        {
            case "1": CadastrarConta(contas); break;
            case "2": ListarContas(contas); break;
            case "3": RegistrarTransacao(contas, servico); break;
            case "4":
                List<Transacao> lista = transacoes.ListarTodas();
                Console.WriteLine($"Transações cadastradas: {lista.Count}");
                foreach (Transacao item in lista) MostrarTransacao(item);
                break;
            case "5": BuscarTransacao(transacoes); break;
            case "6": ListarTransacoesPorConta(contas,transacoes); break;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (ArgumentException erro)
    {
        Console.WriteLine($"Não foi possível concluir: {erro.Message}");
    }
    catch (EndOfStreamException)
    {
        Console.WriteLine("Entrada encerrada.");
        return;
    }
    catch(InvalidOperationException erro)
    {
        Console.WriteLine($"Nao foi possivel concluir erro:{erro.Message}");
    }
    
}

static void CadastrarConta(ContaRepositorioEmMemoria repositorio)
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

static void ListarContas(ContaRepositorioEmMemoria repositorio)
{
    List<Conta> lista = repositorio.ListarTodas();
    if (lista.Count == 0) Console.WriteLine("Nenhuma conta cadastrada.");
    foreach (Conta conta in lista)
    {
        Console.WriteLine($"{conta.Nome} | {conta.Banco} | Saldo: {conta.Saldo.SaldoAtual:C2}");
        Console.WriteLine($"Id: {conta.Id}");
    }
}

static void RegistrarTransacao(ContaRepositorioEmMemoria contas, TransacaoServico servico)
{
    List<Conta> lista = contas.ListarTodas();
    if (lista.Count == 0)
    {
        Console.WriteLine("Cadastre uma conta primeiro pela opção 1.");
        return;
    }
    for (int i = 0; i < lista.Count; i++)
        Console.WriteLine($"{i + 1} - {lista[i].Nome} ({lista[i].Banco})");

    if (!int.TryParse(LerTexto("Escolha o número da conta: "), out int numero)
        || numero < 1 || numero > lista.Count)
        throw new ArgumentException("Escolha uma conta da lista.");

    // Listas começam na posição zero; as opções exibidas começam em um.
    Conta conta = lista[numero - 1];
    TipoTransacao tipo = LerOpcao<TipoTransacao>();
    decimal valor = LerDecimal("Valor (exemplo: 30,50): ");
    if(valor <= 0m || valor != decimal.Round(valor,2))
    {
        throw new ArgumentException(
            "O valor deve ser positivo e ter no maximo duas casas decimais.");
    }
    if (tipo == TipoTransacao.Saida)
    {
        conta.ValidarSaida(valor);
    }
    CategoriaTransacao categoria = LerOpcao<CategoriaTransacao>();
    string descricao = LerTexto("Descrição: ");
    if (!DateTime.TryParseExact(LerTexto("Data (dd/MM/aaaa): "), "dd/MM/yyyy",
        CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime data))
        throw new ArgumentException("Informe uma data válida no formato dd/MM/aaaa.");

    // O construtor valida os dados antes de o repositório receber o objeto.
    Transacao transacao = servico.Registrar(
        conta, tipo, valor, descricao, data, categoria);
    Console.WriteLine($"Transação registrada! Id: {transacao.Id}");
    Console.WriteLine($"Saldo atualizado: {conta.Saldo.SaldoAtual:C2}");
}

static void BuscarTransacao(TransacaoRepositoEmMemoria repositorio)
{
    if (!Guid.TryParse(LerTexto("Id da transação: "), out Guid id))
        throw new ArgumentException("O identificador digitado não é um Guid válido.");
    Transacao? transacao = repositorio.BuscarPorId(id);
    if (transacao is null) Console.WriteLine("Transação não encontrada.");
    else MostrarTransacao(transacao);
}

static void MostrarTransacao(Transacao transacao)
{
    Console.WriteLine($"{transacao.Data:dd/MM/yyyy} | {transacao.Tipo} | {transacao.Categoria} | {transacao.Valor:C2} | {transacao.Descricao}");
    Console.WriteLine($"Id: {transacao.Id} | ContaId: {transacao.ContaId}");
}

static string LerTexto(string mensagem)
{
    Console.Write(mensagem);
    return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
}

static decimal LerDecimal(string mensagem)
{
    if (!decimal.TryParse(LerTexto(mensagem),
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
        CultureInfo.CurrentCulture, out decimal valor))
        throw new ArgumentException("Digite um número válido usando vírgula para os centavos.");
    return valor;
}

// T permite usar a mesma função para os diferentes enums.
static T LerOpcao<T>() where T : struct, Enum
{
    Console.WriteLine($"Opções de {typeof(T).Name}:");
    foreach (T opcao in Enum.GetValues<T>())
        Console.WriteLine($"{Convert.ToInt32(opcao)} - {opcao}");
    if (!int.TryParse(LerTexto("Escolha o número: "), out int numero))
        throw new ArgumentException("Digite o número de uma opção.");
    T escolha = (T)Enum.ToObject(typeof(T), numero);
    if (!Enum.IsDefined(escolha)) throw new ArgumentException("Escolha uma opção da lista.");
    return escolha;
}
