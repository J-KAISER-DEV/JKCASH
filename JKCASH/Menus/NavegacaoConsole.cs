using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JKCASH.Menus;

// Reutiliza a navegação e o tratamento de erros em todos os níveis do menu.
public static class NavegacaoConsole
{
    public static void Executar(
        string titulo,
        string opcaoZero,
        params (string Texto, Action Acao)[] opcoes)
    {
        while (true)
        {
            Console.WriteLine($"\n=== {titulo} ===");
            for (int i = 0; i < opcoes.Length; i++)
                Console.WriteLine($"{i + 1} - {opcoes[i].Texto}");
            Console.WriteLine($"0 - {opcaoZero}");
            Console.Write("Escolha: ");

            string? entrada = Console.ReadLine();
            if (entrada is null) return;

            if (!int.TryParse(entrada, out int numero) || numero < 0 || numero > opcoes.Length)
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }

            if (numero == 0) return;

            try
            {
                opcoes[numero - 1].Acao();
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
                erro.InnerException is SqlException sqlErro && sqlErro.Number is 2601 or 2627)
            {
                Console.WriteLine("Já existe um registro com esses dados.");
            }
            catch (DbUpdateException erro)
            {
                Console.WriteLine($"Não foi possível salvar no banco: {erro.GetBaseException().Message}");
            }
            catch (SqlException erro)
            {
                Console.WriteLine($"Não foi possível acessar o banco: {erro.Message}");
            }
        }
    }
}
