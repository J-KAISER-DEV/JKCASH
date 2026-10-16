using System.Globalization;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Inicializacao;
using JKCASH.Menus;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

bool modoMemoria = ConfiguracaoExecucao.UsarMemoria(args);

// 1. Repositórios: implementações usadas nesta execução da aplicação.
ICartaoCreditoRepositorio cartoes =
    new CartaoCreditoRepositorioEmMemoria();
IContaRepositorio contas = ConfiguracaoExecucao.CriarRepositorioContas(modoMemoria);
ICategoriaRepositorio categorias = ConfiguracaoExecucao.CriarRepositorioCategorias(modoMemoria);
ITransacaoRepositorio transacoes = ConfiguracaoExecucao.CriarRepositorioTransacoes(modoMemoria);

// 2. Serviço: coordena o registro da transação e a movimentação do saldo.
TransacaoServico servico = new(transacoes, categorias);
CompraCartaoRepositorio movimentacoesCartao = new(modoMemoria ? transacoes : null);
CompraCartaoServico servicoCompras = new(cartoes, categorias, movimentacoesCartao);
PagamentoFaturaServico? servicoPagamentos = modoMemoria
    ? new PagamentoFaturaServico(movimentacoesCartao, cartoes, contas, categorias)
    : null;

// 3. Menus: recebem as dependências para coletar dados e mostrar resultados.
MenuCartao menuCartao = new(cartoes, contas);
MenuContas menuContas = new(contas);
MenuCategorias menuCategorias = new(categorias);
MenuTransacoes menuTransacoes = new(contas, categorias, transacoes, servico);
MenuComprasCartao menuCompras = new(cartoes, categorias, movimentacoesCartao, servicoCompras);
MenuFaturas menuFaturas = new(cartoes, movimentacoesCartao, movimentacoesCartao,
    contas, categorias, servicoPagamentos);
MenuPrincipal menuPrincipal = new(menuContas, menuCategorias, menuTransacoes, menuCartao,
    menuCompras, menuFaturas);

// 4. Inicialização do Console.
if (modoMemoria)
{
    Console.WriteLine("JKCASH — modo em memória, sem conexão com o banco.");
    Console.WriteLine("Os dados serão perdidos ao encerrar o programa.");
}
else
{
    Console.WriteLine("JKCASH — contas, categorias e transações salvas no SQL.");
    DiagnosticoBanco.TestarConexao();
}
Console.WriteLine("Cartões, compras e faturas ficam em memória e serão perdidos ao encerrar o programa.");
if (!modoMemoria)
    Console.WriteLine("Para registrar pagamentos de faturas, execute no perfil JKCASH (Memoria).");
menuPrincipal.Executar();
