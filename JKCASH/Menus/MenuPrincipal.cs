using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Menus
{
    public class MenuPrincipal
    {
        private readonly MenuContas _contas;
        private readonly MenuCartao _cartoes;
        private readonly MenuCategorias _categorias;
        private readonly MenuTransacoes _transacoes;
        private readonly MenuComprasCartao _compras;
        private readonly MenuFaturas _faturas;

        public MenuPrincipal(
            MenuContas contas,
            MenuCategorias categorias,
            MenuTransacoes transacoes,
            MenuCartao cartoes,
            MenuComprasCartao compras,
            MenuFaturas faturas)
        {
            ArgumentNullException.ThrowIfNull(contas);
            ArgumentNullException.ThrowIfNull(categorias);
            ArgumentNullException.ThrowIfNull(transacoes);
            ArgumentNullException.ThrowIfNull(cartoes);
            ArgumentNullException.ThrowIfNull(compras);
            ArgumentNullException.ThrowIfNull(faturas);



            _contas = contas;
            _categorias = categorias;
            _transacoes = transacoes;
            _cartoes = cartoes;
            _compras = compras;
            _faturas = faturas;
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
                        case "9":
                            _cartoes.CadastrarCartao();
                            break;

                        case "10":
                            _cartoes.ListarCartoes();
                            break;
                        case "11":
                            _compras.RegistrarCompra();
                            break;
                        case "12":
                            _compras.ListarCompras();
                            break;
                        case "13":
                            _compras.BuscarCompra();
                            break;
                        case "14":
                            _faturas.ListarFaturas();
                            break;
                        case "15":
                            _faturas.ConsultarFatura();
                            break;
                        case "16":
                            _faturas.PagarFatura();
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
            Console.WriteLine("9 - Cadastrar cartão de crédito");
            Console.WriteLine("10 - Listar cartões de crédito");
            Console.WriteLine("11 - Registrar compra no cartão (à vista ou parcelada)");
            Console.WriteLine("12 - Listar compras de um cartão");
            Console.WriteLine("13 - Buscar compra por Id");
            Console.WriteLine("14 - Listar faturas de um cartão");
            Console.WriteLine("15 - Consultar parcelas e pagamentos de uma fatura");
            Console.WriteLine("16 - Registrar pagamento total ou parcial de fatura");
            Console.WriteLine("0 - Sair");
        }
    }
}
