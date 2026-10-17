using System.Drawing.Drawing2D;

namespace JKCASH.WinForms.Visual;

// Paleta compartilhada: mudar uma cor aqui mantém as telas consistentes.
internal static class EstiloVisual
{
    public static readonly Color Fundo = Color.FromArgb(244, 246, 248);
    public static readonly Color Menu = Color.FromArgb(20, 38, 61);
    public static readonly Color MenuSelecionado = Color.FromArgb(31, 61, 89);
    public static readonly Color Verde = Color.FromArgb(22, 134, 92);
    public static readonly Color Vermelho = Color.FromArgb(193, 49, 58);
    public static readonly Color Texto = Color.FromArgb(22, 36, 55);
    public static readonly Color Secundario = Color.FromArgb(105, 122, 143);
    public static readonly Color Borda = Color.FromArgb(220, 228, 236);

    public static Label Rotulo(string texto, float tamanho = 10, bool negrito = false)
        => new()
        {
            Text = texto,
            AutoSize = false,
            AutoEllipsis = true,
            ForeColor = Texto,
            Font = new Font("Segoe UI", tamanho, negrito ? FontStyle.Bold : FontStyle.Regular),
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

    public static Button Botao(string texto, bool principal = false)
    {
        Button botao = new()
        {
            Text = texto,
            FlatStyle = FlatStyle.Flat,
            BackColor = principal ? Verde : Color.White,
            ForeColor = principal ? Color.White : Texto,
            Font = new Font("Segoe UI", 10, principal ? FontStyle.Bold : FontStyle.Regular),
            Cursor = Cursors.Hand,
            Height = 40,
            AutoSize = false,
            UseVisualStyleBackColor = false
        };
        botao.FlatAppearance.BorderSize = principal ? 0 : 1;
        botao.FlatAppearance.BorderColor = Borda;
        botao.FlatAppearance.MouseOverBackColor = principal ? Color.FromArgb(17, 112, 77) : Fundo;
        return botao;
    }

    public static Bitmap Icone(string simbolo, Color cor)
    {
        Bitmap imagem = new(28, 28);
        using Graphics g = Graphics.FromImage(imagem);
        g.Clear(Color.Transparent);
        if (simbolo == "contas")
        {
            using Pen linha = new(cor, 1.5f);
            g.DrawRectangle(linha, 3, 6, 22, 17);
            g.DrawRectangle(linha, 15, 11, 10, 7);
            using SolidBrush ponto = new(cor);
            g.FillEllipse(ponto, 18, 13, 3, 3);
            return imagem;
        }
        using Font fonte = new("Segoe MDL2 Assets", 17);
        using SolidBrush pincel = new(cor);
        g.DrawString(simbolo, fonte, pincel, new RectangleF(0, 0, 28, 28),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        return imagem;
    }
}

internal sealed class RotuloMonetario : Label
{
    public RotuloMonetario()
    {
        AutoSize = false;
        Dock = DockStyle.Fill;
        BackColor = Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        float tamanho = 23;
        Font fonte;
        while (true)
        {
            fonte = new Font("Segoe UI", tamanho, FontStyle.Bold);
            Size medida = TextRenderer.MeasureText(Text, fonte, Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
            if ((medida.Width <= ClientSize.Width - 6 && medida.Height <= ClientSize.Height) || tamanho <= 14) break;
            fonte.Dispose();
            tamanho--;
        }
        using (fonte)
            TextRenderer.DrawText(e.Graphics, Text, fonte, ClientRectangle, ForeColor,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter);
    }
}

// Borda arredondada desenhada pelo próprio Windows Forms, sem biblioteca externa.
internal sealed class PainelCartao : Panel
{
    public PainelCartao()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        Padding = new Padding(18);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? EstiloVisual.Fundo);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using GraphicsPath contorno = new();
        const int diametro = 12;
        Rectangle r = new(0, 0, Width - 1, Height - 1);
        if (r.Width < diametro || r.Height < diametro) return;
        contorno.AddArc(r.Left, r.Top, diametro, diametro, 180, 90);
        contorno.AddArc(r.Right - diametro, r.Top, diametro, diametro, 270, 90);
        contorno.AddArc(r.Right - diametro, r.Bottom - diametro, diametro, diametro, 0, 90);
        contorno.AddArc(r.Left, r.Bottom - diametro, diametro, diametro, 90, 90);
        contorno.CloseFigure();
        using SolidBrush fundo = new(BackColor);
        using Pen borda = new(EstiloVisual.Borda);
        e.Graphics.FillPath(fundo, contorno);
        e.Graphics.DrawPath(borda, contorno);
    }
}
