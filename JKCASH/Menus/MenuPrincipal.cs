using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Menus
{
    public class MenuPrincipal
    {
        private readonly MenuContas _contas;
        private readonly MenuCategorias _categorias;
        private readonly MenuTransacoes _transacoes;

        public MenuPrincipal(
            MenuContas contas,
            MenuCategorias categorias,
            MenuTransacoes transacoes)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(transacoes);

            _contas = contas;
            _categorias = categorias;
            _transacoes = transacoes;
        }

        // Mantém o menu ativo e trata as falhas apresentadas ao usuário.
        public void Executar()
        {
            while (true)
            {
                MostrarMenu();
                Console.Write("Escolha: ");
                string? opcao = Console.ReadLine();

                if (opcao is null || opcao == "0")
                {
                    Console.WriteLine("Programa encerrado.");
                    return;
                }

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            _contas.CadastrarConta();
                            break;
                        case "2":
                            _contas.ListarContas();
                            break;
                        case "3":
                            _transacoes.RegistrarTransacao();
                            break;
                        case "4":
                            _transacoes.ListarTransacoes();
                            break;
                        case "5":
                            _transacoes.BuscarTransacao();
                            break;
                        case "6":
                            _transacoes.ListarTransacoesPorConta();
                            break;
                        case "7":
                            _categorias.CadastrarCategoria();
                            break;
                        case "8":
                            _categorias.ListarCategorias();
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
                catch (ArgumentException erro)
                {
                    Console.WriteLine($"Não foi possível concluir: {erro.Message}");
                }
                catch (InvalidOperationException erro)
                {
                    Console.WriteLine($"Não foi possível concluir: {erro.Message}");
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("Entrada encerrada.");
                    return;
                }
                catch (DbUpdateException erro) when (
                    erro.InnerException is SqlException sqlErro &&
                    (sqlErro.Number == 2601 || sqlErro.Number == 2627))
                {
                    Console.WriteLine("Já existe uma conta com esse nome e banco, ou uma categoria com esse nome.");
                }
                catch (DbUpdateException erro)
                {
                    Console.WriteLine($"Não foi possível salvar no banco: {erro.GetBaseException().Message}");
                }
                catch(SqlException erro)
                {
                    Console.WriteLine($"Nao foi possivel acessar o banco: {erro.Message}");
                }
            }
        }

        private static void MostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1 - Cadastrar conta");
            Console.WriteLine("2 - Listar contas");
            Console.WriteLine("3 - Registrar transação");
            Console.WriteLine("4 - Listar transações");
            Console.WriteLine("5 - Buscar transação por Id");
            Console.WriteLine("6 - Listar transações por conta");
            Console.WriteLine("7 - Cadastrar categoria");
            Console.WriteLine("8 - Listar categorias");
            Console.WriteLine("0 - Sair");
        }
    }
}
