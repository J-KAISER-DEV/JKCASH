using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using JKCASH.Domain.Entidades;

namespace JKCASH.Application.Modelos;

public enum TipoRegistro { Conta, Categoria, Transacao, Transferencia, Cartao, Compra, Fatura, Pagamento, Parcela }
public sealed record ItemExclusao(Guid Id, string Descricao);
public sealed record PreviaExclusao(TipoRegistro Tipo, Guid Id, string Descricao, string Impacto, string Versao);

public sealed record EstadoFinanceiro(List<Conta> Contas, List<Categoria> Categorias, List<Transacao> Transacoes,
    List<CartaoCredito> Cartoes, List<CompraCartao> Compras, List<FaturaCartao> Faturas)
{
    public string Assinatura()
    {
        var dados = new
        {
            Contas = Contas.OrderBy(c => c.Id), Categorias = Categorias.OrderBy(c => c.Id),
            Transacoes = Transacoes.OrderBy(t => t.Id), Cartoes = Cartoes.OrderBy(c => c.Id),
            Compras = Compras.OrderBy(c => c.Id).Select(c => new { c.Id, c.CartaoId, c.Data, c.Descricao,
                c.Valor, c.CategoriaId, c.Importancia, c.Planejamento, c.Urgencia, c.Justificativa, c.QuantidadeParcelas,
                Parcelas = c.Parcelas.OrderBy(p => p.Id).Select(p => new { p.Id, p.CompraId, p.Numero, p.Quantidade, p.Valor, p.Ciclo }) }),
            Faturas = Faturas.OrderBy(f => f.Id).Select(f => new { f.Id, f.CartaoId, f.Ciclo,
                Parcelas = f.Parcelas.Select(p => p.Id).OrderBy(id => id), Pagamentos = f.Pagamentos.OrderBy(p => p.Id) })
        };
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dados))));
    }
}
