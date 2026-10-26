using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Modelos;

public static class CatalogoExclusao
{
    public static List<ItemExclusao> Listar(EstadoFinanceiro e, TipoRegistro tipo)
    {
        string ContaNome(Guid id) => e.Contas.SingleOrDefault(c => c.Id == id)?.Nome ?? "Conta ausente";
        string CartaoNome(Guid id) => e.Cartoes.SingleOrDefault(c => c.Id == id)?.Nome ?? "Cartão ausente";
        IEnumerable<ItemExclusao> itens = tipo switch
        {
            TipoRegistro.Conta => e.Contas.Select(c => new ItemExclusao(c.Id, $"{c.Nome} | {c.Banco} | {c.Saldo.SaldoAtual:C2}")),
            TipoRegistro.Categoria => e.Categorias.Select(c => new ItemExclusao(c.Id, c.Nome)),
            TipoRegistro.Transacao => e.Transacoes.Select(t => new ItemExclusao(t.Id, $"{t.Data:dd/MM/yyyy} | {ContaNome(t.ContaId)} | {t.Tipo} | {t.Valor:C2} | {t.Descricao}")),
            TipoRegistro.Transferencia => e.Transacoes.Where(t => t.TransferenciaId.HasValue)
                .GroupBy(t => t.TransferenciaId!.Value).Select(g => new ItemExclusao(g.Key,
                    $"{g.First().Data:dd/MM/yyyy} | {string.Join(" ↔ ", g.Select(t => ContaNome(t.ContaId)))} | {g.First().Valor:C2}")),
            TipoRegistro.Cartao => e.Cartoes.Select(c => new ItemExclusao(c.Id, $"{c.Nome} | utilizado {c.ValorUtilizado:C2}")),
            TipoRegistro.Compra => e.Compras.Select(c => new ItemExclusao(c.Id, $"{CartaoNome(c.CartaoId)} | {c.Data:dd/MM/yyyy} | {c.Descricao} | {c.Valor:C2}")),
            TipoRegistro.Fatura => e.Faturas.Select(f => new ItemExclusao(f.Id, $"{CartaoNome(f.CartaoId)} | vencimento {f.Ciclo.Vencimento:dd/MM/yyyy} | {f.Total:C2}")),
            TipoRegistro.Pagamento => e.Faturas.SelectMany(f => f.Pagamentos.Select(p => new ItemExclusao(p.Id,
                $"{CartaoNome(f.CartaoId)} | {p.Data:dd/MM/yyyy} | {ContaNome(p.ContaId)} | {p.Valor:C2}"))),
            TipoRegistro.Parcela => e.Compras.SelectMany(c => c.Parcelas.Select(p => new ItemExclusao(p.Id,
                $"{CartaoNome(c.CartaoId)} | {c.Descricao} | parcela {p.Numero}/{p.Quantidade} | {p.Valor:C2}"))),
            _ => throw new ArgumentException("Tipo de registro inválido.", nameof(tipo))
        };
        return itens.OrderBy(i => i.Descricao).ToList();
    }

}
