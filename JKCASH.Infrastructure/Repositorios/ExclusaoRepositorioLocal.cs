using System.Data;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Infrastructure.Repositorios;

public sealed class ExclusaoRepositorioLocal : IExclusaoRepositorio
{
    public EstadoFinanceiro Carregar()
    {
        using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();
        using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
        EstadoFinanceiro estado = Carregar(contexto);
        transacao.Commit();
        return estado;
    }

    private static EstadoFinanceiro Carregar(JKCASHDbContext contexto) => new(
        contexto.Contas.AsNoTracking().ToList(),
        contexto.Categorias.AsNoTracking().ToList(),
        contexto.Transacoes.AsNoTracking().ToList(),
        contexto.CartoesCredito.AsNoTracking().ToList(),
        contexto.ComprasCartao.AsNoTrackingWithIdentityResolution()
            .Include(c => c.Parcelas).ThenInclude(p => p.Fatura).AsSplitQuery().ToList(),
        contexto.FaturasCartao.AsNoTrackingWithIdentityResolution()
            .Include(f => f.Parcelas).Include(f => f.Pagamentos).AsSplitQuery().ToList());

    public void Salvar(EstadoFinanceiro atualizado, string versaoOriginal)
    {
        using JKCASHDbContext contexto = ConexaoLocal.CriarContexto();
        using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);
        EstadoFinanceiro original = Carregar(contexto);
        if (original.Assinatura() != versaoOriginal)
            throw new InvalidOperationException("Os dados foram alterados. Consulte a prévia novamente.");

        var pagamentos = atualizado.Faturas.SelectMany(f => f.Pagamentos).Select(p => p.Id).ToHashSet();
        var parcelas = atualizado.Compras.SelectMany(c => c.Parcelas).Select(p => p.Id).ToHashSet();
        var compras = atualizado.Compras.Select(c => c.Id).ToHashSet();
        var faturas = atualizado.Faturas.Select(f => f.Id).ToHashSet();
        var cartoes = atualizado.Cartoes.Select(c => c.Id).ToHashSet();
        var transacoes = atualizado.Transacoes.Select(t => t.Id).ToHashSet();
        var contas = atualizado.Contas.Select(c => c.Id).ToHashSet();
        var categorias = atualizado.Categorias.Select(c => c.Id).ToHashSet();

        // Dependentes saem antes dos registros que eles referenciam; tudo na mesma transação local.
        contexto.PagamentosFatura.Where(p => !pagamentos.Contains(p.Id)).ExecuteDelete();
        contexto.ParcelasCartao.Where(p => !parcelas.Contains(p.Id)).ExecuteDelete();
        contexto.ComprasCartao.Where(c => !compras.Contains(c.Id)).ExecuteDelete();
        contexto.FaturasCartao.Where(f => !faturas.Contains(f.Id)).ExecuteDelete();
        contexto.CartoesCredito.Where(c => !cartoes.Contains(c.Id)).ExecuteDelete();
        contexto.Transacoes.Where(t => !transacoes.Contains(t.Id)).ExecuteDelete();
        contexto.Contas.Where(c => !contas.Contains(c.Id)).ExecuteDelete();
        contexto.Categorias.Where(c => !categorias.Contains(c.Id)).ExecuteDelete();

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
