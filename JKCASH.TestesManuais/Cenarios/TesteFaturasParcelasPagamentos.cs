using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Domain.ObjetosDeValor;
using JKCASH.Infrastructure.Repositorios;

namespace JKCASH.TestesManuais.Cenarios;

public static class TesteFaturasParcelasPagamentos
{
    public static void Executar()
    {
        Console.WriteLine("\n=== Compras, parcelas, faturas e pagamentos: verificações automáticas ===");
        Testar("Compra parcelada consome total e preserva saldo", CompraParcelada);
        Testar("À vista gera uma parcela", CompraAVista);
        Testar("Centavos e soma exata das parcelas", Arredondamento);
        Testar("Duas escolhas para o dia de fechamento", OpcoesFechamento);
        Testar("Meses curtos, ano bissexto e virada de ano", Calendario);
        Testar("Agrupamento por cartão e ciclo", Agrupamento);
        Testar("Fechamento, quitação e atraso", Estados);
        Testar("Vários pagamentos parciais e quitação", PagamentosParciais);
        Testar("Pagamento pode sair de outra conta", OutraConta);
        Testar("Fatura aberta quitada pode receber novas compras", PagamentoAntecipado);
        Testar("Pagamento não quita parcelas dos próximos meses", ParcelasFuturas);
        Testar("Valores inválidos e pagamento excedente", ValoresInvalidos);
        Testar("Saldo insuficiente preserva todos os valores", SaldoInsuficiente);
        Testar("Conta inativa e referências inexistentes", ReferenciasInvalidas);
        Testar("Compra acima do limite não cria faturas", LimiteInsuficiente);
        Testar("Falha ao salvar pagamento desfaz saldo e limite", FalhaPagamento);
        Testar("Compra futura e ciclo encerrado são recusados", DatasInvalidas);
        Testar("Repetição de compra não duplica fatura", CompraDuplicada);
        Testar("Fatura antiga não pode ser salva por cima de pagamento novo", VersaoAntiga);
        Console.WriteLine("19 cenários automáticos concluídos com sucesso.");
    }

    private static void Testar(string nome, Action teste)
    {
        teste();
        Console.WriteLine($"OK: {nome}");
    }

    private static void Conferir(bool condicao, string mensagem)
    {
        if (!condicao) throw new InvalidOperationException(mensagem);
    }

    private static void Recusar<T>(Action acao) where T : Exception
    {
        try { acao(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Era esperado {typeof(T).Name}.");
    }

    private sealed class Relogio : TimeProvider
    {
        public DateTime Hoje { get; set; } = new(2026, 1, 9);
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(Hoje, DateTimeKind.Utc));
    }

    private sealed class Cenario
    {
        public Relogio Relogio { get; } = new();
        public ContaRepositorioEmMemoria Contas { get; } = new();
        public CartaoCreditoRepositorioEmMemoria Cartoes { get; } = new();
        public CategoriaRepositorioEmMemoria Categorias { get; } = new();
        public TransacaoRepositoEmMemoria Transacoes { get; } = new();
        public Conta Conta { get; } = new("Principal", "Banco A", TipoConta.Corrente, saldoInicial: 1000m);
        public Categoria Categoria { get; } = new("Pagamento de cartão");
        public CartaoCredito Cartao { get; }
        public CompraCartaoRepositorio Compras { get; }
        public IFaturaCartaoRepositorio Faturas => Compras;
        public CompraCartaoServico Servico { get; }
        public PagamentoFaturaServico Pagamentos { get; }

        public Cenario(ITransacaoRepositorio? transacoes = null)
        {
            Cartao = new(Conta.Id, "Principal", 1000m, 10, 20);
            Contas.Adicionar(Conta);
            Cartoes.Adicionar(Cartao);
            Categorias.Adicionar(Categoria);
            Compras = new(transacoes ?? Transacoes, Relogio);
            Servico = new(Cartoes, Categorias, Compras, Relogio);
            Pagamentos = new(Compras, Cartoes, Contas, Categorias, Relogio);
        }

        public CompraCartao Comprar(decimal valor = 300m, int parcelas = 3, Guid? cartaoId = null,
            DateTime? data = null, Guid? categoriaId = null) => Servico.Registrar(
                cartaoId ?? Cartao.Id, valor, "Livros", data ?? Relogio.Hoje,
                categoriaId ?? Categoria.Id, ImportanciaTransacao.Alta,
                PlanejamentoTransacao.Planejada, UrgenciaTransacao.SemUrgencia, "Faculdade", parcelas);

        public FaturaCartao Primeira => Faturas.ListarPorCartao(Cartao.Id)[0];
        public PagamentoFatura Pagar(decimal valor, Guid? contaId = null, Guid? categoriaId = null) =>
            Pagamentos.Registrar(Primeira.Id, contaId ?? Conta.Id, valor, categoriaId ?? Categoria.Id);
    }

    private static void CompraParcelada()
    {
        Cenario c = new();
        CompraCartao compra = c.Comprar();
        Conferir(c.Cartao.LimiteDisponivel == 700m && c.Conta.Saldo.SaldoAtual == 1000m, "Compra movimentou valores incorretos.");
        Conferir(compra.Parcelas.Count == 3 && c.Faturas.ListarPorCartao(c.Cartao.Id).Count == 3, "Parcelas/faturas ausentes.");
        Conferir(compra.Parcelas.All(parcela => parcela.Valor == 100m), "Valores das parcelas incorretos.");
        Conferir(c.Compras.BuscarPorId(compra.Id) == compra && c.Compras.ListarPorCartao(Guid.NewGuid()).Count == 0, "Consulta incorreta.");
    }

    private static void CompraAVista()
    {
        Cenario c = new();
        CompraCartao compra = c.Comprar(25m, 1);
        Conferir(compra.Parcelas.Single().Valor == 25m && c.Primeira.Total == 25m, "Compra à vista incorreta.");
    }

    private static void Arredondamento()
    {
        Cenario c = new();
        CompraCartao compra = c.Comprar(100m, 3);
        Conferir(compra.Parcelas.Select(p => p.Valor).SequenceEqual(new[] { 33.33m, 33.33m, 33.34m }), "Resíduo incorreto.");
        Conferir(compra.Parcelas.Sum(p => p.Valor) == compra.Valor, "Soma incorreta.");
        CompraCartao centavos = c.Comprar(0.03m, 3);
        Conferir(centavos.Parcelas.All(p => p.Valor == 0.01m), "Parcela mínima incorreta.");
    }

    private static void OpcoesFechamento()
    {
        CartaoCredito proxima = new(Guid.NewGuid(), "Próxima", 1000m, 10, 20, true);
        CartaoCredito atual = new(Guid.NewGuid(), "Atual", 1000m, 10, 20, false);
        DateTime data = new(2026, 1, 10);
        Conferir(CicloFatura.DaCompra(proxima, data).Fechamento == new DateTime(2026, 2, 10), "Opção próxima falhou.");
        CicloFatura cicloAtual = CicloFatura.DaCompra(atual, data);
        Conferir(cicloAtual.Fechamento == data && cicloAtual.Encerramento == data.AddDays(1), "Opção atual falhou.");
        Conferir(CicloFatura.DaCompra(atual, data.AddDays(1)).Fechamento == new DateTime(2026, 2, 10), "Após fechamento falhou.");

        Cenario c = new();
        c.Relogio.Hoje = data;
        CartaoCredito mesmoDia = new(c.Conta.Id, "Mesmo dia", 1000m, 10, 20, false);
        c.Cartoes.Adicionar(mesmoDia);
        c.Comprar(10m, 1, mesmoDia.Id);
        Conferir(c.Faturas.ListarPorCartao(mesmoDia.Id).Single().SituacaoEm(data) == SituacaoFatura.Aberta, "Fechou antes do fim do dia.");
    }

    private static void Calendario()
    {
        CartaoCredito cartao = new(Guid.NewGuid(), "Fim do mês", 1000m, 31, 5);
        Conferir(CicloFatura.DaCompra(cartao, new DateTime(2028, 2, 28)).Fechamento == new DateTime(2028, 2, 29), "Ano bissexto falhou.");
        CicloFatura fevereiro = CicloFatura.DaCompra(cartao, new DateTime(2026, 2, 27));
        Conferir(fevereiro.Fechamento == new DateTime(2026, 2, 28) && fevereiro.Vencimento == new DateTime(2026, 3, 5), "Fevereiro falhou.");
        CicloFatura dezembro = CicloFatura.DaCompra(cartao, new DateTime(2026, 12, 31));
        Conferir(dezembro.Fechamento == new DateTime(2027, 1, 31) && dezembro.Vencimento == new DateTime(2027, 2, 5), "Virada de ano falhou.");
        Cenario c = new();
        c.Relogio.Hoje = new(2026, 1, 30);
        c.Cartoes.Adicionar(cartao);
        CompraCartao compra = c.Comprar(90m, 3, cartao.Id);
        Conferir(compra.Parcelas.Select(p => p.Ciclo.Fechamento.Day).SequenceEqual(new[] { 31, 28, 31 }), "Dia 31 não foi recuperado após fevereiro.");
        CartaoCredito diasIguais = new(Guid.NewGuid(), "Datas iguais", 1000m, 10, 10);
        Conferir(CicloFatura.DaCompra(diasIguais, new DateTime(2026, 1, 9)).Vencimento == new DateTime(2026, 2, 10), "Vencimento não ficou após fechamento.");
    }

    private static void Agrupamento()
    {
        Cenario c = new();
        c.Comprar();
        Guid id = c.Primeira.Id;
        c.Comprar(60m, 3);
        Conferir(c.Primeira.Total == 120m && c.Primeira.Id == id && c.Primeira.Parcelas.Count == 2, "Compras do mesmo ciclo não agrupadas.");
        CartaoCredito outro = new(c.Conta.Id, "Outro", 1000m, 10, 20);
        c.Cartoes.Adicionar(outro);
        c.Comprar(50m, 1, outro.Id);
        Conferir(c.Faturas.ListarPorCartao(outro.Id).Single().Total == 50m && c.Primeira.Total == 120m, "Cartões misturados.");
    }

    private static void Estados()
    {
        Cenario c = new();
        c.Comprar();
        Conferir(c.Primeira.SituacaoEm(new(2026, 1, 9)) == SituacaoFatura.Aberta, "Aberta incorreta.");
        Conferir(c.Primeira.SituacaoEm(new(2026, 1, 10)) == SituacaoFatura.Fechada, "Fechada incorreta.");
        Conferir(!c.Primeira.EstaAtrasadaEm(new(2026, 1, 20)) && c.Primeira.EstaAtrasadaEm(new(2026, 1, 21)), "Atraso incorreto.");
        c.Relogio.Hoje = new(2026, 1, 21);
        c.Pagar(40m);
        Conferir(c.Primeira.EstaAtrasadaEm(c.Relogio.Hoje) && c.Primeira.SaldoPendente == 60m, "Parcial atrasada incorreta.");
        c.Pagar(60m);
        Conferir(c.Primeira.SituacaoEm(c.Relogio.Hoje) == SituacaoFatura.Paga && !c.Primeira.EstaAtrasadaEm(c.Relogio.Hoje), "Quitação incorreta.");
    }

    private static void PagamentosParciais()
    {
        Cenario c = new();
        c.Comprar(500m, 1);
        c.Relogio.Hoje = new(2026, 1, 10);
        c.Pagar(200m);
        Conferir(c.Primeira.TotalPago == 200m && c.Primeira.SaldoPendente == 300m && c.Primeira.SituacaoPagamento == SituacaoPagamentoFatura.Parcial, "Primeiro parcial incorreto.");
        c.Pagar(100m);
        c.Pagar(200m);
        Conferir(c.Primeira.Pagamentos.Count == 3 && c.Transacoes.ListarTodas().Count == 3, "Histórico incompleto.");
        Conferir(c.Conta.Saldo.SaldoAtual == 500m && c.Cartao.LimiteDisponivel == 1000m, "Saldo/limite incorreto.");
        Conferir(c.Primeira.SituacaoEm(c.Relogio.Hoje) == SituacaoFatura.Paga, "Fatura não quitada.");
        Conferir(c.Primeira.Pagamentos.All(p => c.Transacoes.BuscarPorId(p.TransacaoId)?.Valor == p.Valor), "Saída não vinculada ao pagamento.");
    }

    private static void OutraConta()
    {
        Cenario c = new();
        c.Comprar();
        Conta outra = new("Outra", "Banco B", TipoConta.Corrente, saldoInicial: 250m);
        c.Contas.Adicionar(outra);
        c.Pagar(100m, outra.Id);
        Conferir(outra.Saldo.SaldoAtual == 150m && c.Conta.Saldo.SaldoAtual == 1000m, "Conta errada debitada.");
        Conferir(c.Primeira.Pagamentos.Single().ContaId == outra.Id, "Histórico perdeu conta pagadora.");
    }

    private static void PagamentoAntecipado()
    {
        Cenario c = new();
        c.Comprar(100m, 1);
        c.Pagar(100m);
        Conferir(c.Primeira.SituacaoEm(c.Relogio.Hoje) == SituacaoFatura.Aberta && c.Primeira.SaldoPendente == 0m, "Fatura aberta foi fechada pelo pagamento.");
        c.Comprar(50m, 1);
        Conferir(c.Primeira.Total == 150m && c.Primeira.TotalPago == 100m && c.Primeira.SaldoPendente == 50m, "Compra posterior ao pagamento incorreta.");
    }

    private static void ParcelasFuturas()
    {
        Cenario c = new();
        c.Comprar();
        c.Pagar(100m);
        Conferir(c.Cartao.LimiteDisponivel == 800m && c.Faturas.ListarPorCartao(c.Cartao.Id).Skip(1).Sum(f => f.SaldoPendente) == 200m, "Pagamento liberou parcelas futuras.");
    }

    private static void ValoresInvalidos()
    {
        Cenario c = new();
        c.Comprar();
        foreach (decimal valor in new[] { 0m, -1m, 0.001m, 101m }) Recusar<ArgumentException>(() => c.Pagar(valor));
        foreach (int parcelas in new[] { 0, -1, 121 }) Recusar<ArgumentException>(() => c.Comprar(100m, parcelas));
        Recusar<ArgumentException>(() => c.Comprar(0.02m, 3));
        Conferir(c.Primeira.TotalPago == 0m && c.Transacoes.ListarTodas().Count == 0 && c.Conta.Saldo.SaldoAtual == 1000m, "Operação inválida mudou dados.");
        c.Pagar(100m);
        Recusar<ArgumentException>(() => c.Pagar(1m));
    }

    private static void SaldoInsuficiente()
    {
        Cenario c = new();
        c.Comprar();
        Conta vazia = new("Sem saldo", "Banco", TipoConta.Corrente);
        c.Contas.Adicionar(vazia);
        Recusar<ArgumentException>(() => c.Pagar(50m, vazia.Id));
        Conferir(c.Cartao.ValorUtilizado == 300m && c.Primeira.TotalPago == 0m && vazia.Saldo.SaldoAtual == 0m && c.Transacoes.ListarTodas().Count == 0, "Falha alterou saldo/limite.");
    }

    private static void ReferenciasInvalidas()
    {
        Cenario c = new();
        Recusar<ArgumentException>(() => c.Comprar(cartaoId: Guid.NewGuid()));
        Recusar<ArgumentException>(() => c.Comprar(categoriaId: Guid.NewGuid()));
        c.Comprar();
        Recusar<ArgumentException>(() => c.Pagar(10m, Guid.NewGuid()));
        Recusar<ArgumentException>(() => c.Pagar(10m, categoriaId: Guid.NewGuid()));
        Recusar<ArgumentException>(() => c.Pagamentos.Registrar(Guid.NewGuid(), c.Conta.Id, 10m, c.Categoria.Id));
        c.Conta.Desativar();
        Recusar<InvalidOperationException>(() => c.Pagar(10m));
        Conferir(c.Primeira.TotalPago == 0m && c.Cartao.ValorUtilizado == 300m, "Referência inválida alterou fatura.");
    }

    private static void LimiteInsuficiente()
    {
        Cenario c = new();
        Recusar<ArgumentException>(() => c.Comprar(1001m, 1));
        Conferir(c.Faturas.ListarPorCartao(c.Cartao.Id).Count == 0 && c.Cartao.ValorUtilizado == 0m, "Compra recusada foi salva.");
    }

    private sealed class TransacoesComFalha : ITransacaoRepositorio
    {
        public void Adicionar(Transacao transacao, Conta conta) => throw new InvalidOperationException("Falha simulada ao salvar saída.");
        public Transacao? BuscarPorId(Guid id) => null;
        public List<Transacao> ListarTodas() => new();
        public List<Transacao> ListarPorConta(Guid contaId) => new();
    }

    private static void FalhaPagamento()
    {
        Cenario c = new(new TransacoesComFalha());
        c.Comprar();
        FaturaCartao original = c.Primeira;
        Recusar<InvalidOperationException>(() => c.Pagar(50m));
        Conferir(ReferenceEquals(c.Primeira, original) && c.Primeira.TotalPago == 0m && c.Primeira.Pagamentos.Count == 0, "Falha deixou pagamento na fatura.");
        Conferir(c.Conta.Saldo.SaldoAtual == 1000m && c.Cartao.ValorUtilizado == 300m, "Falha não restaurou saldo/limite.");
    }

    private static void DatasInvalidas()
    {
        Cenario c = new();
        Recusar<ArgumentException>(() => c.Comprar(data: c.Relogio.Hoje.AddDays(1)));
        Recusar<ArgumentException>(() => c.Comprar(data: new DateTime(2025, 12, 1)));
        c.Comprar();
        FaturaCartao fechada = c.Primeira;
        c.Relogio.Hoje = new(2026, 1, 10);
        Recusar<ArgumentException>(() => c.Comprar(data: new DateTime(2026, 1, 9)));
        CompraCartao compra = c.Comprar(60m, 1);
        Conferir(compra.Parcelas[0].Ciclo.Fechamento == new DateTime(2026, 2, 10) && c.Faturas.BuscarPorId(fechada.Id) == fechada, "Compra alterou fatura fechada.");
    }

    private static void CompraDuplicada()
    {
        Cenario c = new();
        CompraCartao compra = c.Comprar();
        Recusar<ArgumentException>(() => c.Compras.Adicionar(compra, c.Cartao));
        Conferir(c.Primeira.Total == 100m && c.Compras.ListarPorCartao(c.Cartao.Id).Count == 1, "Compra duplicada foi salva.");
    }

    private static void VersaoAntiga()
    {
        Cenario c = new();
        c.Comprar();
        FaturaCartao antiga = c.Primeira;
        c.Pagar(10m);
        Transacao saida = new(c.Conta.Id, TipoTransacao.Saida, 20m, "Teste", c.Relogio.Hoje, c.Categoria.Id,
            ImportanciaTransacao.Alta, PlanejamentoTransacao.Planejada, UrgenciaTransacao.SemUrgencia, "Teste");
        PagamentoFatura pagamento = new(antiga.Id, c.Conta.Id, saida.Id, 20m, c.Relogio.Hoje);
        Recusar<InvalidOperationException>(() => c.Faturas.RegistrarPagamento(antiga, antiga.ComPagamento(pagamento), saida, c.Conta, c.Cartao));
        Conferir(c.Primeira.TotalPago == 10m && c.Transacoes.ListarTodas().Count == 1, "Versão antiga sobrescreveu pagamento.");
    }
}
