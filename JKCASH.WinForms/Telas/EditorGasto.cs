using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

// Mesmo formulário no cadastro e no lançamento. Selecionar um salvo apenas copia os padrões.
public sealed class EditorGasto : UserControl
{
    private readonly ComboBox _selecao = Combo("cmbGastoSalvo");
    private readonly TextBox _nome = new() { Name = "txtNomeGasto", MaxLength = 200, Dock = DockStyle.Fill };
    private readonly ComboBox _categoria = Combo("cmbCategoriaGasto");
    private readonly ComboBox _natureza = Combo("cmbNaturezaGasto");
    private readonly ComboBox _importancia = Combo("cmbImportanciaGasto");
    private readonly ComboBox _necessidade = Combo("cmbNecessidadeGasto");
    private readonly ComboBox _planejamento = Combo("cmbPlanejamentoGasto");
    private readonly ComboBox _frequencia = Combo("cmbFrequenciaGasto");
    private readonly TextBox _motivo = new() { Name = "txtMotivoGasto", MaxLength = 500, Multiline = true, Dock = DockStyle.Fill };
    private readonly Label _rotuloMotivo = new() { AutoSize = true };
    private readonly CheckBox _reutilizar = new() { Name = "chkSalvarGasto", Text = "Salvar este gasto para reutilizar", AutoSize = true, Checked = true };
    private readonly bool _cadastro;
    private Guid? _id;
    public bool Novo => !_id.HasValue;
    public bool Reutilizar => _cadastro || !Novo || _reutilizar.Checked;
    public string CategoriaNome => _categoria.Text;

    public EditorGasto(bool cadastro = false)
    {
        _cadastro = cadastro;
        Name = "editorGasto"; Height = 395; MinimumSize = new Size(580, 395);
        BackColor = Color.White; Font = new Font("Segoe UI", 10);
        var tabela = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(12) };
        tabela.ColumnStyles.Add(new(SizeType.Percent, 50)); tabela.ColumnStyles.Add(new(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) tabela.RowStyles.Add(new(SizeType.Absolute, 51));
        tabela.RowStyles.Add(new(SizeType.Absolute, 65)); tabela.RowStyles.Add(new(SizeType.Absolute, 29));
        Adicionar(tabela, "Gasto salvo ou novo", _selecao, 0, 0);
        var explicacao = new Label { Text = cadastro ? "Cadastre padrões para usar nos próximos lançamentos." : "Ajustes abaixo valem apenas para este lançamento.", Dock = DockStyle.Fill, ForeColor = EstiloVisual.Secundario };
        tabela.Controls.Add(explicacao, 1, 0);
        Adicionar(tabela, "Nome do produto, serviço ou conta", _nome, 0, 1);
        Adicionar(tabela, "Natureza", _natureza, 1, 1);
        Adicionar(tabela, "Categoria", _categoria, 0, 2);
        Adicionar(tabela, "Importância", _importancia, 1, 2);
        Adicionar(tabela, "Necessidade", _necessidade, 0, 3);
        Adicionar(tabela, "Planejamento", _planejamento, 1, 3);
        Adicionar(tabela, "Frequência", _frequencia, 0, 4);
        tabela.Controls.Add(new Label { Text = "Mensal identifica repetição; não gera lançamentos automaticamente.", Dock = DockStyle.Fill, ForeColor = EstiloVisual.Secundario }, 1, 4);
        var motivo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 23, 8, 0) };
        _rotuloMotivo.Location = Point.Empty; motivo.Controls.Add(_motivo); motivo.Controls.Add(_rotuloMotivo);
        tabela.Controls.Add(motivo, 0, 5); tabela.SetColumnSpan(motivo, 2);
        tabela.Controls.Add(_reutilizar, 0, 6); tabela.SetColumnSpan(_reutilizar, 2);
        _reutilizar.Visible = !cadastro;
        Controls.Add(tabela);
        _natureza.DataSource = Enum.GetValues<NaturezaGasto>();
        _importancia.DataSource = Enum.GetValues<ImportanciaTransacao>().Where(i => i != ImportanciaTransacao.Essencial).ToArray();
        _necessidade.DataSource = Enum.GetValues<NecessidadeGasto>();
        _planejamento.DataSource = Enum.GetValues<PlanejamentoTransacao>();
        _frequencia.DataSource = Enum.GetValues<FrequenciaGasto>();
        foreach (var combo in new[] { _natureza, _importancia, _necessidade, _planejamento })
            combo.Format += (_, e) => e.Value = e.ListItem?.ToString() switch
            { "Servico" => "Serviço", "Media" => "Média", "NaoPlanejada" => "Não planejado", "Planejada" => "Planejado", _ => e.Value };
        _selecao.SelectedIndexChanged += (_, _) => Selecionar((_selecao.SelectedItem as Opcao)?.Gasto);
        _importancia.SelectedIndexChanged += (_, _) => AtualizarMotivo();
        _necessidade.SelectedIndexChanged += (_, _) => AtualizarMotivo();
        AtualizarMotivo();
    }

    internal static ComboBox Combo(string nome) => new() { Name = nome, DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, FormattingEnabled = true };
    internal static void Adicionar(TableLayoutPanel tabela, string rotulo, Control campo, int coluna, int linha)
    {
        var painel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 23, 10, 2) };
        painel.Controls.Add(campo);
        painel.Controls.Add(new Label { Text = rotulo, AutoSize = true, Location = Point.Empty });
        tabela.Controls.Add(painel, coluna, linha);
    }
    private sealed record Opcao(string Nome, Gasto? Gasto);
    public void Carregar(List<Categoria> categorias, List<Gasto> gastos)
    {
        // Atualizar consultas preserva o rascunho e a avaliação do usuário.
        Guid? categoria = _categoria.SelectedValue as Guid?;
        var rascunho = (_nome.Text, _natureza.SelectedItem, _importancia.SelectedItem, _necessidade.SelectedItem,
            _planejamento.SelectedItem, _frequencia.SelectedItem, _motivo.Text, _id);
        bool tinhaDados = _selecao.Items.Count > 0;
        _categoria.DisplayMember = nameof(Categoria.Nome); _categoria.ValueMember = nameof(Categoria.Id);
        _categoria.DataSource = categorias;
        _selecao.DisplayMember = nameof(Opcao.Nome);
        _selecao.DataSource = new[] { new Opcao("+ Criar novo gasto", null) }
            .Concat(gastos.Select(g => new Opcao(g.Nome, g))).ToList();
        if (tinhaDados)
        {
            var opcao = _selecao.Items.Cast<Opcao>().FirstOrDefault(o => o.Gasto?.Id == rascunho.Item8);
            if (rascunho.Item8.HasValue && opcao is not null) _selecao.SelectedItem = opcao;
            _id = rascunho.Item8; _nome.Text = rascunho.Item1;
            _natureza.SelectedItem = rascunho.Item2; _importancia.SelectedItem = rascunho.Item3;
            _necessidade.SelectedItem = rascunho.Item4; _planejamento.SelectedItem = rascunho.Item5;
            _frequencia.SelectedItem = rascunho.Item6; _motivo.Text = rascunho.Item7;
            if (categoria.HasValue && categorias.Any(c => c.Id == categoria)) _categoria.SelectedValue = categoria.Value;
        }
    }
    public void Selecionar(Gasto? gasto)
    {
        _id = gasto?.Id; _reutilizar.Visible = !_cadastro && Novo;
        _nome.Text = gasto?.Nome ?? "";
        if (gasto is not null) _categoria.SelectedValue = gasto.CategoriaId;
        _natureza.SelectedItem = gasto?.Natureza ?? NaturezaGasto.Produto;
        _importancia.SelectedItem = gasto?.Importancia ?? ImportanciaTransacao.Media;
        _necessidade.SelectedItem = gasto?.Necessidade ?? NecessidadeGasto.Media;
        _planejamento.SelectedItem = gasto?.Planejamento ?? PlanejamentoTransacao.Planejada;
        _frequencia.SelectedItem = gasto?.Frequencia ?? FrequenciaGasto.Eventual;
        _motivo.Text = gasto?.Motivo ?? "";
        AtualizarMotivo();
    }
    public void Limpar() { _selecao.SelectedIndex = 0; Selecionar(null); }
    private void AtualizarMotivo() => _rotuloMotivo.Text =
        _importancia.SelectedItem is ImportanciaTransacao.Baixa || _necessidade.SelectedItem is NecessidadeGasto.Baixa
            ? "Motivo do gasto (obrigatório)" : "Motivo do gasto (opcional)";
    public Gasto Ler()
    {
        if (_categoria.SelectedValue is not Guid categoria) throw new ArgumentException("Selecione uma categoria. Cadastre uma na área Gastos, se necessário.");
        return new Gasto(_nome.Text, categoria, (NaturezaGasto)_natureza.SelectedItem!,
            (ImportanciaTransacao)_importancia.SelectedItem!, (NecessidadeGasto)_necessidade.SelectedItem!,
            (PlanejamentoTransacao)_planejamento.SelectedItem!, (FrequenciaGasto)_frequencia.SelectedItem!, _motivo.Text, _id);
    }
}
