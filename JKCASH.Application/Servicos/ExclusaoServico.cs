using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.Application.Servicos;

// Prepara cópias e valida todos os efeitos antes de pedir a gravação conjunta.
public sealed class ExclusaoServico(IExclusaoRepositorio repositorio)
{
    private readonly IExclusaoRepositorio _repositorio = repositorio
        ?? throw new ArgumentNullException(nameof(repositorio));

    public List<ItemExclusao> Listar(TipoRegistro tipo) => _repositorio.Listar(tipo);

    public PreviaExclusao Preparar(TipoRegistro tipo, Guid id)
    {
        EstadoFinanceiro original = _repositorio.Carregar(tipo, id);
        string descricao = CatalogoExclusao.Listar(original, tipo).SingleOrDefault(i => i.Id == id)?.Descricao
            ?? throw new InvalidOperationException("Registro não encontrado.");
        EstadoFinanceiro atualizado = Remover(original, tipo, id);
        List<string> linhas = new();
        void Quantidade(string nome, int antes, int depois)
        {
            if (antes != depois) linhas.Add($"{nome}: {antes - depois} registro(s) excluído(s).");
        }
        Quantidade("Contas", original.Contas.Count, atualizado.Contas.Count);
        Quantidade("Categorias", original.Categorias.Count, atualizado.Categorias.Count);
        Quantidade("Transações", original.Transacoes.Count, atualizado.Transacoes.Count);
        Quantidade("Cartões", original.Cartoes.Count, atualizado.Cartoes.Count);
        Quantidade("Compras", original.Compras.Count, atualizado.Compras.Count);
        Quantidade("Faturas", original.Faturas.Count, atualizado.Faturas.Count);
        Quantidade("Parcelas", original.Compras.Sum(c => c.Parcelas.Count), atualizado.Compras.Sum(c => c.Parcelas.Count));
        Quantidade("Pagamentos", original.Faturas.Sum(f => f.Pagamentos.Count), atualizado.Faturas.Sum(f => f.Pagamentos.Count));
        foreach (Conta conta in atualizado.Contas)
        {
            decimal anterior = original.Contas.Single(c => c.Id == conta.Id).Saldo.SaldoAtual;
            if (anterior != conta.Saldo.SaldoAtual)
                linhas.Add($"Saldo de {conta.Nome}: {anterior:C2} → {conta.Saldo.SaldoAtual:C2}.");
        }
        foreach (CartaoCredito cartao in atualizado.Cartoes)
        {
            decimal anterior = original.Cartoes.Single(c => c.Id == cartao.Id).ValorUtilizado;
            if (anterior != cartao.ValorUtilizado)
                linhas.Add($"Utilizado de {cartao.Nome}: {anterior:C2} → {cartao.ValorUtilizado:C2}.");
        }
        if (tipo is TipoRegistro.Parcela or TipoRegistro.Fatura)
            linhas.Insert(0, "Serão removidas as compras completas, incluindo as parcelas de outros meses.");
        return new(tipo, id, descricao, string.Join(Environment.NewLine, linhas), original.Assinatura());
    }

    public void Excluir(PreviaExclusao previa)
    {
        ArgumentNullException.ThrowIfNull(previa);
        EstadoFinanceiro original = _repositorio.Carregar(previa.Tipo, previa.Id);
        if (original.Assinatura() != previa.Versao)
            throw new InvalidOperationException("Os dados foram alterados. Consulte a prévia novamente.");
        _repositorio.Salvar(Remover(original, previa.Tipo, previa.Id), previa);
    }

    private static EstadoFinanceiro Remover(EstadoFinanceiro e, TipoRegistro tipo, Guid id)
    {
        if (!CatalogoExclusao.Listar(e, tipo).Any(i => i.Id == id)) throw new InvalidOperationException("Registro não encontrado.");
        HashSet<Guid> contas = new(), categorias = new(), transacoes = new(), cartoes = new(),
            compras = new(), faturas = new(), pagamentos = new();
        var todosPagamentos = e.Faturas.SelectMany(f => f.Pagamentos).ToList();
        switch (tipo)
        {
            case TipoRegistro.Conta:
                if (e.Cartoes.Any(c => c.ContaId == id) || todosPagamentos.Any(p => p.ContaId == id)
                    || e.Transacoes.Any(t => t.ContaId == id && t.TransferenciaId.HasValue))
                    throw new InvalidOperationException("Exclua primeiro os cartões, pagamentos e transferências relacionados à conta.");
                contas.Add(id);
                transacoes.UnionWith(e.Transacoes.Where(t => t.ContaId == id).Select(t => t.Id));
                break;
            case TipoRegistro.Categoria:
                if (e.Transacoes.Any(t => t.CategoriaId == id) || e.Compras.Any(c => c.CategoriaId == id))
                    throw new InvalidOperationException("A categoria está em uso. Exclua as movimentações relacionadas primeiro.");
                categorias.Add(id);
                break;
            case TipoRegistro.Transacao:
                Transacao transacao = e.Transacoes.Single(t => t.Id == id);
                if (transacao.TransferenciaId is Guid transferencia) AdicionarTransferencia(transferencia);
                else transacoes.Add(id);
                pagamentos.UnionWith(todosPagamentos.Where(p => p.TransacaoId == id).Select(p => p.Id));
                break;
            case TipoRegistro.Transferencia:
                AdicionarTransferencia(id);
                break;
            case TipoRegistro.Cartao:
                cartoes.Add(id);
                compras.UnionWith(e.Compras.Where(c => c.CartaoId == id).Select(c => c.Id));
                faturas.UnionWith(e.Faturas.Where(f => f.CartaoId == id).Select(f => f.Id));
                pagamentos.UnionWith(e.Faturas.Where(f => faturas.Contains(f.Id)).SelectMany(f => f.Pagamentos).Select(p => p.Id));
                break;
            case TipoRegistro.Compra:
                compras.Add(id);
                break;
            case TipoRegistro.Parcela:
                compras.Add(e.Compras.Single(c => c.Parcelas.Any(p => p.Id == id)).Id);
                break;
            case TipoRegistro.Fatura:
                FaturaCartao fatura = e.Faturas.Single(f => f.Id == id);
                faturas.Add(id);
                compras.UnionWith(fatura.Parcelas.Select(p => p.CompraId));
                if (fatura.Pagamentos.Count > 0)
                    throw new InvalidOperationException("Exclua os pagamentos da fatura primeiro.");
                break;
            case TipoRegistro.Pagamento:
                pagamentos.Add(id);
                break;
        }

        HashSet<Guid> parcelas = e.Compras.Where(c => compras.Contains(c.Id)).SelectMany(c => c.Parcelas).Select(p => p.Id).ToHashSet();
        foreach (FaturaCartao fatura in e.Faturas.Where(f => !cartoes.Contains(f.CartaoId)))
            if (fatura.Parcelas.Any(p => parcelas.Contains(p.Id)) && fatura.Pagamentos.Count > 0)
                throw new InvalidOperationException("A compra possui fatura com pagamento. Exclua esses pagamentos antes de remover a compra.");

        foreach (PagamentoFatura pagamento in todosPagamentos.Where(p => pagamentos.Contains(p.Id)))
        {
            Transacao? debito = e.Transacoes.SingleOrDefault(t => t.Id == pagamento.TransacaoId);
            if (debito is null || debito.ContaId != pagamento.ContaId || debito.Valor != pagamento.Valor
                || debito.Tipo != TipoTransacao.Saida || debito.TransferenciaId.HasValue)
                throw new InvalidOperationException("O pagamento não corresponde à sua saída na conta. A exclusão foi cancelada.");
            transacoes.Add(debito.Id);
        }

        List<Conta> novasContas = e.Contas.Where(c => !contas.Contains(c.Id)).Select(c =>
        {
            decimal ajuste = e.Transacoes.Where(t => transacoes.Contains(t.Id) && t.ContaId == c.Id)
                .Sum(t => t.Tipo == TipoTransacao.Entrada ? -t.Valor : t.Valor);
            decimal saldo = c.Saldo.SaldoAtual + ajuste;
            if (saldo < -c.Saldo.LimiteChequeEspecial)
                throw new InvalidOperationException($"A exclusão deixaria a conta {c.Nome} sem saldo suficiente. Remova as saídas relacionadas primeiro.");
            return ajuste == 0 ? c : Conta.Restaurar(c.Id, c.Nome, c.Banco, c.Tipo, c.Destinacao,
                c.Saldo.SaldoInicial, saldo, c.Saldo.LimiteChequeEspecial, c.Ativa);
        }).ToList();

        List<CartaoCredito> novosCartoes = e.Cartoes.Where(c => !cartoes.Contains(c.Id)).Select(c =>
        {
            decimal removido = e.Compras.Where(compra => compras.Contains(compra.Id) && compra.CartaoId == c.Id).Sum(compra => compra.Valor);
            decimal estornoPagamento = e.Faturas.Where(f => f.CartaoId == c.Id).SelectMany(f => f.Pagamentos)
                .Where(p => pagamentos.Contains(p.Id)).Sum(p => p.Valor);
            decimal utilizado = c.ValorUtilizado - removido + estornoPagamento;
            if (utilizado < 0 || utilizado > c.Limite)
                throw new InvalidOperationException($"A exclusão ultrapassaria o limite do cartão {c.Nome}. Revise as compras e pagamentos relacionados.");
            return utilizado == c.ValorUtilizado ? c : CartaoCredito.Restaurar(c.Id, c.ContaId, c.Nome,
                c.Limite, utilizado, c.DiaFechamento, c.DiaVencimento, c.CompraNoFechamentoNaProximaFatura);
        }).ToList();

        List<FaturaCartao> novasFaturas = new();
        foreach (FaturaCartao fatura in e.Faturas.Where(f => !faturas.Contains(f.Id)))
        {
            bool alterada = fatura.Parcelas.Any(p => parcelas.Contains(p.Id))
                || fatura.Pagamentos.Any(p => pagamentos.Contains(p.Id));
            FaturaCartao restante = alterada ? fatura.SemRegistros(parcelas, pagamentos) : fatura;
            if (!alterada || restante.Parcelas.Count > 0 || restante.Pagamentos.Count > 0)
                novasFaturas.Add(restante);
        }
        return new(novasContas, e.Categorias.Where(c => !categorias.Contains(c.Id)).ToList(),
            e.Transacoes.Where(t => !transacoes.Contains(t.Id)).ToList(), novosCartoes,
            e.Compras.Where(c => !compras.Contains(c.Id)).ToList(), novasFaturas);

        void AdicionarTransferencia(Guid transferenciaId)
        {
            var par = e.Transacoes.Where(t => t.TransferenciaId == transferenciaId).ToList();
            if (par.Count != 2 || par[0].ContaId == par[1].ContaId || par[0].Valor != par[1].Valor
                || par.Count(t => t.Tipo == TipoTransacao.Entrada) != 1 || par.Count(t => t.Tipo == TipoTransacao.Saida) != 1)
                throw new InvalidOperationException("A transferência está inconsistente. Nenhum registro foi excluído.");
            transacoes.UnionWith(par.Select(t => t.Id));
        }
    }
}
