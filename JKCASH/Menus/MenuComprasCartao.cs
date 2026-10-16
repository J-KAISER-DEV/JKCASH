using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Menus;

public sealed class MenuComprasCartao
{
    private readonly ICartaoCreditoRepositorio _cartoes;
    private readonly ICategoriaRepositorio _categorias;
    private readonly ICompraCartaoRepositorio _compras;
    private readonly CompraCartaoServico _servico;

    public MenuComprasCartao(ICartaoCreditoRepositorio cartoes, ICategoriaRepositorio categorias,
        ICompraCartaoRepositorio compras, CompraCartaoServico servico)
    {
        _cartoes = cartoes;
        _categorias = categorias;
        _compras = compras;
        _servico = servico;
    }

    public void RegistrarCompra()
    {
        List<CartaoCredito> cartoes = _cartoes.ListarTodos();
        if (cartoes.Count == 0)
        {
            Console.WriteLine("Cadastre um cartão primeiro pela opção 9.");
            return;
        }
        List<Categoria> categorias = _categorias.ListarTodas();
        if (categorias.Count == 0)
        {
            Console.WriteLine("Cadastre uma categoria primeiro pela opção 7.");
            return;
        }

        CartaoCredito cartao = MenuCartao.SelecionarCartao(cartoes);
        decimal valor = LeituraConsole.LerDecimal("Valor TOTAL da compra: ");
        int quantidade = LeituraConsole.LerInteiro("Quantidade de parcelas (1 para à vista, até 120): ");
        string descricao = LeituraConsole.LerTexto("Descrição: ");
        DateTime data = LeituraConsole.LerData("Data da compra (dd/MM/aaaa): ");
        Categoria categoria = MenuCategorias.SelecionarCategoria(categorias);
        ImportanciaTransacao importancia = LeituraConsole.LerOpcao<ImportanciaTransacao>();
        PlanejamentoTransacao planejamento = LeituraConsole.LerOpcao<PlanejamentoTransacao>();
        UrgenciaTransacao urgencia = LeituraConsole.LerOpcao<UrgenciaTransacao>();
        string justificativa = LeituraConsole.LerTexto("Justificativa: ");

        CompraCartao compra = _servico.Registrar(cartao.Id, valor, descricao, data, categoria.Id,
            importancia, planejamento, urgencia, justificativa, quantidade);
        Console.WriteLine("Compra registrada! O valor total comprometeu o limite do cartão.");
        MostrarCompra(compra);
        Console.WriteLine($"Limite disponível: {cartao.LimiteDisponivel:C2}");
    }

    public void ListarCompras()
    {
        List<CartaoCredito> cartoes = _cartoes.ListarTodos();
        if (cartoes.Count == 0)
        {
            Console.WriteLine("Nenhum cartão cadastrado.");
            return;
        }
        CartaoCredito cartao = MenuCartao.SelecionarCartao(cartoes);
        List<CompraCartao> compras = _compras.ListarPorCartao(cartao.Id);
        Console.WriteLine($"Compras de {cartao.Nome}: {compras.Count}");
        foreach (CompraCartao compra in compras) MostrarCompra(compra);
    }

    public void BuscarCompra()
    {
        string texto = LeituraConsole.LerTexto("Id da compra: ");
        if (!Guid.TryParse(texto, out Guid id))
            throw new ArgumentException("Informe um Id de compra válido.");
        CompraCartao? compra = _compras.BuscarPorId(id);
        if (compra is null) Console.WriteLine("Compra não encontrada.");
        else MostrarCompra(compra);
    }

    private void MostrarCompra(CompraCartao compra)
    {
        string categoria = _categorias.ListarTodas().FirstOrDefault(item => item.Id == compra.CategoriaId)?.Nome
            ?? compra.CategoriaId.ToString();
        Console.WriteLine($"{compra.Data:dd/MM/yyyy} | {compra.Descricao} | Total: {compra.Valor:C2} | {compra.QuantidadeParcelas} parcela(s)");
        Console.WriteLine($"Id: {compra.Id} | Categoria: {categoria} | CartãoId: {compra.CartaoId}");
        Console.WriteLine($"{compra.Importancia} | {compra.Planejamento} | {compra.Urgencia} | {compra.Justificativa}");
        foreach (ParcelaCartao parcela in compra.Parcelas)
            Console.WriteLine($"  {parcela.Numero}/{parcela.Quantidade}: {parcela.Valor:C2} | Fechamento: {parcela.Ciclo.Fechamento:dd/MM/yyyy} | Vencimento: {parcela.Ciclo.Vencimento:dd/MM/yyyy}");
    }
}
