using System.Globalization;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Inicializacao;
using JKCASH.Menus;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// 1. Repositórios: implementações usadas nesta execução da aplicação.
IContaRepositorio contas = new ContaRepositorioSql();
ICategoriaRepositorio categorias = new CategoriaRepositorioSql();
ITransacaoRepositorio transacoes = new TransacaoRepositorioSql();

// 2. Serviço: coordena o registro da transação e a movimentação do saldo.
TransacaoServico servico = new(transacoes, categorias);

// 3. Menus: recebem as dependências para coletar dados e mostrar resultados.
MenuContas menuContas = new(contas);
MenuCategorias menuCategorias = new(categorias);
MenuTransacoes menuTransacoes = new(contas, categorias, transacoes, servico);
MenuPrincipal menuPrincipal = new(menuContas, menuCategorias, menuTransacoes);

// 4. Inicialização do Console.
Console.WriteLine("JKCASH — contas, categorias e transações salvas no SQL.");
DiagnosticoBanco.TestarConexao();
menuPrincipal.Executar();
