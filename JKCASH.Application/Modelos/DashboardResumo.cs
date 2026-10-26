using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

// Dados de leitura: não transportam entidades nem contextos de banco até a tela.
public sealed record DashboardConta(Guid Id, string Nome, string Banco, bool Ativa);
public sealed record DashboardCategoria(Guid Id, string Nome, decimal Valor);
public sealed record DashboardMovimentacao(DateTime Data, string Descricao, string Conta,
    string Categoria, TipoTransacao Tipo, decimal Valor, bool Transferencia);

public sealed record DashboardResumo(
    IReadOnlyList<DashboardConta> Contas,
    Guid? ContaId,
    DateTime Inicio,
    DateTime FimExclusivo,
    decimal SaldoAtual,
    decimal Receitas,
    decimal Despesas,
    decimal FaturasPendentes,
    int QuantidadeCartoes,
    decimal LimiteTotal,
    decimal LimiteDisponivel,
    IReadOnlyList<DashboardCategoria> SaidasPorCategoria,
    IReadOnlyList<DashboardMovimentacao> Movimentacoes);
