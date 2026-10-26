using JKCASH.Application.Modelos;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed partial class ExclusaoRepositorioEf
{
    public List<ItemExclusao> Listar(TipoRegistro tipo)
    {
        using var db = _criarContexto();
        // Projeta somente os campos necessários à seleção no Console, sem Includes.
        IEnumerable<ItemExclusao> itens = tipo switch
        {
            TipoRegistro.Conta => db.Contas.AsNoTracking().Select(c => new { c.Id, c.Nome, c.Banco, Valor = c.Saldo.SaldoAtual })
                .ToList().Select(c => new ItemExclusao(c.Id, $"{c.Nome} | {c.Banco} | {c.Valor:C2}")),
            TipoRegistro.Categoria => db.Categorias.AsNoTracking().Select(c => new ItemExclusao(c.Id, c.Nome)).ToList(),
            TipoRegistro.Transacao => db.Transacoes.AsNoTracking().Select(t => new { t.Id, t.Data, t.Tipo, t.Valor, t.Descricao,
                    Conta = db.Contas.Where(c => c.Id == t.ContaId).Select(c => c.Nome).FirstOrDefault() })
                .ToList().Select(t => new ItemExclusao(t.Id, $"{t.Data:dd/MM/yyyy} | {t.Conta ?? "Conta ausente"} | {t.Tipo} | {t.Valor:C2} | {t.Descricao}")),
            TipoRegistro.Transferencia => db.Transacoes.AsNoTracking().Where(t => t.TransferenciaId != null)
                .Select(t => new { t.TransferenciaId, t.Data, t.Valor, Conta = db.Contas.Where(c => c.Id == t.ContaId).Select(c => c.Nome).FirstOrDefault() })
                .ToList().GroupBy(t => t.TransferenciaId!.Value)
                .Select(g => new ItemExclusao(g.Key, $"{g.First().Data:dd/MM/yyyy} | {string.Join(" ↔ ", g.Select(t => t.Conta ?? "Conta ausente"))} | {g.First().Valor:C2}")),
            TipoRegistro.Cartao => db.CartoesCredito.AsNoTracking().Select(c => new { c.Id, c.Nome, c.ValorUtilizado })
                .ToList().Select(c => new ItemExclusao(c.Id, $"{c.Nome} | utilizado {c.ValorUtilizado:C2}")),
            TipoRegistro.Compra => db.ComprasCartao.AsNoTracking().Select(c => new { c.Id, c.Data, c.Descricao, c.Valor,
                    Cartao = db.CartoesCredito.Where(t => t.Id == c.CartaoId).Select(t => t.Nome).FirstOrDefault() })
                .ToList().Select(c => new ItemExclusao(c.Id, $"{c.Cartao ?? "Cartão ausente"} | {c.Data:dd/MM/yyyy} | {c.Descricao} | {c.Valor:C2}")),
            TipoRegistro.Fatura => ListarFaturas(db),
            TipoRegistro.Pagamento => db.PagamentosFatura.AsNoTracking().Select(p => new { p.Id, p.Data, p.Valor,
                    Conta = db.Contas.Where(c => c.Id == p.ContaId).Select(c => c.Nome).FirstOrDefault(),
                    Cartao = db.FaturasCartao.Where(f => f.Id == p.FaturaId).Select(f => db.CartoesCredito.Where(c => c.Id == f.CartaoId).Select(c => c.Nome).FirstOrDefault()).FirstOrDefault() })
                .ToList().Select(p => new ItemExclusao(p.Id, $"{p.Cartao ?? "Cartão ausente"} | {p.Data:dd/MM/yyyy} | {p.Conta ?? "Conta ausente"} | {p.Valor:C2}")),
            TipoRegistro.Parcela => db.ParcelasCartao.AsNoTracking().Select(p => new { p.Id, p.Numero, p.Quantidade, p.Valor,
                    Cartao = db.CartoesCredito.Where(c => c.Id == p.CartaoId).Select(c => c.Nome).FirstOrDefault(),
                    Compra = db.ComprasCartao.Where(c => c.Id == p.CompraId).Select(c => c.Descricao).FirstOrDefault() })
                .ToList().Select(p => new ItemExclusao(p.Id, $"{p.Cartao ?? "Cartão ausente"} | {p.Compra} | parcela {p.Numero}/{p.Quantidade} | {p.Valor:C2}")),
            _ => throw new ArgumentException("Tipo de registro inválido.", nameof(tipo))
        };
        return itens.OrderBy(i => i.Descricao).ToList();
    }

    private static List<ItemExclusao> ListarFaturas(Persistencia.JKCASHDbContext db)
    {
        if (db.Database.IsSqlite())
            return db.FaturasCartao.AsNoTracking().Select(f => new { f.Id, f.Ciclo.Vencimento,
                Cartao = db.CartoesCredito.Where(c => c.Id == f.CartaoId).Select(c => c.Nome).FirstOrDefault(),
                Total = db.ParcelasCartao.Where(p => p.FaturaId == f.Id).Sum(p => (long)p.Valor) })
                .ToList().Select(f => new ItemExclusao(f.Id, $"{f.Cartao ?? "Cartão ausente"} | vencimento {f.Vencimento:dd/MM/yyyy} | {f.Total / 100m:C2}")).ToList();
        return db.FaturasCartao.AsNoTracking().Select(f => new { f.Id, f.Ciclo.Vencimento,
            Cartao = db.CartoesCredito.Where(c => c.Id == f.CartaoId).Select(c => c.Nome).FirstOrDefault(),
            Total = db.ParcelasCartao.Where(p => p.FaturaId == f.Id).Sum(p => p.Valor) })
            .ToList().Select(f => new ItemExclusao(f.Id, $"{f.Cartao ?? "Cartão ausente"} | vencimento {f.Vencimento:dd/MM/yyyy} | {f.Total:C2}")).ToList();
    }
}
