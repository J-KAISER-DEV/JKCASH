using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class ExclusaoRepositorioEmMemoria(
    ContaRepositorioEmMemoria contas, CategoriaRepositorioEmMemoria categorias,
    TransacaoRepositoEmMemoria transacoes, CartaoCreditoRepositorioEmMemoria cartoes,
    CompraCartaoRepositorio compras) : IExclusaoRepositorio
{
    public EstadoFinanceiro Carregar()
    {
        var listaCartoes = cartoes.ListarTodos();
        return new(contas.ListarTodas(), categorias.ListarTodas(), transacoes.ListarTodas(), listaCartoes,
            listaCartoes.SelectMany(c => compras.ListarPorCartao(c.Id)).ToList(),
            listaCartoes.SelectMany(c => ((IFaturaCartaoRepositorio)compras).ListarPorCartao(c.Id)).ToList());
    }

    public void Salvar(EstadoFinanceiro atualizado, string versaoOriginal)
    {
        if (Carregar().Assinatura() != versaoOriginal)
            throw new InvalidOperationException("Os dados foram alterados. Consulte a prévia novamente.");

        // Prepara todas as coleções antes de substituir o armazenamento do Console.
        var novasContas = atualizado.Contas.ToDictionary(c => c.Id);
        var novasCategorias = atualizado.Categorias.ToDictionary(c => c.Id);
        var novasTransacoes = atualizado.Transacoes.ToDictionary(t => t.Id);
        var novosCartoes = atualizado.Cartoes.ToDictionary(c => c.Id);
        var novasCompras = atualizado.Compras.ToDictionary(c => c.Id);
        var novasFaturas = atualizado.Faturas.ToDictionary(f => f.Id);
        contas.Substituir(novasContas);
        categorias.Substituir(novasCategorias);
        transacoes.Substituir(novasTransacoes);
        cartoes.Substituir(novosCartoes);
        compras.Substituir(novasCompras, novasFaturas);
    }
}
