namespace JKCASH.WinForms
{
    using JKCASH.Infrastructure.Persistencia;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            System.Globalization.CultureInfo.CurrentCulture =
                System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

            FileStream instancia;
            try { instancia = Inicializacao.BloqueioAplicacao.Abrir(); }
            catch (IOException)
            {
                MessageBox.Show("Não foi possível reservar os dados locais. Feche outra instância do JKCASH e tente novamente.",
                    "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Não foi possível acessar a pasta de dados locais. Confira as permissões do usuário.",
                    "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using var bloqueio = instancia;

            // Cada retorno cria uma tela inicial nova, atualizando conexão e histórico.
            while (true)
            {
                using (FrmInicial inicio = Inicializacao.FabricaTelas.CriarInicial())
                {
                    if (inicio.ShowDialog() != DialogResult.OK)
                        return;
                }

                try
                {
                    ConexaoLocal.Inicializar();
                }
                catch (Exception erro)
                {
                    MessageBox.Show(
                        $"Não foi possível preparar o banco local.\n\n" +
                        $"Arquivo: {ConexaoLocal.CaminhoBanco}\n\n{erro.GetBaseException().Message}",
                        "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                using FrmPrincipal principal = new();
                principal.ShowDialog();
            }
        }
    }
}
