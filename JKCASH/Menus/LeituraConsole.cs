using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JKCASH.Menus
{
    public static class LeituraConsole
    {
        public static string LerTexto(string mensagem)
        {
            Console.Write(mensagem);
            return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
        }
        public static decimal LerDecimal(string mensagem)
        {
            string texto = LeituraConsole.LerTexto(mensagem);

            if (!decimal.TryParse(texto,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.CurrentCulture, out decimal valor))
            {
                throw new ArgumentException("Digite um número válido usando vírgula para os centavos.");
            }

            return valor;
        }
        public static DateTime LerData(string mensagem)
        {
            string texto = LeituraConsole.LerTexto(mensagem);

            if (!DateTime.TryParseExact(texto, "dd/MM/yyyy",
                CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime data))
            {
                throw new ArgumentException("Informe uma data válida no formato dd/MM/aaaa.");
            }

            return data;
        }
        public static int LerNumeroDaLista(string mensagem, int quantidade)
        {
            string texto = LeituraConsole.LerTexto(mensagem);

            if (!int.TryParse(texto, out int numero) || numero < 1 || numero > quantidade)
            {
                throw new ArgumentException("Escolha um número da lista.");
            }

            return numero;
        }
       public static T LerOpcao<T>() where T : struct, Enum
        {
            Console.WriteLine($"Opções de {typeof(T).Name}:");

            foreach (T opcao in Enum.GetValues<T>())
            {
                Console.WriteLine($"{Convert.ToInt32(opcao)} - {opcao}");
            }

            if (!int.TryParse(LeituraConsole.LerTexto("Escolha o número: "), out int numero))
            {
                throw new ArgumentException("Digite o número de uma opção.");
            }

            T escolha = (T)Enum.ToObject(typeof(T), numero);

            if (!Enum.IsDefined(escolha))
            {
                throw new ArgumentException("Escolha uma opção da lista.");
            }

            return escolha;
        }

        public static int LerInteiro(string mensagem)
        {
            string texto = LerTexto(mensagem);
            if (!int.TryParse(texto,out int numero))
            {
                throw new ArgumentException("Digite um numero inteiro valido");
            }

            return numero;
        }
    }
}
