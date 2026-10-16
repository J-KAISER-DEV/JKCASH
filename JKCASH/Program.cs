using System.Globalization;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Inicializacao;
using JKCASH.Menus;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

bool modoMemoria = ConfiguracaoExecucao.UsarMemoria(args);

// 1. Repositórios: implementações usadas nesta execução da aplicação.
ICartaoCreditoRepositorio cartoes = ConfiguracaoExecucao.CriarRepositorioCartoes(modoMemoria);
IContaRepositorio contas = ConfiguracaoExecucao.CriarRepositorioContas(modoMemoria);
ICategoriaRepositorio categorias = ConfiguracaoExecucao.CriarRepositorioCategorias(modoMemoria);
ITransacaoRepositorio transacoes = ConfiguracaoExecucao.CriarRepositorioTransacoes(modoMemoria);

// 2. Serviço: coordena o registro da transação e a movimentação do saldo.
TransacaoServico servico = new(transacoes, categorias);
var (comprasCartao, faturas) =
    ConfiguracaoExecucao.CriarRepositorioMovimentacoesCartao(modoMemoria, transacoes);
CompraCartaoServico servicoCompras = new(cartoes, categorias, comprasCartao);
PagamentoFaturaServico servicoPagamentos = new(faturas, cartoes, contas, categorias);

// 3. Menus: recebem as dependências para coletar dados e mostrar resultados.
MenuContas menuContas = new(contas);
MenuCategorias menuCategorias = new(categorias);
MenuTransacoes menuTransacoes = new(contas, categorias, transacoes, servico);
MenuComprasCartao menuCompras = new(cartoes, categorias, comprasCartao, servicoCompras);
MenuCartao menuCartao = new(cartoes, contas, menuCompras);
MenuFaturas menuFaturas = new(cartoes, faturas, comprasCartao,
    contas, categorias, servicoPagamentos);
MenuPrincipal menuPrincipal = new(menuContas, menuCategorias, menuTransacoes, menuCartao,
    menuFaturas);

// 4. Inicialização do Console.
if (modoMemoria)
{
    Console.WriteLine("JKCASH — modo em memória, sem conexão com o banco.");
    Console.WriteLine("Os dados serão perdidos ao encerrar o programa.");
}
else
{
    Console.WriteLine("JKCASH — modo SQL. Todos os cadastros e movimentações são salvos no banco.");
    DiagnosticoBanco.TestarConexao();
}
menuPrincipal.Executar();
