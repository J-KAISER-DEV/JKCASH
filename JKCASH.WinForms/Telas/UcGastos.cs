using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public sealed class UcGastos : UserControl
{
    private readonly IGastoRepositorio _gastos;
    private readonly ICategoriaRepositorio _categorias;
    private readonly EditorGasto _editor = new(true) { Dock = DockStyle.Top };
    private readonly DataGridView _lista = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
        RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White };
    private readonly Label _status = new() { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12) };
    private readonly Button _salvar = EstiloVisual.Botao("Salvar gasto");
    private readonly Button _atualizar = EstiloVisual.Botao("Atualizar dados");
    private readonly TabControl _abas = new() { Dock = DockStyle.Fill };
    private bool _ocupado, _carregado;

    public UcGastos(IGastoRepositorio gastos, ICategoriaRepositorio categorias)
    {
        _gastos = gastos; _categorias = categorias;
        Name = "ucGastos"; BackColor = EstiloVisual.Fundo; Padding = new Padding(24); Font = new Font("Segoe UI", 10);
        var titulo = new Label { Text = "Gastos", Font = new Font("Segoe UI", 26, FontStyle.Bold), Dock = DockStyle.Top, Height = 65 };
        var cadastro = new TabPage("Cadastrar ou editar gasto") { AutoScroll = true };
        var botoes = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, Padding = new Padding(12, 5, 0, 5) };
        _salvar.Size = new Size(170, 38); _atualizar.Size = new Size(170, 38);
        botoes.Controls.AddRange([_salvar, _atualizar]);
        cadastro.Controls.Add(botoes); cadastro.Controls.Add(_editor);
        var lista = new TabPage("Gastos salvos"); lista.Controls.Add(_lista);
        var categoriasAuxiliares = new TabPage("Categorias"); categoriasAuxiliares.Controls.Add(new UcCategorias(categorias) { Dock = DockStyle.Fill });
        _abas.TabPages.AddRange([cadastro, lista, categoriasAuxiliares]);
        Controls.Add(_abas); Controls.Add(_status); Controls.Add(titulo);
        Load += async (_, _) => await AtualizarAsync();
        _atualizar.Click += async (_, _) => await AtualizarAsync();
        _salvar.Click += async (_, _) => await SalvarAsync();
        _abas.SelectedIndexChanged += async (_, _) => { if (_abas.SelectedIndex < 2) await AtualizarAsync(); };
        Ocupar(false);
    }
    private void Ocupar(bool ocupado)
    {
        _ocupado = ocupado;
        if (IsDisposed) return;
        UseWaitCursor = ocupado; _editor.Enabled = !ocupado; _atualizar.Enabled = !ocupado;
        _salvar.Enabled = !ocupado && _carregado; _abas.Enabled = !ocupado;
    }
    private async Task CarregarAsync()
    {
        var dados = await Task.Run(() => (_gastos.ListarTodos(), _categorias.ListarTodas()));
        if (IsDisposed) return;
        _editor.Carregar(dados.Item2, dados.Item1);
        var categorias = dados.Item2.ToDictionary(c => c.Id, c => c.Nome);
        _lista.DataSource = dados.Item1.Select(g => new { g.Nome, g.Natureza,
            Categoria = categorias.GetValueOrDefault(g.CategoriaId, "Categoria não encontrada"),
            Importância = g.Importancia, g.Necessidade, g.Planejamento, Frequência = g.Frequencia, g.Motivo }).ToList();
        _carregado = true;
        _status.Text = dados.Item2.Count == 0 ? "Primeiro cadastre uma categoria na aba Categorias." : $"{dados.Item1.Count} gasto(s) salvo(s). Selecione um no formulário para editar.";
    }
    private async Task AtualizarAsync()
    {
        if (_ocupado || IsDisposed) return;
        Ocupar(true);
        try { await CarregarAsync(); }
        catch (Exception erro)
        {
            System.Diagnostics.Debug.WriteLine(erro); _carregado = false;
            if (!IsDisposed) _status.Text = "Não foi possível carregar os dados. Use Atualizar dados.";
        }
        finally { Ocupar(false); }
    }
    private async Task SalvarAsync()
    {
        if (_ocupado || !_carregado || IsDisposed) return;
        bool salvo = false;
        try
        {
            var gasto = _editor.Ler(); Ocupar(true);
            await Task.Run(() => _gastos.Salvar(gasto)); salvo = true;
            if (IsDisposed) return;
            _editor.Limpar(); await CarregarAsync();
            if (!IsDisposed) _status.Text = "Gasto salvo. As alterações serão usadas nos próximos lançamentos; o histórico foi preservado.";
        }
        catch (Exception erro)
        {
            System.Diagnostics.Debug.WriteLine(erro);
            if (IsDisposed) return;
            if (salvo || !FalhaTela.EhValidacao(erro)) _carregado = false;
            _status.Text = salvo ? "Gasto salvo, mas a lista não pôde ser atualizada. Use Atualizar dados."
                : FalhaTela.EhValidacao(erro) ? erro.Message : "Não foi possível confirmar o cadastro. Atualize e confira antes de tentar novamente.";
        }
        finally { Ocupar(false); }
    }
}
