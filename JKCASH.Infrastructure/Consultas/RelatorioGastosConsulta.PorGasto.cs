using JKCASH.Application.Modelos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Consultas;

public sealed partial class RelatorioGastosConsulta
{
    public IReadOnlyList<string> ListarNomesGastos()
    {
        using var db = _criarContexto();
        // Inclui os avulsos e nomes históricos, mesmo após editar o cadastro.
        return db.Gastos.Select(g => g.Nome)
            .Union(db.Transacoes.Where(t => t.Gasto != null).Select(t => t.Gasto!.Nome))
            .Union(db.ComprasCartao.Where(c => c.Gasto != null).Select(c => c.Gasto!.Nome))
            .OrderBy(nome => nome).ToList();
    }

    public IReadOnlyList<ResumoGasto> ConsultarPorGasto(IReadOnlyCollection<Guid> contaIds,
        DateTime dataInicial, DateTime dataFinal, FiltrosRelatorioGastos? filtros = null) =>
        ConsultarGastos(contaIds, dataInicial, dataFinal, filtros, false);

    public IReadOnlyList<ResumoGasto> ConsultarComprasPorGasto(IReadOnlyCollection<Guid> cartaoIds,
        DateTime dataInicial, DateTime dataFinal, FiltrosRelatorioGastos? filtros = null) =>
        ConsultarGastos(cartaoIds, dataInicial, dataFinal, filtros, true);

    private IReadOnlyList<ResumoGasto> ConsultarGastos(IReadOnlyCollection<Guid> origens,
        DateTime dataInicial, DateTime dataFinal, FiltrosRelatorioGastos? filtros, bool credito)
    {
        ArgumentNullException.ThrowIfNull(origens);
        if (origens.Count == 0) throw new ArgumentException(credito ? "Selecione pelo menos um cartão." : "Selecione pelo menos uma conta.");
        if (dataInicial.Date > dataFinal.Date) throw new ArgumentException("A data inicial não pode ser posterior à data final.");
        using var db = _criarContexto();
        Guid[] ids = origens.Distinct().ToArray();
        var lancamentos = credito
            ? db.ComprasCartao.AsNoTracking().Where(c => ids.Contains(c.CartaoId)).Select(c => new LinhaGasto
            {
                Data = c.Data, CategoriaId = c.CategoriaId, Valor = c.Valor,
                TemGasto = c.Gasto != null, CadastroId = c.Gasto == null ? null : c.Gasto.CadastroId,
                Nome = c.Gasto == null ? "Sem gasto vinculado" : c.Gasto.Nome,
                Natureza = c.Gasto == null ? null : c.Gasto.Natureza,
                Necessidade = c.Gasto == null ? null : c.Gasto.Necessidade,
                Frequencia = c.Gasto == null ? null : c.Gasto.Frequencia,
                Importancia = c.Importancia, Planejamento = c.Planejamento
            })
            : db.Transacoes.AsNoTracking().Where(t => ids.Contains(t.ContaId) && t.Tipo == TipoTransacao.Saida
                && t.TransferenciaId == null && !db.PagamentosFatura.Any(p => p.TransacaoId == t.Id)).Select(t => new LinhaGasto
            {
                Data = t.Data, CategoriaId = t.CategoriaId, Valor = t.Valor,
                TemGasto = t.Gasto != null, CadastroId = t.Gasto == null ? null : t.Gasto.CadastroId,
                Nome = t.Gasto == null ? "Sem gasto vinculado" : t.Gasto.Nome,
                Natureza = t.Gasto == null ? null : t.Gasto.Natureza,
                Necessidade = t.Gasto == null ? null : t.Gasto.Necessidade,
                Frequencia = t.Gasto == null ? null : t.Gasto.Frequencia,
                Importancia = t.Importancia, Planejamento = t.Planejamento
            });

        DateTime inicio = dataInicial.Date;
        lancamentos = lancamentos.Where(t => t.Data >= inicio);
        if (dataFinal.Date < DateTime.MaxValue.Date)
        {
            DateTime fim = dataFinal.Date.AddDays(1);
            lancamentos = lancamentos.Where(t => t.Data < fim);
        }
        if (filtros is not null)
        {
            string[] nomes = filtros.NomesGastos.Distinct().ToArray();
            bool semGasto = filtros.SemGastoVinculado;
            if (nomes.Length > 0 || semGasto)
                lancamentos = lancamentos.Where(t => (t.TemGasto && nomes.Contains(t.Nome)) || (!t.TemGasto && semGasto));
            Guid[] categorias = filtros.Categorias.Distinct().ToArray();
            if (categorias.Length > 0) lancamentos = lancamentos.Where(t => categorias.Contains(t.CategoriaId));
            var importancias = filtros.Importancias.Distinct().ToArray();
            if (importancias.Length > 0) lancamentos = lancamentos.Where(t => t.Importancia.HasValue && importancias.Contains(t.Importancia.Value));
            var naturezas = filtros.Naturezas.Distinct().ToArray();
            if (naturezas.Length > 0) lancamentos = lancamentos.Where(t => t.Natureza.HasValue && naturezas.Contains(t.Natureza.Value));
            var necessidades = filtros.Necessidades.Distinct().ToArray();
            if (necessidades.Length > 0) lancamentos = lancamentos.Where(t => t.Necessidade.HasValue && necessidades.Contains(t.Necessidade.Value));
            var planejamentos = filtros.Planejamentos.Distinct().ToArray();
            if (planejamentos.Length > 0) lancamentos = lancamentos.Where(t => t.Planejamento.HasValue && planejamentos.Contains(t.Planejamento.Value));
            var frequencias = filtros.Frequencias.Distinct().ToArray();
            if (frequencias.Length > 0) lancamentos = lancamentos.Where(t => t.Frequencia.HasValue && frequencias.Contains(t.Frequencia.Value));
        }
        var grupos = from t in lancamentos
            join categoria in db.Categorias on t.CategoriaId equals categoria.Id into categorias
            from categoria in categorias.DefaultIfEmpty()
            group t by new
            {
                t.CadastroId, t.TemGasto, t.Nome, t.CategoriaId,
                Categoria = categoria == null ? "Categoria não encontrada" : categoria.Nome,
                t.Natureza, t.Importancia, t.Necessidade, t.Planejamento, t.Frequencia
            };

        // SQLite guarda moeda em centavos; a agregação continua inteira no banco.
        var resultado = db.Database.IsSqlite()
            ? grupos.Select(g => new { g.Key, Quantidade = g.Count(), Centavos = g.Sum(t => (long)t.Valor) })
                .ToList().Select(g => new ResumoGasto(g.Key.CadastroId, g.Key.Nome, g.Key.Categoria,
                    g.Key.Natureza, g.Key.Importancia, g.Key.Necessidade, g.Key.Planejamento, g.Key.Frequencia, g.Quantidade, g.Centavos / 100m)).ToList()
            : grupos.Select(g => new ResumoGasto(g.Key.CadastroId, g.Key.Nome, g.Key.Categoria,
                g.Key.Natureza, g.Key.Importancia, g.Key.Necessidade, g.Key.Planejamento, g.Key.Frequencia, g.Count(), g.Sum(t => t.Valor))).ToList();
        return resultado.OrderByDescending(g => g.Total).ThenBy(g => g.Nome).ThenBy(g => g.Categoria).ToList();
    }

    private sealed class LinhaGasto
    {
        public DateTime Data { get; init; }
        public Guid CategoriaId { get; init; }
        public decimal Valor { get; init; }
        public bool TemGasto { get; init; }
        public Guid? CadastroId { get; init; }
        public string Nome { get; init; } = "";
        public NaturezaGasto? Natureza { get; init; }
        public ImportanciaTransacao? Importancia { get; init; }
        public NecessidadeGasto? Necessidade { get; init; }
        public PlanejamentoTransacao? Planejamento { get; init; }
        public FrequenciaGasto? Frequencia { get; init; }
    }
}
