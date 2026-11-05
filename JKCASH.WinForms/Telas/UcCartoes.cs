using JKCASH.WinForms.Visual;
using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;

namespace JKCASH.WinForms.Telas;

public partial class UcCartoes : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private readonly ICartaoCreditoRepositorio? _cartoes;
    private readonly IContaRepositorio? _contas;
    private bool _consultaCartoesValida, _ocupado;

    public UcCartoes()
    {
        InitializeComponent();
        ConfigurarTabelaCartoes();
        DefinirOcupacao(false);
    }

    public UcCartoes(ICartaoCreditoRepositorio cartoes, IContaRepositorio contas, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(cartoes);
        ArgumentNullException.ThrowIfNull(contas);
        _cartoes = cartoes;
        _contas = contas;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        Load += async (_, _) => await CarregarCartoesAsync();
        btnAtualizarCartoes.Click += async (_, _) => await CarregarCartoesAsync();
        btnSalvarCartao.Click += async (_, _) => await SalvarCartaoAsync();
    }

    private void PreencherCadastroCartoes(List<CartaoCredito> cartoes, List<Conta> contas)
    {
        Guid? anterior = cmbContaCartao.SelectedValue as Guid?;
        var ativas = contas.Where(c => c.Ativa)
            .Select(c => new { c.Id, Nome = $"{c.Nome} ({c.Banco})" }).ToList();
        cmbContaCartao.DisplayMember = nameof(Conta.Nome);
        cmbContaCartao.ValueMember = nameof(Conta.Id);
        cmbContaCartao.DataSource = ativas;
        if (anterior.HasValue && ativas.Any(c => c.Id == anterior)) cmbContaCartao.SelectedValue = anterior.Value;
        var nomes = contas.ToDictionary(c => c.Id);
        dgvCartoes.DataSource = cartoes.Select(c => new
        {
            c.Nome,
            Conta = nomes.TryGetValue(c.ContaId, out Conta? conta) ? $"{conta.Nome} ({conta.Banco})" : "Conta não encontrada",
            c.Limite, c.ValorUtilizado, c.LimiteDisponivel, c.DiaFechamento, c.DiaVencimento
        }).ToList();
    }

        private void ConfigurarTabelaCartoes()
        {
            dgvCartoes.AutoGenerateColumns = false;
            dgvCartoes.Columns.Clear();

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.Nome), "Cartão");

            dgvCartoes.Columns.Add(
                "Conta", "Conta vinculada");

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.Limite), "Limite");

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.ValorUtilizado), "Utilizado");

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.LimiteDisponivel), "Disponível");

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.DiaFechamento), "Fechamento");

            dgvCartoes.Columns.Add(
                nameof(CartaoCredito.DiaVencimento), "Vencimento");

            foreach (DataGridViewColumn coluna in dgvCartoes.Columns)
            {
                coluna.DataPropertyName = coluna.Name;
                coluna.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvCartoes.Columns[nameof(CartaoCredito.Limite)]!
                .DefaultCellStyle.Format = "C2";

            dgvCartoes.Columns[nameof(CartaoCredito.ValorUtilizado)]!
                .DefaultCellStyle.Format = "C2";

            dgvCartoes.Columns[nameof(CartaoCredito.LimiteDisponivel)]!
                .DefaultCellStyle.Format = "C2";
        }

    private async Task SalvarCartaoAsync()
    {
        if (_ocupado || !_consultaCartoesValida || _cartoes is null || _contas is null ||
            IsDisposed || Disposing) return;
        bool gravando = false, salvo = false;
        DefinirOcupacao(true);
        txtResultadoCartao.Text = "Cadastrando cartão…";
        try
        {
            if (cmbContaCartao.SelectedValue is not Guid contaId)
                throw new ArgumentException("Selecione a conta vinculada ao cartão.");
            CartaoCredito cartao = new(contaId, txtNomeCartao.Text, numLimiteCartao.Value,
                (int)numDiaFechamentoCartao.Value, (int)numDiaVencimentoCartao.Value,
                chkCompraNaProximaFatura.Checked);
            // Captura todos os controles antes de consultar/gravar fora da thread visual.
            bool ativa = await Task.Run(() => _contas.ListarTodas().Any(c => c.Id == contaId && c.Ativa));
            if (IsDisposed || Disposing) return;
            if (!ativa) throw new ArgumentException("A conta selecionada não existe ou está inativa. Atualize os dados.");
            if (!_confirmacao.Confirmar(this, "Cadastrar cartão", ResumoOperacao.Cartao(cartao, cmbContaCartao.Text)))
            { txtResultadoCartao.Text = "Cadastro cancelado. Preenchimento preservado."; return; }
            gravando = true;
            await Task.Run(() => _cartoes.Adicionar(cartao));
            salvo = true;
            if (IsDisposed || Disposing) return;
            txtNomeCartao.Clear();
            numLimiteCartao.Value = 0m;
            await RecarregarCartoesCoreAsync();
            if (IsDisposed || Disposing) return;
            txtResultadoCartao.Text = "Cartão cadastrado. Limites e opções atualizados.";
            txtNomeCartao.Focus();
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            if (!gravando && erro is ArgumentException)
                txtResultadoCartao.Text = erro.Message;
            else
            {
                InvalidarDadosCartoes();
                txtResultadoCartao.Text = salvo
                    ? "O cartão foi salvo, mas a consulta não pôde ser atualizada. Use Atualizar dados para conferir."
                    : "Não foi possível confirmar o cadastro. Atualize e confira os cartões antes de tentar novamente.";
            }
        }
        finally { DefinirOcupacao(false); }
    }

    private async Task CarregarCartoesAsync()
    {
        if (_ocupado || _cartoes is null || _contas is null || IsDisposed || Disposing) return;
        DefinirOcupacao(true);
        txtResultadoCartao.Text = "Consultando cartões…";
        try
        {
            await RecarregarCartoesCoreAsync();
            if (!IsDisposed && !Disposing) txtResultadoCartao.Clear();
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            InvalidarDadosCartoes();
            txtResultadoCartao.Text = "Não foi possível carregar os cartões. Clique em Atualizar dados para tentar novamente.";
        }
        finally { DefinirOcupacao(false); }
    }

    private async Task RecarregarCartoesCoreAsync()
    {
        var dados = await Task.Run(() => (_cartoes!.ListarTodos(), _contas!.ListarTodas()));
        if (IsDisposed || Disposing) return;
        PreencherCadastroCartoes(dados.Item1, dados.Item2);
        _consultaCartoesValida = true;
    }

    private void InvalidarDadosCartoes()
    {
        _consultaCartoesValida = false;
        cmbContaCartao.DataSource = null;
        dgvCartoes.DataSource = null;
    }

    private void DefinirOcupacao(bool ocupado)
    {
        _ocupado = ocupado;
        if (IsDisposed || Disposing) return;
        UseWaitCursor = ocupado;
        tabCartoes.Enabled = !ocupado;
        btnAtualizarCartoes.Enabled = !ocupado;
        btnSalvarCartao.Enabled = !ocupado && _consultaCartoesValida && cmbContaCartao.Items.Count > 0;
    }

}
