using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace JKCASH.Infrastructure.Consultas;

public sealed class DashboardConsulta : IDashboardConsulta
{
    private readonly Func<JKCASHDbContext> _criarContexto;

    public DashboardConsulta(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }

    public DashboardResumo Consultar(Guid? contaId, DateTime mes)
    {
        DateTime inicio = new(mes.Year, mes.Month, 1);
        if (inicio.Year == 9999 && inicio.Month == 12)
            throw new ArgumentOutOfRangeException(nameof(mes), "Selecione um mês anterior a dezembro de 9999.");
        DateTime fimExclusivo = inicio.AddMonths(1);
        using JKCASHDbContext contexto = _criarContexto();
        // Todos os indicadores devem representar a mesma leitura consistente do banco.
        contexto.Database.OpenConnection();
        using DbTransaction leitura = contexto.Database.IsSqlite()
            ? ((SqliteConnection)contexto.Database.GetDbConnection()).BeginTransaction(IsolationLevel.Serializable, deferred: true)
            : contexto.Database.GetDbConnection().BeginTransaction(IsolationLevel.Serializable);
        using var vinculo = contexto.Database.UseTransaction(leitura);
        bool centavos = contexto.Database.IsSqlite();

        var contas = contexto.Contas.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id)
            .Select(c => new DashboardConta(c.Id, c.Nome, c.Banco, c.Ativa)).ToList();
        // Preserva o comportamento anterior se uma conta selecionada tiver sido excluída.
        Guid? filtro = contas.Any(c => c.Id == contaId) ? contaId : null;
        var selecionadas = contexto.Contas.AsNoTracking().Where(c => !filtro.HasValue || c.Id == filtro);
        var ids = selecionadas.Select(c => c.Id);
        var transacoes = contexto.Transacoes.AsNoTracking().Where(t => ids.Contains(t.ContaId)
            && t.Data >= inicio && t.Data < fimExclusivo);
        var comuns = transacoes.Where(t => t.TransferenciaId == null);
        decimal saldo = Somar(selecionadas, c => c.Saldo.SaldoAtual, centavos);

        // No SQLite, o cast seleciona os centavos INTEGER da coluna, não reais em double.
        var totais = centavos
            ? comuns.GroupBy(t => t.Tipo).Select(g => new { Tipo = g.Key, Valor = g.Sum(t => (long)t.Valor) })
                .ToList().ToDictionary(g => g.Tipo, g => g.Valor / 100m)
            : comuns.GroupBy(t => t.Tipo).Select(g => new { Tipo = g.Key, Valor = g.Sum(t => t.Valor) })
                .ToList().ToDictionary(g => g.Tipo, g => g.Valor);

        var saidas = comuns.Where(t => t.Tipo == TipoTransacao.Saida);
        var grupos = centavos
            ? saidas.GroupBy(t => t.CategoriaId).Select(g => new { Id = g.Key, Valor = g.Sum(t => (long)t.Valor) })
                .ToList().Select(g => new { g.Id, Valor = g.Valor / 100m }).ToList()
            : saidas.GroupBy(t => t.CategoriaId).Select(g => new { Id = g.Key, Valor = g.Sum(t => t.Valor) }).ToList();
        var idsCategorias = grupos.Select(g => g.Id).ToArray();
        var nomesCategorias = contexto.Categorias.AsNoTracking().Where(c => idsCategorias.Contains(c.Id))
            .Select(c => new { c.Id, c.Nome }).ToDictionary(c => c.Id, c => c.Nome);
        var categorias = grupos.Select(g => new DashboardCategoria(g.Id,
                nomesCategorias.GetValueOrDefault(g.Id, "Categoria não encontrada"), g.Valor))
            .OrderByDescending(g => g.Valor).ThenBy(g => g.Nome).ThenBy(g => g.Id).ToList();

        var cartoes = contexto.CartoesCredito.AsNoTracking().Where(c => ids.Contains(c.ContaId));
        var credito = centavos
            ? cartoes.GroupBy(c => 1).Select(g => new
                { Quantidade = g.Count(), Limite = g.Sum(c => (long)c.Limite), Utilizado = g.Sum(c => (long)c.ValorUtilizado) })
                .ToList().Select(g => new { g.Quantidade, Limite = g.Limite / 100m, Utilizado = g.Utilizado / 100m }).SingleOrDefault()
            : cartoes.GroupBy(c => 1).Select(g => new
                { Quantidade = g.Count(), Limite = g.Sum(c => c.Limite), Utilizado = g.Sum(c => c.ValorUtilizado) }).SingleOrDefault();

        var faturas = contexto.FaturasCartao.AsNoTracking().Where(f => cartoes.Select(c => c.Id).Contains(f.CartaoId)
            && f.Ciclo.Vencimento >= inicio && f.Ciclo.Vencimento < fimExclusivo).Select(f => f.Id);
        // Somar separadamente evita multiplicar parcelas ao juntar vários pagamentos da fatura.
        decimal parcelas = Somar(contexto.ParcelasCartao.Where(p => faturas.Contains(p.FaturaId)), p => p.Valor, centavos);
        decimal pagamentos = Somar(contexto.PagamentosFatura.Where(p => faturas.Contains(p.FaturaId)), p => p.Valor, centavos);

        var movimentacoes = (from t in transacoes.OrderByDescending(t => t.Data).ThenByDescending(t => t.Id).Take(8)
            join c in contexto.Contas on t.ContaId equals c.Id
            join categoria in contexto.Categorias on t.CategoriaId equals categoria.Id into categoriasMovimento
            from categoria in categoriasMovimento.DefaultIfEmpty()
            orderby t.Data descending, t.Id descending
            select new DashboardMovimentacao(t.Data, t.Descricao, c.Nome,
                categoria == null ? "Categoria não encontrada" : categoria.Nome,
                t.Tipo, t.Valor, t.TransferenciaId.HasValue)).ToList();

        leitura.Commit();
        return new DashboardResumo(contas, filtro, inicio, fimExclusivo, saldo,
            totais.GetValueOrDefault(TipoTransacao.Entrada), totais.GetValueOrDefault(TipoTransacao.Saida),
            parcelas - pagamentos, credito?.Quantidade ?? 0, credito?.Limite ?? 0m,
            (credito?.Limite ?? 0m) - (credito?.Utilizado ?? 0m), categorias, movimentacoes);
    }

    private static decimal Somar<T>(IQueryable<T> consulta, Expression<Func<T, decimal>> campo, bool centavos)
    {
        if (!centavos) return consulta.Sum(campo);
        var seletor = Expression.Lambda<Func<T, long>>(Expression.Convert(campo.Body, typeof(long)), campo.Parameters);
        return consulta.Sum(seletor) / 100m;
    }
}
