using JKCASH.Application.Interfaces;
using JKCASH.Application.Excecoes;
using JKCASH.Domain.Entidades;
using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Data;
using JKCASH.Domain.Enums;
namespace JKCASH.Infrastructure.Repositorios
{
    public class CompraCartaoRepositorioLocal
        : ICompraCartaoRepositorio, IFaturaCartaoRepositorio
    {
        private readonly TimeProvider _relogio;
        private readonly Func<JKCASHDbContext> _criarContexto;

        public CompraCartaoRepositorioLocal(TimeProvider? relogio = null, Func<JKCASHDbContext>? criarContexto = null)
        {
            _relogio = relogio ?? TimeProvider.System;
            _criarContexto = criarContexto ?? ConexaoLocal.CriarContexto;
        }

        public void Adicionar(CompraCartao compra, CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(compra);
            ArgumentNullException.ThrowIfNull(cartao);

            if (compra.CartaoId != cartao.Id)
                throw new ArgumentException("A compra pertence a outro cartão.", nameof(compra));

            using JKCASHDbContext contexto = _criarContexto();
            // Mantém as leituras e a gravação na mesma operação do banco.
            using var transacao = contexto.Database.BeginTransaction(IsolationLevel.Serializable);

            CartaoCredito persistido = contexto.CartoesCredito.SingleOrDefault(c => c.Id == cartao.Id)
                ?? throw new ConflitoFinanceiroException("O cartão não está mais disponível. Atualize os dados.");

            // O serviço já consumiu o limite do objeto recebido. Confere o estado anterior.
            if (persistido.ValorUtilizado != cartao.ValorUtilizado - compra.Valor
                || persistido.ContaId != cartao.ContaId
                || persistido.DiaFechamento != cartao.DiaFechamento
                || persistido.DiaVencimento != cartao.DiaVencimento
                || persistido.CompraNoFechamentoNaProximaFatura != cartao.CompraNoFechamentoNaProximaFatura)
            {
                throw new InvalidOperationException("O cartão foi alterado. Consulte novamente antes de comprar.");
            }

            persistido.RegistrarCompra(compra.Valor);
            DateTime hoje = _relogio.GetLocalNow().Date;
            if (compra.Data > hoje)
                throw new ArgumentException("Não é possível registrar uma compra futura.", nameof(compra));

            DateTime[] fechamentos = compra.Parcelas.Select(p => p.Ciclo.Fechamento).Distinct().ToArray();
            Dictionary<DateTime, FaturaCartao> faturas = ConsultarFaturas(contexto)
                .Where(f => f.CartaoId == cartao.Id && fechamentos.Contains(f.Ciclo.Fechamento))
                .ToDictionary(f => f.Ciclo.Fechamento);

            foreach (ParcelaCartao parcela in compra.Parcelas)
            {
                if (parcela.Ciclo.Encerramento <= hoje)
                    throw new InvalidOperationException("A fatura já fechou e não recebe novas compras.");

                if (faturas.TryGetValue(parcela.Ciclo.Fechamento, out FaturaCartao? fatura))
                {
                    // Valida pelas regras da entidade, sem anexar uma segunda versão da fatura.
                    _ = fatura.ComParcela(parcela, hoje);
                    contexto.Entry(parcela).Reference(p => p.Fatura).CurrentValue = fatura;
                }
                else
                {
                    fatura = FaturaCartao.Criar(parcela);
                    contexto.FaturasCartao.Add(fatura);
                    faturas.Add(parcela.Ciclo.Fechamento, fatura);
                }
            }

            contexto.ComprasCartao.Add(compra);
            // Compra, parcelas, faturas e limite são confirmados juntos.
            contexto.SaveChanges();
            transacao.Commit();
        }

        public CompraCartao? BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return ConsultarCompras(contexto)
                .SingleOrDefault(compra => compra.Id == id);
        }

        public List<CompraCartao> ListarPorCartao(Guid cartaoId)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return ConsultarCompras(contexto)
                .Where(compra => compra.CartaoId == cartaoId)
                .OrderBy(compra => compra.Data)
                .ThenBy(compra => compra.Id)
                .ToList();
        }

        FaturaCartao? IFaturaCartaoRepositorio.BuscarPorId(Guid id)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return ConsultarFaturas(contexto)
                .AsNoTrackingWithIdentityResolution()
                .SingleOrDefault(fatura => fatura.Id == id);
        }

        List<FaturaCartao> IFaturaCartaoRepositorio.ListarPorCartao(Guid cartaoId)
        {
            using JKCASHDbContext contexto = _criarContexto();

            return ConsultarFaturas(contexto)
                .AsNoTrackingWithIdentityResolution()
                .Where(fatura => fatura.CartaoId == cartaoId)
                .OrderBy(fatura => fatura.Ciclo.Fechamento)
                .ThenBy(fatura => fatura.Id)
                .ToList();
        }

        public void RegistrarPagamento(
            FaturaCartao original,
            FaturaCartao atualizada,
            Transacao transacao,
            Conta conta,
            CartaoCredito cartao)
        {
            ArgumentNullException.ThrowIfNull(original);
            ArgumentNullException.ThrowIfNull(atualizada);
            ArgumentNullException.ThrowIfNull(transacao);
            ArgumentNullException.ThrowIfNull(conta);
            ArgumentNullException.ThrowIfNull(cartao);

            if (atualizada.Id != original.Id || atualizada.CartaoId != original.CartaoId
                || original.CartaoId != cartao.Id
                || atualizada.Pagamentos.Count != original.Pagamentos.Count + 1
                || !MesmosIds(original.Pagamentos.Select(p => p.Id), atualizada.Pagamentos.SkipLast(1).Select(p => p.Id))
                || !MesmosIds(original.Parcelas.Select(p => p.Id), atualizada.Parcelas.Select(p => p.Id)))
            {
                throw new ArgumentException("A atualização não corresponde à fatura original.", nameof(atualizada));
            }

            PagamentoFatura pagamento = atualizada.Pagamentos.Last();
            if (pagamento.FaturaId != original.Id || pagamento.ContaId != conta.Id
                || pagamento.TransacaoId != transacao.Id || transacao.ContaId != conta.Id
                || pagamento.Valor != transacao.Valor || transacao.Tipo != TipoTransacao.Saida
                || pagamento.Data != transacao.Data.Date)
            {
                throw new ArgumentException("Pagamento e saída da conta não correspondem à fatura.");
            }

            using JKCASHDbContext contexto = _criarContexto();
            OperacaoFinanceira.Executar(contexto, () =>
            {

                FaturaCartao persistida = ConsultarFaturas(contexto).SingleOrDefault(f => f.Id == original.Id)
                    ?? throw new ConflitoFinanceiroException("A fatura não está mais disponível. Atualize os dados.");

                if (persistida.CartaoId != original.CartaoId || persistida.Ciclo != original.Ciclo
                    || persistida.Total != original.Total || persistida.TotalPago != original.TotalPago
                    || !MesmosIds(persistida.Pagamentos.Select(p => p.Id), original.Pagamentos.Select(p => p.Id))
                    || !MesmosIds(persistida.Parcelas.Select(p => p.Id), original.Parcelas.Select(p => p.Id)))
                {
                    throw new ConflitoFinanceiroException("A fatura foi alterada. Consulte novamente antes de pagar.");
                }

                Conta contaPersistida = contexto.Contas.SingleOrDefault(c => c.Id == conta.Id)
                    ?? throw new ConflitoFinanceiroException("A conta não está mais disponível. Atualize os dados.");
                CartaoCredito cartaoPersistido = contexto.CartoesCredito.SingleOrDefault(c => c.Id == cartao.Id)
                    ?? throw new ConflitoFinanceiroException("O cartão não está mais disponível. Atualize os dados.");

                OperacaoFinanceira.ConferirConta(contaPersistida, conta, conta.Saldo.SaldoAtual + pagamento.Valor);
                if (cartaoPersistido.ValorUtilizado != cartao.ValorUtilizado + pagamento.Valor
                    || cartaoPersistido.Limite != cartao.Limite || cartaoPersistido.ContaId != cartao.ContaId)
                {
                    throw new ConflitoFinanceiroException("O cartão foi alterado. Consulte o limite antes de pagar novamente.");
                }

                _ = persistida.ComPagamento(pagamento);
                contaPersistida.Debitar(pagamento.Valor);
                cartaoPersistido.RegistrarPagamento(pagamento.Valor);
                contexto.Transacoes.Add(transacao);
                contexto.PagamentosFatura.Add(pagamento);

                // Não chama outro repositório: todas as alterações usam este contexto e transação.
            });
        }

        private static IQueryable<CompraCartao> ConsultarCompras(JKCASHDbContext contexto)
        {
            return contexto.ComprasCartao
                .AsNoTrackingWithIdentityResolution()
                .Include(compra => compra.Parcelas)
                    .ThenInclude(parcela => parcela.Fatura)
                .AsSplitQuery();
        }

        private static IQueryable<FaturaCartao> ConsultarFaturas(JKCASHDbContext contexto)
        {
            // Parcelas e pagamentos completos são necessários para calcular os totais.
            return contexto.FaturasCartao
                .Include(fatura => fatura.Parcelas)
                .Include(fatura => fatura.Pagamentos)
                .AsSplitQuery();
        }

        private static bool MesmosIds(IEnumerable<Guid> primeiros, IEnumerable<Guid> segundos)
        {
            return primeiros.OrderBy(id => id).SequenceEqual(segundos.OrderBy(id => id));
        }
    }
}
