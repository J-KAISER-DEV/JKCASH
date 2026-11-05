using JKCASH.Application.Modelos;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed partial class ExclusaoRepositorioEf
{
    private static EstadoFinanceiro Carregar(JKCASHDbContext db, TipoRegistro tipo, Guid id)
    {
        HashSet<Guid> contas = new(), categorias = new(), cartoes = new(), compras = new(), faturas = new();
        List<Transacao> transacoes = new();
        switch (tipo)
        {
            case TipoRegistro.Conta:
                // Predicados também são repetidos na confirmação, dentro da transação.
                if (db.CartoesCredito.Any(c => c.ContaId == id) || db.PagamentosFatura.Any(p => p.ContaId == id)
                    || db.Transacoes.Any(t => t.ContaId == id && t.TransferenciaId != null))
                    throw new InvalidOperationException("Exclua primeiro os cartões, pagamentos e transferências relacionados à conta.");
                contas.Add(id);
                transacoes = db.Transacoes.AsNoTracking().Where(t => t.ContaId == id).ToList();
                break;
            case TipoRegistro.Categoria:
                if (db.Gastos.Any(g => g.CategoriaId == id))
                    throw new InvalidOperationException("A categoria está em uso por um gasto salvo. Altere a categoria desse gasto primeiro.");
                if (db.Transacoes.Any(t => t.CategoriaId == id) || db.ComprasCartao.Any(c => c.CategoriaId == id))
                    throw new InvalidOperationException("A categoria está em uso. Exclua as movimentações relacionadas primeiro.");
                categorias.Add(id);
                break;
            case TipoRegistro.Transacao:
                var alvo = db.Transacoes.AsNoTracking().SingleOrDefault(t => t.Id == id);
                if (alvo is not null) transacoes = alvo.TransferenciaId is Guid transferencia
                    ? db.Transacoes.AsNoTracking().Where(t => t.TransferenciaId == transferencia).ToList()
                    : new() { alvo };
                break;
            case TipoRegistro.Transferencia:
                transacoes = db.Transacoes.AsNoTracking().Where(t => t.TransferenciaId == id).ToList();
                break;
            case TipoRegistro.Cartao:
                cartoes.Add(id);
                compras.UnionWith(db.ComprasCartao.Where(c => c.CartaoId == id).Select(c => c.Id));
                faturas.UnionWith(db.FaturasCartao.Where(f => f.CartaoId == id).Select(f => f.Id));
                break;
            case TipoRegistro.Compra:
                compras.Add(id);
                break;
            case TipoRegistro.Parcela:
                compras.UnionWith(db.ParcelasCartao.Where(p => p.Id == id).Select(p => p.CompraId));
                break;
            case TipoRegistro.Fatura:
                faturas.Add(id);
                compras.UnionWith(db.ParcelasCartao.Where(p => p.FaturaId == id).Select(p => p.CompraId));
                break;
            case TipoRegistro.Pagamento:
                faturas.UnionWith(db.PagamentosFatura.Where(p => p.Id == id).Select(p => p.FaturaId));
                transacoes = db.Transacoes.AsNoTracking()
                    .Where(t => db.PagamentosFatura.Any(p => p.Id == id && p.TransacaoId == t.Id)).ToList();
                break;
            default:
                throw new ArgumentException("Tipo de registro inválido.", nameof(tipo));
        }
        var idsTransacoes = transacoes.Select(t => t.Id).ToArray();
        // Uma transação pode ser a saída de um pagamento.
        faturas.UnionWith(db.PagamentosFatura.Where(p => idsTransacoes.Contains(p.TransacaoId)).Select(p => p.FaturaId));
        var idsCompras = compras.ToArray();
        var listaCompras = db.ComprasCartao.AsNoTrackingWithIdentityResolution()
            .Where(c => idsCompras.Contains(c.Id)).Include(c => c.Parcelas).ThenInclude(p => p.Fatura)
            .AsSplitQuery().ToList();
        faturas.UnionWith(listaCompras.SelectMany(c => c.Parcelas).Select(p => p.FaturaId));
        cartoes.UnionWith(listaCompras.Select(c => c.CartaoId));
        var idsFaturas = faturas.ToArray();
        var listaFaturas = db.FaturasCartao.AsNoTrackingWithIdentityResolution()
            .Where(f => idsFaturas.Contains(f.Id)).Include(f => f.Parcelas).Include(f => f.Pagamentos)
            .AsSplitQuery().ToList();
        cartoes.UnionWith(listaFaturas.Select(f => f.CartaoId));
        if (tipo == TipoRegistro.Cartao)
        {
            var debitos = listaFaturas.SelectMany(f => f.Pagamentos).Select(p => p.TransacaoId).ToArray();
            transacoes = db.Transacoes.AsNoTracking().Where(t => debitos.Contains(t.Id)).ToList();
        }
        contas.UnionWith(transacoes.Select(t => t.ContaId));
        // Inclui a conta declarada pelo pagamento para conferir também vínculos inconsistentes.
        contas.UnionWith(listaFaturas.SelectMany(f => f.Pagamentos)
            .Where(p => idsTransacoes.Contains(p.TransacaoId) || tipo == TipoRegistro.Cartao || p.Id == id)
            .Select(p => p.ContaId));
        var idsContas = contas.ToArray();
        var idsCategorias = categorias.ToArray();
        var idsCartoes = cartoes.ToArray();
        return new(db.Contas.AsNoTracking().Where(c => idsContas.Contains(c.Id)).ToList(),
            db.Categorias.AsNoTracking().Where(c => idsCategorias.Contains(c.Id)).ToList(),
            transacoes, db.CartoesCredito.AsNoTracking().Where(c => idsCartoes.Contains(c.Id)).ToList(),
            listaCompras, listaFaturas);
    }
}
