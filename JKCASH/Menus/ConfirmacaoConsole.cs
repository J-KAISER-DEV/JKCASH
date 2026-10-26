namespace JKCASH.Menus;

public static class ConfirmacaoConsole
{
    public static bool Confirmar(string operacao, string resumo)
    {
        Console.WriteLine($"\n=== {operacao} ===\n{resumo}");
        Console.Write("Confirmar? [S/N] (Enter cancela): ");
        bool confirmou = string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase);
        if (!confirmou) Console.WriteLine("Operação cancelada. Nenhum registro foi gravado.");
        return confirmou;
    }
}
