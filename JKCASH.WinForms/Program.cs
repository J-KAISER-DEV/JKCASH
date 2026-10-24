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
                return;
            }

            System.Windows.Forms.Application.Run(new FrmPrincipal());
        }
    }
}
