using JKCASH.Application.Interfaces;
using JKCASH.Application.Modelos;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public sealed class UcRegistroGasto : UserControl
{
    private readonly IGastoRepositorio _gastos;
    private readonly ICategoriaRepositorio _categorias;
    private readonly IContaRepositorio _contas;
    private readonly ICartaoCreditoRepositorio _cartoes;
    private readonly TransacaoServico _transacoes;
    private readonly CompraCartaoServico _compras;
    private readonly IConfirmacaoOperacao _confirmacao;
    private readonly EditorGasto _editor = new();
    private readonly ComboBox _pagamento = EditorGasto.Combo("cmbFormaPagamentoGasto");
    private readonly ComboBox _origem = EditorGasto.Combo("cmbOrigemPagamentoGasto");
    private readonly ComboBox _modalidade = EditorGasto.Combo("cmbModalidadeCreditoGasto");
    private readonly NumericUpDown _parcelas = new() { Name = "numParcelasGasto", Minimum = 2, Maximum = 120, Value = 2, Dock = DockStyle.Fill };
    private readonly NumericUpDown _valor = new() { Name = "numValorGasto", DecimalPlaces = 2, Maximum = 999999999, ThousandsSeparator = true, Dock = DockStyle.Fill };
    private readonly DateTimePicker _data = new() { Name = "dtpDataGasto", Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, MaxDate = DateTime.Today };
    private readonly Label _situacao = new() { AutoSize = true, MaximumSize = new Size(850, 0), Dock = DockStyle.Top, Padding = new Padding(12) };
    private readonly Label _saldo = new() { Dock = DockStyle.Fill, ForeColor = EstiloVisual.Secundario };
    private readonly Button _registrar = EstiloVisual.Botao("Confirmar gasto");
    private readonly Panel _conteudo = new() { Dock = DockStyle.Top, Height = 650 };
    private List<Conta> _listaContas = [];
    private List<CartaoCredito> _listaCartoes = [];
    private bool _ocupado, _carregado;
    private bool Credito => _pagamento.SelectedIndex == 1;
    private sealed record Origem(Guid Id, string Nome);

    public UcRegistroGasto(IGastoRepositorio gastos, ICategoriaRepositorio categorias, IContaRepositorio contas,
        ICartaoCreditoRepositorio cartoes, TransacaoServico transacoes, CompraCartaoServico compras,
        IConfirmacaoOperacao confirmacao)
    {
        _gastos = gastos; _categorias = categorias; _contas = contas; _cartoes = cartoes;
        _transacoes = transacoes; _compras = compras; _confirmacao = confirmacao;
        Name = "registroGasto"; AutoScroll = true; BackColor = Color.White; Font = new Font("Segoe UI", 10);
        _editor.Dock = DockStyle.Top;
        var pagamento = new TableLayoutPanel { Dock = DockStyle.Top, Height = 170, Padding = new Padding(12), ColumnCount = 2, RowCount = 3 };
        pagamento.ColumnStyles.Add(new(SizeType.Percent, 50)); pagamento.ColumnStyles.Add(new(SizeType.Percent, 50));
        for (int i = 0; i < 3; i++) pagamento.RowStyles.Add(new(SizeType.Percent, 33.33f));
        EditorGasto.Adicionar(pagamento, "Valor total", _valor, 0, 0);
        EditorGasto.Adicionar(pagamento, "Data", _data, 1, 0);
        EditorGasto.Adicionar(pagamento, "Forma de pagamento", _pagamento, 0, 1);
        EditorGasto.Adicionar(pagamento, "Conta ou cartão", _origem, 1, 1);
        EditorGasto.Adicionar(pagamento, "Crédito: à vista ou parcelado", _modalidade, 0, 2);
        EditorGasto.Adicionar(pagamento, "Quantidade de parcelas", _parcelas, 1, 2);
        var rodape = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(12) };
        _registrar.Dock = DockStyle.Right; _registrar.Width = 190;
        rodape.Controls.Add(_saldo); rodape.Controls.Add(_registrar);
        _conteudo.Controls.Add(rodape); _conteudo.Controls.Add(pagamento); _conteudo.Controls.Add(_editor);
        Controls.Add(_situacao); Controls.Add(_conteudo);
        _pagamento.DataSource = new[] { "Débito em conta", "Crédito no cartão" };
        _modalidade.DataSource = new[] { "À vista (1 parcela na fatura)", "Parcelado" };
        _pagamento.SelectedIndexChanged += (_, _) => AtualizarOrigens();
        _origem.SelectedIndexChanged += (_, _) => AtualizarSaldo();
        _modalidade.SelectedIndexChanged += (_, _) => AtualizarModalidade();
        _registrar.Click += async (_, _) => await RegistrarAsync();
        Ocupar(false);
    }

    public async Task AtualizarDadosAsync()
    {
        if (_ocupado || IsDisposed) return;
        Ocupar(true);
        try { await CarregarAsync(); }
        catch (Exception erro)
        {
            System.Diagnostics.Debug.WriteLine(erro); _carregado = false;
            if (!IsDisposed) _situacao.Text = "Não foi possível consultar os dados. Use Atualizar dados para tentar novamente.";
        }
        finally { Ocupar(false); }
    }
    private async Task CarregarAsync()
    {
        var dados = await Task.Run(() => (_gastos.ListarTodos(), _categorias.ListarTodas(), _contas.ListarTodas(), _cartoes.ListarTodos()));
        if (IsDisposed) return;
        _editor.Carregar(dados.Item2, dados.Item1);
        _listaContas = dados.Item3; _listaCartoes = dados.Item4;
        AtualizarOrigens(); _carregado = true;
        _situacao.Text = dados.Item2.Count == 0 ? "Cadastre uma categoria na área Gastos para começar." : "Escolha um gasto salvo ou crie um novo. O formulário será salvo somente ao confirmar.";
    }
    private void AtualizarOrigens()
    {
        Guid? anterior = _origem.SelectedValue as Guid?;
        var contas = _listaContas.ToDictionary(c => c.Id);
        var opcoes = Credito
            ? _listaCartoes.Where(c => contas.TryGetValue(c.ContaId, out var conta) && conta.Ativa)
                .Select(c => new Origem(c.Id, $"{c.Nome} — {contas[c.ContaId].Nome}")).ToList()
            : _listaContas.Where(c => c.Ativa).Select(c => new Origem(c.Id, $"{c.Nome} ({c.Banco})")).ToList();
        _origem.DisplayMember = nameof(Origem.Nome); _origem.ValueMember = nameof(Origem.Id); _origem.DataSource = opcoes;
        if (anterior.HasValue && opcoes.Any(o => o.Id == anterior)) _origem.SelectedValue = anterior.Value;
        AtualizarModalidade(); AtualizarSaldo();
    }
    private void AtualizarModalidade()
    {
        _modalidade.Enabled = Credito; _parcelas.Enabled = Credito && _modalidade.SelectedIndex == 1;
    }
    private void AtualizarSaldo()
    {
        if (_origem.SelectedValue is not Guid id) { _saldo.Text = "Selecione uma conta ou cartão ativo."; return; }
        _saldo.Text = Credito
            ? $"Limite disponível: {_listaCartoes.First(c => c.Id == id).LimiteDisponivel:C2}\nA compra será cobrada na fatura."
            : $"Disponível com cheque especial: {_listaContas.First(c => c.Id == id).Saldo.DisponivelComChequeEspecial:C2}";
    }
    private void Ocupar(bool valor)
    {
        _ocupado = valor;
        if (IsDisposed) return;
        _conteudo.Enabled = !valor; UseWaitCursor = valor; _registrar.Enabled = !valor && _carregado;
    }
    private async Task RegistrarAsync()
    {
        if (_ocupado || !_carregado || IsDisposed) return;
        bool salvo = false;
        try
        {
            Gasto gasto = _editor.Ler();
            if (_origem.SelectedValue is not Guid origem) throw new ArgumentException("Selecione a conta ou o cartão para pagar.");
            decimal valor = _valor.Value; DateTime data = _data.Value.Date;
            bool credito = Credito, reutilizar = _editor.Reutilizar, novo = _editor.Novo;
            int parcelas = credito && _modalidade.SelectedIndex == 1 ? (int)_parcelas.Value : 1;
            string resumo;
            Ocupar(true);
            if (credito)
            {
                var cartao = await Task.Run(() => _cartoes.BuscarPorId(origem) ?? throw new ArgumentException("Cartão não disponível. Atualize os dados."));
                if (IsDisposed) return;
                var previa = new CompraCartao(cartao, valor, gasto.Nome, data, gasto.CategoriaId,
                    gasto.Importancia, gasto.Planejamento, UrgenciaTransacao.SemUrgencia, gasto.Motivo, parcelas, gasto, reutilizar, novo);
                if (valor > cartao.LimiteDisponivel) throw new ArgumentException("O valor ultrapassa o limite disponível.");
                resumo = ResumoOperacao.Compra(previa, _origem.Text, _editor.CategoriaNome);
            }
            else
            {
                var conta = await Task.Run(() => _contas.ListarTodas().Single(c => c.Id == origem));
                if (IsDisposed) return;
                conta.ValidarSaida(valor);
                var previa = new Transacao(origem, TipoTransacao.Saida, valor, gasto.Nome, data, gasto.CategoriaId,
                    gasto: gasto, salvarGasto: reutilizar, novoGasto: novo);
                resumo = ResumoOperacao.Transacao(previa, _origem.Text, _editor.CategoriaNome);
            }
            resumo += $"\nNatureza: {gasto.Natureza}\nNecessidade: {gasto.Necessidade}\nFrequência: {gasto.Frequencia}"
                + (novo ? $"\nSalvar no cadastro: {(reutilizar ? "Sim" : "Não, somente neste lançamento")}" : "\nUsando cadastro salvo; ajustes ficam neste lançamento.");
            if (!_confirmacao.Confirmar(this, "Confirmar gasto", resumo)) { _situacao.Text = "Registro cancelado. Preenchimento preservado."; return; }
            await Task.Run(() =>
            {
                if (credito) _compras.Registrar(origem, valor, gasto.Nome, data, gasto.CategoriaId,
                    gasto.Importancia, gasto.Planejamento, UrgenciaTransacao.SemUrgencia, gasto.Motivo, parcelas, gasto, reutilizar, novo);
                else
                {
                    var conta = _contas.ListarTodas().Single(c => c.Id == origem);
                    _transacoes.Registrar(conta, TipoTransacao.Saida, valor, gasto.Nome, data, gasto.CategoriaId,
                        gasto: gasto, salvarGasto: reutilizar, novoGasto: novo);
                }
            });
            salvo = true;
            if (IsDisposed) return;
            _editor.Limpar(); _valor.Value = 0; _data.Value = DateTime.Today;
            _modalidade.SelectedIndex = 0;
            await CarregarAsync();
            if (!IsDisposed) _situacao.Text = "Gasto registrado. Consulte o histórico em Extrato e, para crédito, as parcelas em Faturas.";
        }
        catch (Exception erro)
        {
            System.Diagnostics.Debug.WriteLine(erro);
            if (IsDisposed) return;
            if (salvo || !FalhaTela.EhValidacao(erro)) _carregado = false;
            _situacao.Text = salvo ? "O gasto foi salvo, mas a consulta falhou. Atualize e confira o extrato antes de registrar novamente."
                : FalhaTela.EhValidacao(erro) ? erro.Message : "Não foi possível confirmar a operação. Atualize os dados e confira o extrato antes de tentar novamente.";
        }
        finally { Ocupar(false); }
    }
}
