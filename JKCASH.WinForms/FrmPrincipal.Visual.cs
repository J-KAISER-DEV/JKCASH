using JKCASH.Application.Servicos;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.WinForms.Telas;
using JKCASH.WinForms.Visual;
namespace JKCASH.WinForms;

public partial class FrmPrincipal
{
    private readonly List<(Button Botao, string Tela)> _botoesMenu = new();
    private readonly List<Image> _iconesMenu = new();

    // A estrutura básica continua no Designer; o estilo compartilhado fica aqui.
    private void ConfigurarVisual()
    {
        DoubleBuffered = true;
        Disposed += (_, _) =>
        {
            foreach (Image imagem in _iconesMenu) imagem.Dispose();
        };
        pnlMenuPrincipal.BackColor = EstiloVisual.Menu;
        pnlConteudo.BackColor = EstiloVisual.Fundo;

        Panel identidade = new() { Dock = DockStyle.Top, Height = 135 };
        Label logo = EstiloVisual.Rotulo("JKCASH", 23, true);
        logo.Dock = DockStyle.None;
        logo.ForeColor = Color.White;
        logo.Location = new Point(61, 28);
        logo.Size = new Size(155, 44);
        Label subtitulo = EstiloVisual.Rotulo("Finanças pessoais", 10);
        subtitulo.Dock = DockStyle.None;
        subtitulo.ForeColor = Color.FromArgb(180, 195, 210);
        subtitulo.Location = new Point(62, 76);
        subtitulo.Size = new Size(155, 24);
        Panel marca = new() { Location = new Point(22, 39), Size = new Size(31, 36) };
        marca.Paint += (_, e) =>
        {
            using SolidBrush pincel = new(EstiloVisual.Verde);
            e.Graphics.FillRectangle(pincel, 0, 22, 8, 14);
            e.Graphics.FillRectangle(pincel, 11, 12, 8, 24);
            e.Graphics.FillRectangle(pincel, 22, 0, 8, 36);
        };
        identidade.Controls.AddRange([logo, subtitulo, marca]);

        FlowLayoutPanel navegacao = new()
        {
            Name = "pnlNavegacao", Dock = DockStyle.Top, Height = 384,
            FlowDirection = FlowDirection.TopDown, WrapContents = false,
            Padding = new Padding(9, 0, 9, 0)
        };
        var opcoes = new[]
        {
            ("Dashboard", "\uE80F"), ("Contas", "contas"),
            ("Transações", "\uE8AB"), ("Cartões", "\uE8C7"),
            ("Faturas", "\uE8A5"), ("Categorias", "\uE8EC")
        };
        foreach (var (tela, simbolo) in opcoes)
        {
            Button botao = EstiloVisual.Botao(tela);
            botao.Name = "btn" + tela.Replace("ç", "c").Replace("õ", "o").Replace("ã", "a");
            botao.Size = new Size(202, 53);
            botao.Margin = new Padding(0, 0, 0, 9);
            botao.Padding = new Padding(13, 0, 0, 0);
            botao.BackColor = EstiloVisual.Menu;
            botao.ForeColor = Color.FromArgb(221, 232, 242);
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = EstiloVisual.MenuSelecionado;
            botao.FlatAppearance.MouseDownBackColor = EstiloVisual.MenuSelecionado;
            botao.TextAlign = ContentAlignment.MiddleLeft;
            botao.ImageAlign = ContentAlignment.MiddleLeft;
            botao.TextImageRelation = TextImageRelation.ImageBeforeText;
            Image icone = EstiloVisual.Icone(simbolo, Color.FromArgb(164, 201, 222));
            _iconesMenu.Add(icone);
            botao.Image = icone;
            botao.Text = "   " + tela;
            botao.Click += (_, _) => AbrirTela(tela);
            botao.Paint += (_, e) =>
            {
                if (botao.BackColor == EstiloVisual.MenuSelecionado)
                {
                    using SolidBrush verde = new(EstiloVisual.Verde);
                    e.Graphics.FillRectangle(verde, 0, 8, 4, botao.Height - 16);
                }
            };
            _botoesMenu.Add((botao, tela));
            navegacao.Controls.Add(botao);
        }

        Label rodape = EstiloVisual.Rotulo("Seu dinheiro, organizado.", 9);
        rodape.ForeColor = Color.FromArgb(164, 183, 201);
        rodape.Dock = DockStyle.Bottom;
        rodape.Height = 61;
        rodape.Padding = new Padding(22, 0, 0, 0);
        pnlMenuPrincipal.Controls.Add(navegacao);
        pnlMenuPrincipal.Controls.Add(identidade);
        pnlMenuPrincipal.Controls.Add(rodape);

        // Respeita o espaço útil do monitor antes de centralizar a janela.
        Rectangle area = Screen.FromControl(this).WorkingArea;
        Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
        AbrirTela("Dashboard");
    }

    private void AbrirTela(string tela)
    {
        foreach (var (botao, nome) in _botoesMenu)
        {
            botao.BackColor = nome == tela ? EstiloVisual.MenuSelecionado : EstiloVisual.Menu;
            botao.Invalidate();
        }

        while (pnlConteudo.Controls.Count > 0)
        {
            Control anterior = pnlConteudo.Controls[0];
            pnlConteudo.Controls.Remove(anterior);
            anterior.Dispose();
        }

        if (tela == "Transações")
        {
            CategoriaRepositorioSql categorias = new();
            TransacaoRepositorioSql repositorioTransacoes = new();
            TransacaoServico servico = new(
       repositorioTransacoes,
       categorias);

            UcTransacoes transacoes = new(
                new ContaRepositorioSql(),
                categorias,
                servico,
                repositorioTransacoes);

            transacoes.Dock = DockStyle.Fill;

            pnlConteudo.Controls.Add(transacoes);
            return;
        }

        if (tela == "Contas")
        {
            ContaRepositorioSql repositorioContas = new();
            CategoriaRepositorioSql categorias = new();
            TransferenciaServico transferencias = new(repositorioContas, categorias, new TransferenciaRepositorioSql());
            UcContas contas = new(repositorioContas, categorias, transferencias);
            contas.Dock = DockStyle.Fill;
            pnlConteudo.Controls.Add(contas);
            return;
        }

        if (tela == "Dashboard")
        {
            UcDashboard dashboard = new(new ContaRepositorioSql(), new CategoriaRepositorioSql(),
                new TransacaoRepositorioSql(), new CartaoCreditoRepositorioSql(), new CompraCartaoRepositorioSql());
            dashboard.AbrirTransacoes += (_, _) => AbrirTela("Transações");
            dashboard.AbrirFaturas += (_, _) => AbrirTela("Faturas");
            pnlConteudo.Controls.Add(dashboard);
            return;

           
        }
        if (tela == "Categorias")
        {
            UcCategorias categorias = new(new CategoriaRepositorioSql());

            categorias.Dock = DockStyle.Fill;

            pnlConteudo.Controls.Add(categorias);

            return;
        }
        if (tela == "Cartões")
        {
            CartaoCreditoRepositorioSql repositorioCartoes = new();
            CategoriaRepositorioSql categorias = new();
            CompraCartaoRepositorioSql compras = new();
            CompraCartaoServico servico = new(repositorioCartoes, categorias, compras);
            UcCartoes cartoes = new(repositorioCartoes, new ContaRepositorioSql(), categorias, compras, servico);

            cartoes.Dock = DockStyle.Fill;

            pnlConteudo.Controls.Add(cartoes);
            return;
        }

        if (tela == "Faturas")
        {
            CartaoCreditoRepositorioSql cartoes = new();
            ContaRepositorioSql contas = new();
            CategoriaRepositorioSql categorias = new();
            CompraCartaoRepositorioSql comprasEFaturas = new();
            PagamentoFaturaServico pagamentos = new(comprasEFaturas, cartoes, contas, categorias);
            UcFaturas faturas = new(cartoes, comprasEFaturas, comprasEFaturas, contas, categorias, pagamentos)
            { Dock = DockStyle.Fill };
            pnlConteudo.Controls.Add(faturas);
            return;
        }

        Panel pagina = new() { Dock = DockStyle.Fill, Padding = new Padding(30), BackColor = EstiloVisual.Fundo };
        Label titulo = EstiloVisual.Rotulo(tela, 28, true);
        titulo.Dock = DockStyle.Top;
        titulo.Height = 66;
        Label descricao = EstiloVisual.Rotulo("Tela em construção. Estamos definindo o visual antes de ligar as funcionalidades.", 11);
        descricao.Dock = DockStyle.Top;
        descricao.Height = 65;
        descricao.ForeColor = EstiloVisual.Secundario;
        Button voltar = EstiloVisual.Botao("Voltar ao dashboard", true);
        voltar.Dock = DockStyle.Top;
        voltar.Height = 42;
        voltar.Click += (_, _) => AbrirTela("Dashboard");
        pagina.Controls.Add(voltar);
        pagina.Controls.Add(descricao);
        pagina.Controls.Add(titulo);
        pnlConteudo.Controls.Add(pagina);
    }
}
