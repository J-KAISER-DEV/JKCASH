using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcContas : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private readonly IContaRepositorio? _repositorio;
    private readonly BarraAtualizacao _consulta;
    private bool _ocupado, _consultaValida;

    public UcContas()
    {
        InitializeComponent();
        cmbTipoConta.DataSource = Enum.GetValues<TipoConta>();
        cmbDestinacaoConta.DataSource = Enum.GetValues<DestinacaoConta>();
        _consulta = new BarraAtualizacao(pnListaContas);
        _consulta.Atualizar.Click += async (_, _) => await AtualizarContasAsync();
        Ocupar(false);
    }

    public UcContas(IContaRepositorio repositorio, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(repositorio);
        _repositorio = repositorio;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        Load += async (_, _) => await AtualizarContasAsync();
        btnSalvarConta.Click += async (_, _) => await SalvarContaAsync();
    }

    private async Task AtualizarContasAsync()
    {
        if (_ocupado || _repositorio is null || IsDisposed || Disposing) return;
        Ocupar(true, "Consultando contas…");
        try { await CarregarCoreAsync(); }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            InvalidarConsulta("Não foi possível consultar as contas. Use Atualizar dados para tentar novamente.");
        }
        finally { Ocupar(false); }
    }

    private async Task CarregarCoreAsync()
    {
        var contas = await Task.Run(() => _repositorio!.ListarTodas().Select(conta => new
        {
            conta.Id, conta.Nome, conta.Banco, conta.Tipo, conta.Destinacao,
            SaldoAtual = conta.Saldo.SaldoAtual,
            LimiteChequeEspecial = conta.Saldo.LimiteChequeEspecial, conta.Ativa
        }).ToList());
        if (IsDisposed || Disposing) return;
        dgvContas.DataSource = contas;
        if (dgvContas.Columns[nameof(Conta.Id)] is DataGridViewColumn colunaId) colunaId.Visible = false;
        _consultaValida = true;
        _consulta.Mostrar(contas.Count == 0 ? "Nenhuma conta cadastrada." : "Saldos atualizados.");
    }

    private async Task SalvarContaAsync()
    {
        if (_ocupado || !_consultaValida || _repositorio is null || IsDisposed || Disposing) return;
        bool gravando = false, salva = false;
        Ocupar(true, "Salvando conta…");
        try
        {
            if (cmbTipoConta.SelectedItem is not TipoConta tipo ||
                cmbDestinacaoConta.SelectedItem is not DestinacaoConta destinacao)
                throw new ArgumentException("Selecione o tipo e a destinação da conta.");
            Conta conta = new(txtNomeConta.Text, txtBancoConta.Text, tipo, destinacao,
                numSaldoInicial.Value, numLimiteChequeEspecial.Value);
            if (!_confirmacao.Confirmar(this, "Cadastrar conta", ResumoOperacao.Conta(conta)))
            { _consulta.Mostrar("Cadastro cancelado. Preenchimento preservado."); return; }
            gravando = true;
            await Task.Run(() => _repositorio.Adicionar(conta));
            salva = true;
            if (IsDisposed || Disposing) return;
            txtNomeConta.Clear();
            txtBancoConta.Clear();
            numSaldoInicial.Value = numLimiteChequeEspecial.Value = 0m;
            await CarregarCoreAsync();
            if (IsDisposed || Disposing) return;
            _consulta.Mostrar("Conta salva. Saldos atualizados.");
            txtNomeConta.Focus();
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            if (!gravando) _consulta.Mostrar(erro.Message, true);
            else InvalidarConsulta(salva
                ? "A conta foi salva, mas a lista não pôde ser atualizada. Use Atualizar dados para conferir."
                : "Não foi possível confirmar a gravação. Atualize e confira a lista antes de cadastrar novamente.");
        }
        finally { Ocupar(false); }
    }

    private void InvalidarConsulta(string mensagem)
    {
        _consultaValida = false;
        dgvContas.DataSource = null;
        _consulta.Mostrar(mensagem, true);
    }

    private void Ocupar(bool ocupado, string? mensagem = null)
    {
        _ocupado = ocupado;
        if (IsDisposed || Disposing) return;
        UseWaitCursor = ocupado;
        tlpDadosConta.Enabled = !ocupado;
        tabContas.Enabled = !ocupado;
        btnSalvarConta.Enabled = !ocupado && _consultaValida;
        _consulta.Atualizar.Enabled = !ocupado;
        if (mensagem is not null) _consulta.Mostrar(mensagem);
    }

    private void tlpDadosConta_Paint(object sender, PaintEventArgs e) { }
}
