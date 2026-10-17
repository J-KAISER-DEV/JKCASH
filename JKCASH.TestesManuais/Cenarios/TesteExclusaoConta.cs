using System.Transactions;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Infrastructure.Persistencia;

namespace JKCASH.TestesManuais.Cenarios
{
    public static class TesteExclusaoConta
    {
        public static void Executar(bool usarSql)
        {
            // No SQL, todas as inclusões e exclusões deste teste serão desfeitas.
            using TransactionScope? transacao = usarSql ? new TransactionScope() : null;
            ITransacaoRepositorio transacoes = usarSql ? new TransacaoRepositorioSql() : new TransacaoRepositoEmMemoria();
            IContaRepositorio contas = usarSql ? new ContaRepositorioSql()
                : new ContaRepositorioEmMemoria((TransacaoRepositoEmMemoria)transacoes);
            ICartaoCreditoRepositorio cartoes = usarSql ? new CartaoCreditoRepositorioSql() : new CartaoCreditoRepositorioEmMemoria();
            ICategoriaRepositorio categorias = usarSql ? new CategoriaRepositorioSql() : new CategoriaRepositorioEmMemoria();
            CompraCartaoRepositorio comprasMemoria = new(transacoes);
            IFaturaCartaoRepositorio faturas = usarSql ? new CompraCartaoRepositorioSql() : comprasMemoria;
            ContaServico servico = new(contas, transacoes, cartoes, faturas);
            string sufixo = Guid.NewGuid().ToString("N");
            Conta livre = new($"Excluir {sufixo}", "Teste", TipoConta.Corrente);
            Conta comTransacao = new($"Histórico {sufixo}", "Teste", TipoConta.Corrente);
            Conta comCartao = new($"Cartão {sufixo}", "Teste", TipoConta.Corrente);
            contas.Adicionar(livre);
            contas.Adicionar(comTransacao);
            contas.Adicionar(comCartao);

            Categoria categoria = new($"Exclusão {sufixo}");
            categorias.Adicionar(categoria);
            Transacao movimento = new(comTransacao.Id, TipoTransacao.Entrada, 10m,
                "Entrada de teste", DateTime.Today, categoria.Id, ImportanciaTransacao.Baixa,
                PlanejamentoTransacao.Planejada, UrgenciaTransacao.SemUrgencia, "Verificar proteção do histórico");
            comTransacao.Creditar(10m);
            transacoes.Adicionar(movimento, comTransacao);
            CartaoCredito cartao = new(comCartao.Id, "Cartão de teste", 100m, 10, 20);
            cartoes.Adicionar(cartao);

            servico.Excluir(livre.Id);
            Conferir(!contas.ListarTodas().Any(conta => conta.Id == livre.Id), "Conta livre não foi removida.");
            EsperarBloqueio(() => servico.Excluir(livre.Id));
            EsperarBloqueio(() => servico.Excluir(comTransacao.Id));
            EsperarBloqueio(() => servico.Excluir(comCartao.Id));

            bool vazioRecusado = false;
            try { servico.Excluir(Guid.Empty); }
            catch (ArgumentException) { vazioRecusado = true; }
            Conferir(vazioRecusado, "Identificador vazio foi aceito.");
            Conferir(contas.ListarTodas().Any(conta => conta.Id == comTransacao.Id), "Conta com histórico foi removida.");
            Conferir(contas.ListarTodas().Any(conta => conta.Id == comCartao.Id), "Conta com cartão foi removida.");
            Conferir(transacoes.BuscarPorId(movimento.Id) is not null, "Transação foi removida.");
            Conferir(cartoes.BuscarPorId(cartao.Id) is not null, "Cartão foi removido.");

            if (usarSql)
            {
                // Confere também a proteção do repositório quando chamado diretamente.
                EsperarBloqueio(() => contas.Remover(comTransacao.Id));
                EsperarBloqueio(() => contas.Remover(comCartao.Id));
            }

            EsperarBloqueio(() => servico.Excluir(comCartao.Id, excluirTransacoes: true));
            servico.Excluir(comTransacao.Id, excluirTransacoes: true);
            Conferir(!contas.ListarTodas().Any(conta => conta.Id == comTransacao.Id), "Conta com transações não foi removida após confirmação.");
            Conferir(transacoes.BuscarPorId(movimento.Id) is null, "A transação ficou órfã.");
            Conferir(categorias.Existe(categoria.Id), "A categoria compartilhada foi removida.");

            // A conta pagadora não tem cartão próprio, mas seu pagamento precisa ser preservado.
            Conta pagadora = new($"Pagadora {sufixo}", "Teste", TipoConta.Corrente, saldoInicial: 100m);
            contas.Adicionar(pagadora);
            PagamentoFatura pagamento;
            if (usarSql)
            {
                using var contexto = ConexaoSql.CriarContexto();
                var cartaoPersistido = contexto.CartoesCredito.Single(c => c.Id == cartao.Id);
                var pagadoraPersistida = contexto.Contas.Single(c => c.Id == pagadora.Id);
                CompraCartao compra = new(cartaoPersistido, 10m, "Compra teste", DateTime.Today,
                    categoria.Id, ImportanciaTransacao.Baixa, PlanejamentoTransacao.Planejada,
                    UrgenciaTransacao.SemUrgencia, "Teste de preservação de pagamento");
                FaturaCartao fatura = FaturaCartao.Criar(compra.Parcelas[0]);
                Transacao saida = new(pagadora.Id, TipoTransacao.Saida, 5m, "Pagamento teste",
                    DateTime.Today, categoria.Id, ImportanciaTransacao.Baixa,
                    PlanejamentoTransacao.Planejada, UrgenciaTransacao.SemUrgencia, "Teste de exclusão");
                pagamento = new(fatura.Id, pagadora.Id, saida.Id, 5m, DateTime.Today);
                cartaoPersistido.RegistrarCompra(10m);
                cartaoPersistido.RegistrarPagamento(5m);
                pagadoraPersistida.Debitar(5m);
                contexto.FaturasCartao.Add(fatura.ComPagamento(pagamento));
                contexto.ComprasCartao.Add(compra);
                contexto.Transacoes.Add(saida);
                contexto.SaveChanges();
            }
            else
            {
                CompraCartaoServico compras = new(cartoes, categorias, comprasMemoria);
                compras.Registrar(cartao.Id, 10m, "Compra teste", DateTime.Today, categoria.Id,
                    ImportanciaTransacao.Baixa, PlanejamentoTransacao.Planejada,
                    UrgenciaTransacao.SemUrgencia, "Teste de preservação de pagamento");
                PagamentoFaturaServico pagamentos = new(faturas, cartoes, contas, categorias);
                pagamento = pagamentos.Registrar(faturas.ListarPorCartao(cartao.Id)[0].Id,
                    pagadora.Id, 5m, categoria.Id);
            }

            EsperarBloqueio(() => servico.Excluir(pagadora.Id, excluirTransacoes: true));
            if (usarSql) EsperarBloqueio(() => contas.RemoverComTransacoes(pagadora.Id));
            Conferir(contas.ListarTodas().Single(c => c.Id == pagadora.Id).Saldo.SaldoAtual == 95m,
                "O bloqueio alterou a conta pagadora.");
            Conferir(transacoes.BuscarPorId(pagamento.TransacaoId) is not null, "O bloqueio apagou a saída do pagamento.");
            Conferir(faturas.BuscarPorId(pagamento.FaturaId)!.Pagamentos.Any(p => p.Id == pagamento.Id),
                "O pagamento foi removido.");
            Conferir(cartoes.BuscarPorId(cartao.Id)!.ValorUtilizado == 5m, "O bloqueio alterou o limite do cartão.");
            Console.WriteLine($"OK: exclusão conjunta ({(usarSql ? "SQL com rollback" : "memória")}), categoria preservada, cartões e pagamentos protegidos.");
        }

        private static void EsperarBloqueio(Action acao)
        {
            try { acao(); }
            catch (InvalidOperationException) { return; }
            throw new InvalidOperationException("Uma exclusão inválida deveria ter sido bloqueada.");
        }

        private static void Conferir(bool condicao, string mensagem)
        {
            if (!condicao) throw new InvalidOperationException(mensagem);
        }
    }
}
