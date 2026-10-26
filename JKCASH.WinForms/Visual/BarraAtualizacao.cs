namespace JKCASH.WinForms.Visual;

// A barra pertence ao painel e é descartada junto com ele.
internal sealed class BarraAtualizacao
{
    public Button Atualizar { get; }
    private readonly Label _status;

    public BarraAtualizacao(Control painel)
    {
        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 66, ColumnCount = 2, RowCount = 1,
            Padding = new Padding(0, 0, 0, 8)
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        _status = new Label
        {
            AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = EstiloVisual.Secundario, Text = "Consultando dados…"
        };
        Atualizar = EstiloVisual.Botao("Atualizar dados");
        Atualizar.Name = "btnAtualizarDados";
        Atualizar.Dock = DockStyle.Fill;
        barra.Controls.Add(_status, 0, 0);
        barra.Controls.Add(Atualizar, 1, 0);
        painel.Controls.Add(barra);
        // O cabeçalho deve ser disposto antes da tabela com Dock.Fill.
        barra.SendToBack();
    }

    public void Mostrar(string mensagem, bool erro = false)
    {
        _status.Text = mensagem;
        _status.ForeColor = erro ? EstiloVisual.Vermelho : EstiloVisual.Secundario;
    }
}
