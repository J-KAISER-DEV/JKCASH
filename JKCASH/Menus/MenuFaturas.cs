using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;

namespace JKCASH.Menus;

public sealed class MenuFaturas
{
    private readonly ICartaoCreditoRepositorio _cartoes;
    private readonly IFaturaCartaoRepositorio _faturas;
    private readonly ICompraCartaoRepositorio _compras;
    private readonly IContaRepositorio _contas;
    private readonly ICategoriaRepositorio _categorias;
    private readonly PagamentoFaturaServico _pagamentos;

    public MenuFaturas(ICartaoCreditoRepositorio cartoes, IFaturaCartaoRepositorio faturas,
        ICompraCartaoRepositorio compras, IContaRepositorio contas, ICategoriaRepositorio categorias,
        PagamentoFaturaServico pagamentos)
    {
        ArgumentNullException.ThrowIfNull(cartoes);
        ArgumentNullException.ThrowIfNull(faturas);
        ArgumentNullException.ThrowIfNull(compras);
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(pagamentos);
        _cartoes = cartoes;
        _faturas = faturas;
        _compras = compras;
        _contas = contas;
        _categorias = categorias;
        _pagamentos = pagamentos;
    }

    public void Executar()
    {
        NavegacaoConsole.Executar("Faturas", "Voltar",
            ("Listar faturas de um cartão", ListarFaturas),
            ("Consultar parcelas e pagamentos", ConsultarFatura),
            ("Registrar pagamento total ou parcial", PagarFatura));
    }

    public void ListarFaturas()
    {
        List<FaturaCartao> faturas = FaturasDoCartao();
        foreach (FaturaCartao fatura in faturas) MostrarResumo(fatura);
    }

    public void ConsultarFatura()
    {
        FaturaCartao? fatura = SelecionarFatura();
        if (fatura is null) return;
        MostrarResumo(fatura);
        Console.WriteLine("Parcelas da fatura:");
        foreach (ParcelaCartao parcela in fatura.Parcelas)
        {
            CompraCartao? compra = _compras.BuscarPorId(parcela.CompraId);
            Console.WriteLine($"{compra?.Descricao} | Compra: {parcela.CompraId} | {parcela.Numero}/{parcela.Quantidade} | {parcela.Valor:C2}");
        }
        Console.WriteLine($"Pagamentos confirmados: {fatura.Pagamentos.Count}");
        Dictionary<Guid, Conta> contas = _contas.ListarTodas().ToDictionary(conta => conta.Id);
        foreach (PagamentoFatura pagamento in fatura.Pagamentos)
        {
            string conta = contas.TryGetValue(pagamento.ContaId, out Conta? encontrada)
                ? $"{encontrada.Nome} ({encontrada.Banco})" : pagamento.ContaId.ToString();
            Console.WriteLine($"{pagamento.Data:dd/MM/yyyy} | {pagamento.Valor:C2} | Conta: {conta}");
            Console.WriteLine($"PagamentoId: {pagamento.Id} | TransacaoId: {pagamento.TransacaoId}");
        }
    }

    public void PagarFatura()
    {
        FaturaCartao? fatura = SelecionarFatura();
        if (fatura is null) return;
        if (fatura.SaldoPendente == 0m)
        {
            Console.WriteLine("Esta fatura já está quitada.");
            return;
        }
        List<Conta> contas = _contas.ListarTodas();
        List<Categoria> categorias = _categorias.ListarTodas();
        if (contas.Count == 0 || categorias.Count == 0)
        {
            Console.WriteLine("Cadastre uma conta e uma categoria antes de registrar o pagamento.");
            return;
        }
        Console.WriteLine("Selecione a conta usada para pagar (pode ser de outro banco):");
        Conta conta = MenuContas.SelecionarConta(contas);
        Console.WriteLine("Selecione a categoria da saída na conta:");
        Categoria categoria = MenuCategorias.SelecionarCategoria(categorias);
        Console.WriteLine("Registre somente um pagamento já confirmado, com data de hoje.");
        decimal valor = LeituraConsole.LerDecimal($"Valor pago (pendente: {fatura.SaldoPendente:C2}): ");
        PagamentoFatura pagamento = _pagamentos.Registrar(fatura.Id, conta.Id, valor, categoria.Id);
        Console.WriteLine($"Pagamento confirmado! Id: {pagamento.Id}");
        // Recarrega o saldo, pois no modo SQL o serviço usa outra instância da conta.
        Conta atualizada = _contas.ListarTodas().Single(item => item.Id == conta.Id);
        Console.WriteLine($"Saldo da conta: {atualizada.Saldo.SaldoAtual:C2}");
        Console.WriteLine($"Limite disponível: {_cartoes.BuscarPorId(fatura.CartaoId)!.LimiteDisponivel:C2}");
        MostrarResumo(_faturas.BuscarPorId(fatura.Id)!);
    }

    private List<FaturaCartao> FaturasDoCartao()
    {
        List<CartaoCredito> cartoes = _cartoes.ListarTodos();
        if (cartoes.Count == 0)
        {
            Console.WriteLine("Nenhum cartão cadastrado.");
            return new();
        }
        CartaoCredito cartao = MenuCartao.SelecionarCartao(cartoes);
        List<FaturaCartao> faturas = _faturas.ListarPorCartao(cartao.Id);
        if (faturas.Count == 0) Console.WriteLine("Nenhuma fatura gerada. Use Cartões > Registrar compra à vista ou parcelada.");
        return faturas;
    }

    private FaturaCartao? SelecionarFatura()
    {
        List<FaturaCartao> faturas = FaturasDoCartao();
        if (faturas.Count == 0) return null;
        for (int i = 0; i < faturas.Count; i++)
        {
            Console.Write($"{i + 1} - ");
            MostrarResumo(faturas[i]);
        }
        int numero = LeituraConsole.LerNumeroDaLista("Escolha o número da fatura: ", faturas.Count);
        return faturas[numero - 1];
    }

    private static void MostrarResumo(FaturaCartao fatura)
    {
        DateTime hoje = DateTime.Today;
        Console.WriteLine($"Fatura {fatura.Id} | Fechamento: {fatura.Ciclo.Fechamento:dd/MM/yyyy} | Vencimento: {fatura.Ciclo.Vencimento:dd/MM/yyyy}");
        Console.WriteLine($"Total: {fatura.Total:C2} | Pago: {fatura.TotalPago:C2} | Pendente: {fatura.SaldoPendente:C2}");
        Console.WriteLine($"Situação: {fatura.SituacaoEm(hoje)} | Pagamento: {fatura.SituacaoPagamento}" +
            (fatura.EstaAtrasadaEm(hoje) ? " | ATRASADA (sem cálculo de juros ou multas)" : ""));
    }
}
