namespace JKCASH.WinForms.Visual;

internal static class FalhaTela
{
    // Erros de infraestrutura encapsulados não devem ser apresentados como validação.
    public static bool EhValidacao(Exception erro) =>
        (erro is ArgumentException or InvalidOperationException) && erro.InnerException is null;
}
