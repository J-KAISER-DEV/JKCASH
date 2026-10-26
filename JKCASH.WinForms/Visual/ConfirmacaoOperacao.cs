namespace JKCASH.WinForms.Visual;

public interface IConfirmacaoOperacao
{
    bool Confirmar(IWin32Window dono, string operacao, string resumo);
}

public sealed class ConfirmacaoOperacao : IConfirmacaoOperacao
{
    public static IConfirmacaoOperacao Padrao { get; } = new ConfirmacaoOperacao();

    public bool Confirmar(IWin32Window dono, string operacao, string resumo) =>
        MessageBox.Show(dono, resumo + "\n\nDeseja confirmar esta operação?", operacao,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) == DialogResult.Yes;
}
