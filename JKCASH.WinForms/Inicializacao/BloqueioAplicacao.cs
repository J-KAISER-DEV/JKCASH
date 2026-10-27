using JKCASH.Infrastructure.Persistencia;

namespace JKCASH.WinForms.Inicializacao;

internal static class BloqueioAplicacao
{
    // Mantido aberto até o encerramento, inclusive durante a restauração.
    public static FileStream Abrir()
    {
        ConexaoLocal.CriarPasta();
        return new FileStream(Path.Combine(ConexaoLocal.PastaDados, "aplicacao.lock"),
            FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }
}
