using System.Data;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed partial class ExclusaoRepositorioEf : IExclusaoRepositorio
{
    private readonly Func<JKCASHDbContext> _criarContexto;

    public ExclusaoRepositorioEf(Func<JKCASHDbContext> criarContexto)
    {
        ArgumentNullException.ThrowIfNull(criarContexto);
        _criarContexto = criarContexto;
    }
    public EstadoFinanceiro Carregar(TipoRegistro tipo, Guid id)
    {
        using JKCASHDbContext contexto = _criarContexto();
        using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
        EstadoFinanceiro estado = Carregar(contexto, tipo, id);
        transacao.Commit();
        return estado;
    }

    public void Salvar(EstadoFinanceiro atualizado, PreviaExclusao previa)
    {
        using JKCASHDbContext contexto = _criarContexto();
        using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
        EstadoFinanceiro original = Carregar(contexto, previa.Tipo, previa.Id);
        if (original.Assinatura() != previa.Versao)
            throw new InvalidOperationException("Os dados foram alterados. Consulte a prévia novamente.");

        var pagamentos = atualizado.Faturas.SelectMany(f => f.Pagamentos).Select(p => p.Id).ToHashSet();
        var parcelas = atualizado.Compras.SelectMany(c => c.Parcelas).Select(p => p.Id).ToHashSet();
        var compras = atualizado.Compras.Select(c => c.Id).ToHashSet();
        var faturas = atualizado.Faturas.Select(f => f.Id).ToHashSet();
        var cartoes = atualizado.Cartoes.Select(c => c.Id).ToHashSet();
        var transacoes = atualizado.Transacoes.Select(t => t.Id).ToHashSet();
        var contas = atualizado.Contas.Select(c => c.Id).ToHashSet();
        var categorias = atualizado.Categorias.Select(c => c.Id).ToHashSet();

        var removerpagamentos = original.Faturas.SelectMany(f => f.Pagamentos).Select(p => p.Id).Except(pagamentos).ToArray();
        var removerparcelas = original.Compras.SelectMany(c => c.Parcelas).Select(p => p.Id).Except(parcelas).ToArray();
        var removercompras = original.Compras.Select(c => c.Id).Except(compras).ToArray();
        var removerfaturas = original.Faturas.Select(f => f.Id).Except(faturas).ToArray();
        var removercartoes = original.Cartoes.Select(c => c.Id).Except(cartoes).ToArray();
        var removertransacoes = original.Transacoes.Select(t => t.Id).Except(transacoes).ToArray();
        var removercontas = original.Contas.Select(c => c.Id).Except(contas).ToArray();
        var removercategorias = original.Categorias.Select(c => c.Id).Except(categorias).ToArray();
        // Dependentes saem antes dos registros que eles referenciam; tudo na mesma transação do banco.
        contexto.PagamentosFatura.Where(p => removerpagamentos.Contains(p.Id)).ExecuteDelete();
        contexto.ParcelasCartao.Where(p => removerparcelas.Contains(p.Id)).ExecuteDelete();
        contexto.ComprasCartao.Where(c => removercompras.Contains(c.Id)).ExecuteDelete();
        contexto.FaturasCartao.Where(f => removerfaturas.Contains(f.Id)).ExecuteDelete();
        contexto.CartoesCredito.Where(c => removercartoes.Contains(c.Id)).ExecuteDelete();
        contexto.Transacoes.Where(t => removertransacoes.Contains(t.Id)).ExecuteDelete();
        contexto.Contas.Where(c => removercontas.Contains(c.Id)).ExecuteDelete();
        contexto.Categorias.Where(c => removercategorias.Contains(c.Id)).ExecuteDelete();

        foreach (var conta in atualizado.Contas)
        {
            if (original.Contas.Single(c => c.Id == conta.Id).Saldo.SaldoAtual == conta.Saldo.SaldoAtual) continue;
            contexto.Contas.Attach(conta);
            contexto.Entry(conta.Saldo).Property(s => s.SaldoAtual).IsModified = true;
        }
        foreach (var cartao in atualizado.Cartoes)
        {
            if (original.Cartoes.Single(c => c.Id == cartao.Id).ValorUtilizado == cartao.ValorUtilizado) continue;
            contexto.CartoesCredito.Attach(cartao);
            contexto.Entry(cartao).Property(c => c.ValorUtilizado).IsModified = true;
        }
        contexto.SaveChanges();
        transacao.Commit();
    }
}
