using System.Data;
using System.Data.Common;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Consultas;

public sealed class ExtratoConsulta : IExtratoConsulta
{
    private readonly Func<JKCASHDbContext> _criarContexto;

    public ExtratoConsulta(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }

    public ExtratoPagina Consultar(IReadOnlyCollection<Guid> contaIds, DateTime dataInicial,
        DateTime dataFinal, int pagina = 1, int tamanhoPagina = 50)
    {
        ArgumentNullException.ThrowIfNull(contaIds);
        if (contaIds.Count == 0) throw new ArgumentException("Selecione pelo menos uma conta.", nameof(contaIds));
        if (dataInicial.Date > dataFinal.Date)
            throw new ArgumentException("A data inicial não pode ser posterior à data final.");
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
        if (tamanhoPagina is < 1 or > 200) throw new ArgumentOutOfRangeException(nameof(tamanhoPagina));

        Guid[] ids = contaIds.Distinct().ToArray();
        DateTime inicio = dataInicial.Date;
        using JKCASHDbContext contexto = _criarContexto();
        // Página, quantidade e totais representam a mesma leitura.
        contexto.Database.OpenConnection();
        using DbTransaction leitura = contexto.Database.IsSqlite()
            ? ((SqliteConnection)contexto.Database.GetDbConnection()).BeginTransaction(IsolationLevel.Serializable, deferred: true)
            : contexto.Database.GetDbConnection().BeginTransaction(IsolationLevel.Serializable);
        using var vinculo = contexto.Database.UseTransaction(leitura);

        var filtradas = contexto.Transacoes.AsNoTracking()
            .Where(t => ids.Contains(t.ContaId) && t.Data >= inicio);
        // Não aplica uma função sobre a coluna Data e inclui todo o último dia.
        if (dataFinal.Date < DateTime.MaxValue.Date)
        {
            DateTime fimExclusivo = dataFinal.Date.AddDays(1);
            filtradas = filtradas.Where(t => t.Data < fimExclusivo);
        }

        var grupos = filtradas.GroupBy(t => new { t.Tipo, Transferencia = t.TransferenciaId != null });
        // SQLite armazena dinheiro em centavos INTEGER. A soma permanece inteira no banco.
        var totais = contexto.Database.IsSqlite()
            ? grupos.Select(g => new { g.Key.Tipo, g.Key.Transferencia, Quantidade = g.Count(), Valor = g.Sum(t => (long)t.Valor) })
                .ToList().Select(g => new Grupo(g.Tipo, g.Transferencia, g.Quantidade, g.Valor / 100m)).ToList()
            : grupos.Select(g => new { g.Key.Tipo, g.Key.Transferencia, Quantidade = g.Count(), Valor = g.Sum(t => t.Valor) })
                .ToList().Select(g => new Grupo(g.Tipo, g.Transferencia, g.Quantidade, g.Valor)).ToList();
        int quantidade = totais.Sum(g => g.Quantidade);
        int totalPaginas = quantidade == 0 ? 1 : (quantidade - 1) / tamanhoPagina + 1;
        int paginaAtual = Math.Min(pagina, totalPaginas);
        var paginaConsulta = filtradas.OrderBy(t => t.Data).ThenBy(t => t.Id)
            .Skip((paginaAtual - 1) * tamanhoPagina).Take(tamanhoPagina);
        var itens = (from t in paginaConsulta
            join c in contexto.Contas on t.ContaId equals c.Id into contas
            from c in contas.DefaultIfEmpty()
            join categoria in contexto.Categorias on t.CategoriaId equals categoria.Id into categorias
            from categoria in categorias.DefaultIfEmpty()
            orderby t.Data, t.Id
            select new ExtratoItem(t.Id, t.Data,
                c == null ? "Conta não encontrada" : c.Nome,
                c == null ? "Banco não encontrado" : c.Banco,
                t.Descricao, categoria == null ? "Categoria não encontrada" : categoria.Nome,
                t.Tipo, t.Valor, t.TransferenciaId.HasValue,
                t.Importancia, t.Planejamento, t.Urgencia, t.Justificativa)).ToList();

        decimal Somar(TipoTransacao tipo, bool transferencia) =>
            totais.Where(g => g.Tipo == tipo && g.Transferencia == transferencia).Sum(g => g.Valor);
        leitura.Commit();
        return new ExtratoPagina(paginaAtual, tamanhoPagina, quantidade,
            new ExtratoTotais(Somar(TipoTransacao.Entrada, false), Somar(TipoTransacao.Saida, false),
                Somar(TipoTransacao.Entrada, true), Somar(TipoTransacao.Saida, true)), itens);
    }

    public ExtratoPagina ConsultarCompras(IReadOnlyCollection<Guid> cartaoIds, DateTime dataInicial,
        DateTime dataFinal, int pagina = 1, int tamanhoPagina = 50)
    {
        ArgumentNullException.ThrowIfNull(cartaoIds);
        if (cartaoIds.Count == 0) throw new ArgumentException("Selecione pelo menos um cartão.", nameof(cartaoIds));
        if (dataInicial.Date > dataFinal.Date)
            throw new ArgumentException("A data inicial não pode ser posterior à data final.");
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
        if (tamanhoPagina is < 1 or > 200) throw new ArgumentOutOfRangeException(nameof(tamanhoPagina));

        Guid[] ids = cartaoIds.Distinct().ToArray();
        DateTime inicio = dataInicial.Date;
        using JKCASHDbContext contexto = _criarContexto();
        contexto.Database.OpenConnection();
        using DbTransaction leitura = contexto.Database.IsSqlite()
            ? ((SqliteConnection)contexto.Database.GetDbConnection()).BeginTransaction(IsolationLevel.Serializable, deferred: true)
            : contexto.Database.GetDbConnection().BeginTransaction(IsolationLevel.Serializable);
        using var vinculo = contexto.Database.UseTransaction(leitura);

        var compras = contexto.ComprasCartao.AsNoTracking()
            .Where(c => ids.Contains(c.CartaoId) && c.Data >= inicio);
        if (dataFinal.Date < DateTime.MaxValue.Date)
        {
            DateTime fimExclusivo = dataFinal.Date.AddDays(1);
            compras = compras.Where(c => c.Data < fimExclusivo);
        }

        int quantidade = compras.Count();
        decimal total = contexto.Database.IsSqlite()
            ? compras.Sum(c => (long)c.Valor) / 100m
            : compras.Sum(c => c.Valor);
        int totalPaginas = quantidade == 0 ? 1 : (quantidade - 1) / tamanhoPagina + 1;
        int paginaAtual = Math.Min(pagina, totalPaginas);
        var paginaConsulta = compras.OrderBy(c => c.Data).ThenBy(c => c.Id)
            .Skip((paginaAtual - 1) * tamanhoPagina).Take(tamanhoPagina);

        var itens = (from compra in paginaConsulta
            join cartao in contexto.CartoesCredito on compra.CartaoId equals cartao.Id into cartoes
            from cartao in cartoes.DefaultIfEmpty()
            join categoria in contexto.Categorias on compra.CategoriaId equals categoria.Id into categorias
            from categoria in categorias.DefaultIfEmpty()
            orderby compra.Data, compra.Id
            select new ExtratoItem(compra.Id, compra.Data,
                cartao == null ? "Cartão não encontrado" : cartao.Nome, "",
                compra.Descricao, categoria == null ? "Categoria não encontrada" : categoria.Nome,
                TipoTransacao.Saida, compra.Valor, false, compra.Importancia, compra.Planejamento,
                compra.Urgencia, compra.Justificativa, compra.QuantidadeParcelas)).ToList();

        if (itens.Count > 0)
        {
            Guid[] compraIds = itens.Select(item => item.Id).ToArray();
            var parcelas = contexto.ParcelasCartao.AsNoTracking()
                .Include(p => p.Fatura)
                .Where(p => compraIds.Contains(p.CompraId))
                .OrderBy(p => p.Numero)
                .ToList()
                .ToLookup(p => p.CompraId, p => new ExtratoParcela(p.Numero, p.Quantidade,
                    p.Valor, p.Ciclo.Fechamento, p.Ciclo.Vencimento));

            itens = itens.Select(item => item with { Parcelas = parcelas[item.Id].ToArray() }).ToList();
        }

        leitura.Commit();
        return new ExtratoPagina(paginaAtual, tamanhoPagina, quantidade,
            new ExtratoTotais(0m, total, 0m, 0m), itens);
    }

    private sealed record Grupo(TipoTransacao Tipo, bool Transferencia, int Quantidade, decimal Valor);
}
