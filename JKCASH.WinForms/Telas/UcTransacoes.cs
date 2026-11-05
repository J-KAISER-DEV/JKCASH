using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Excecoes;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcTransacoes : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private readonly TransacaoServico? _servico;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private bool _registrando, _opcoesCarregadas;
    private UcTransferencias? _telaTransferencias;
    private UcCompraCartao? _telaCompraCartao;
    private readonly Button _atualizarOpcoes;

    public UcTransacoes()
    {
        InitializeComponent();
        CarregarOpcoes();
        AtualizarCamposAvaliacao();
        _atualizarOpcoes = EstiloVisual.Botao("Atualizar dados");
        _atualizarOpcoes.Name = "btnAtualizarOpcoes";
        _atualizarOpcoes.Size = new Size(180, 36);
        _atualizarOpcoes.Location = new Point(Width - 206, 70);
        _atualizarOpcoes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        Controls.Add(_atualizarOpcoes);
        _atualizarOpcoes.Click += async (_, _) => await AtualizarFormularioAtualAsync();
        cmbTipoTransacao.SelectedIndexChanged += async (_, _) => await AtualizarTipoMovimentacaoAsync();
        AtualizarEstadoRegistro();
    }

    public UcTransacoes(IContaRepositorio contas, ICategoriaRepositorio categorias,
        TransacaoServico servico, TransferenciaServico transferencias,
        ICartaoCreditoRepositorio cartoes, CompraCartaoServico compras, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(servico);
        _contas = contas;
        _categorias = categorias;
        _servico = servico;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        _telaTransferencias = new UcTransferencias(contas, categorias, transferencias, _confirmacao)
        {
            Bounds = pnlCadastroTransacao.Bounds,
            Anchor = pnlCadastroTransacao.Anchor,
            Visible = false
        };
        _telaTransferencias.TransferenciaRealizada += async (_, _) =>
        {
            if (!EmModoTransferencia)
                await AtualizarOpcoesAsync();
        };
        Controls.Add(_telaTransferencias);
        _telaCompraCartao = new UcCompraCartao(cartoes, contas, categorias, compras, _confirmacao)
        {
            Bounds = pnlCadastroTransacao.Bounds,
            Anchor = pnlCadastroTransacao.Anchor,
            Visible = false
        };
        Controls.Add(_telaCompraCartao);
        Load += async (_, _) => await AtualizarOpcoesAsync();
        btnRegistrarTransacao.Click += async (_, _) => await RegistrarTransacaoAsync();
    }

    private async Task AtualizarOpcoesAsync()
    {
        if (_registrando || _contas is null || IsDisposed || Disposing) return;
        _registrando = true;
        AtualizarEstadoRegistro();
        try
        {
            await CarregarContasECategoriasAsync();
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            InvalidarOpcoes();
            MessageBox.Show("Não foi possível carregar as opções. Use Atualizar dados para tentar novamente.",
                "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _registrando = false;
            AtualizarEstadoRegistro();
        }
    }

    private async Task CarregarContasECategoriasAsync()
    {
        if (_contas is null || _categorias is null || IsDisposed) return;
        Guid? contaSelecionada = cmbContaTransacao.SelectedValue as Guid?;
        Guid? categoriaSelecionada = cmbCategoriaTransacao.SelectedValue as Guid?;
        _opcoesCarregadas = false;
        AtualizarEstadoRegistro();
        var dados = await Task.Run(() => (_contas.ListarTodas(), _categorias.ListarTodas()));
        if (IsDisposed) return;
        var contas = dados.Item1.Where(conta => conta.Ativa)
            .Select(conta => new { conta.Id, Nome = $"{conta.Nome} ({conta.Banco})" }).ToList();
        cmbContaTransacao.DisplayMember = "Nome";
        cmbContaTransacao.ValueMember = "Id";
        cmbContaTransacao.DataSource = contas;
        if (contaSelecionada.HasValue && contas.Any(conta => conta.Id == contaSelecionada))
            cmbContaTransacao.SelectedValue = contaSelecionada.Value;
        cmbCategoriaTransacao.DisplayMember = nameof(Categoria.Nome);
        cmbCategoriaTransacao.ValueMember = nameof(Categoria.Id);
        cmbCategoriaTransacao.DataSource = dados.Item2;
        if (categoriaSelecionada.HasValue && dados.Item2.Any(categoria => categoria.Id == categoriaSelecionada))
            cmbCategoriaTransacao.SelectedValue = categoriaSelecionada.Value;
        _opcoesCarregadas = contas.Count > 0 && dados.Item2.Count > 0;
        AtualizarEstadoRegistro();
    }

    private async Task RegistrarTransacaoAsync()
    {
        if (_registrando || !_opcoesCarregadas || _servico is null ||
            _contas is null || IsDisposed || Disposing) return;
        bool salva = false;
        try
        {
            if (cmbContaTransacao.SelectedValue is not Guid contaId ||
                cmbCategoriaTransacao.SelectedValue is not Guid categoriaId)
                throw new ArgumentException("Selecione uma conta e uma categoria.");
            if (cmbTipoTransacao.SelectedItem is not OpcaoMovimentacao { Tipo: TipoTransacao tipo })
                throw new ArgumentException("Selecione o tipo de movimentação.");
            ImportanciaTransacao? importancia = null;
            PlanejamentoTransacao? planejamento = null;
            UrgenciaTransacao? urgencia = null;
            string? justificativa = null;
            if (tipo == TipoTransacao.Saida)
            {
                importancia = cmbImportanciaTransacao.SelectedItem as ImportanciaTransacao?;
                planejamento = cmbPlanejamentoTransacao.SelectedItem as PlanejamentoTransacao?;
                urgencia = cmbUrgenciaTransacao.SelectedItem as UrgenciaTransacao?;
                justificativa = txtJustificativaTransacao.Text;
            }
            decimal valor = numValorTransacao.Value;
            string descricao = txtDescricaoTransacao.Text;
            DateTime data = dtpDataTransacao.Value.Date;
            Transacao previa = new(contaId, tipo, valor, descricao, data, categoriaId, importancia, planejamento, urgencia, justificativa);
            _registrando = true;
            AtualizarEstadoRegistro();
            Conta conta = await Task.Run(() => _contas.ListarTodas().FirstOrDefault(item => item.Id == contaId)
                ?? throw new ConflitoFinanceiroException("A conta não está mais disponível. Atualize os dados."));
            if (IsDisposed || Disposing) return;
            if (!conta.Ativa) throw new ConflitoFinanceiroException("A conta foi desativada.");
            if (tipo == TipoTransacao.Saida) conta.ValidarSaida(valor);
            if (!_confirmacao.Confirmar(this, "Registrar transação", ResumoOperacao.Transacao(previa, cmbContaTransacao.Text, cmbCategoriaTransacao.Text))) return;
            await Task.Run(() => _servico.Registrar(conta, tipo, valor, descricao, data, categoriaId,
                importancia, planejamento, urgencia, justificativa));
            salva = true;
            if (IsDisposed) return;
            numValorTransacao.Value = 0m;
            txtDescricaoTransacao.Clear();
            txtJustificativaTransacao.Clear();
            await CarregarContasECategoriasAsync();
            if (IsDisposed) return;
            MessageBox.Show($"Transação registrada!\nSaldo após o registro: {conta.Saldo.SaldoAtual:C2}",
                "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Information);
            numValorTransacao.Focus();
        }
        catch (ConflitoFinanceiroException erro) when (!salva)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            string mensagem;
            try
            {
                await CarregarContasECategoriasAsync();
                if (IsDisposed) return;
                mensagem = erro.Message + "\nOs dados foram atualizados. Confira o preenchimento antes de tentar novamente.";
            }
            catch (Exception falhaConsulta)
            {
                Debug.WriteLine(falhaConsulta);
                if (IsDisposed || Disposing) return;
                InvalidarOpcoes();
                mensagem = "A operação não foi confirmada e não foi possível atualizar os dados. Use Atualizar dados para consultar novamente.";
            }
            if (!IsDisposed) MessageBox.Show(mensagem, "Dados atualizados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            string mensagem = salva
                ? "A transação foi salva, mas a consulta não pôde ser atualizada. Consulte o extrato antes de fazer outro registro."
                : FalhaTela.EhValidacao(erro) ? erro.Message
                : "Não foi possível confirmar a transação. Consulte o extrato antes de tentar novamente.";
            if (salva || !FalhaTela.EhValidacao(erro)) InvalidarOpcoes();
            MessageBox.Show(mensagem, "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _registrando = false;
            AtualizarEstadoRegistro();
        }
    }

    private void AtualizarEstadoRegistro()
    {
        if (IsDisposed) return;
        bool ocupado = _registrando;
        UseWaitCursor = ocupado;
        tlpDadosTransacao.Enabled = !ocupado;
        txtDescricaoTransacao.Enabled = !ocupado;
        txtJustificativaTransacao.Enabled = !ocupado;
        btnRegistrarTransacao.Enabled = !ocupado && _opcoesCarregadas;
        _atualizarOpcoes.Enabled = !ocupado;
        cmbTipoTransacao.Enabled = !ocupado;
    }

    private void InvalidarOpcoes()
    {
        _opcoesCarregadas = false;
        cmbContaTransacao.DataSource = null;
        cmbCategoriaTransacao.DataSource = null;
    }

    private sealed record OpcaoMovimentacao(string Nome, TipoTransacao? Tipo, bool CompraCartao = false);

    private bool EmModoTransferencia =>
        cmbTipoTransacao.SelectedItem is OpcaoMovimentacao { Tipo: null, CompraCartao: false };

    private bool EmModoCompraCartao =>
        cmbTipoTransacao.SelectedItem is OpcaoMovimentacao { CompraCartao: true };

    private async Task AtualizarFormularioAtualAsync()
    {
        if (EmModoCompraCartao && _telaCompraCartao is not null)
            await _telaCompraCartao.AtualizarDadosAsync();
        else
            await AtualizarOpcoesAsync();
    }

    private async Task AtualizarTipoMovimentacaoAsync()
    {
        AtualizarCamposAvaliacao();
        bool formularioAlternativoAberto =
            _telaTransferencias?.Visible == true || _telaCompraCartao?.Visible == true;
        pnlCadastroTransacao.Visible = !EmModoTransferencia && !EmModoCompraCartao;
        _atualizarOpcoes.Visible = !EmModoTransferencia;

        if (_telaTransferencias is not null) _telaTransferencias.Visible = EmModoTransferencia;
        if (_telaCompraCartao is not null) _telaCompraCartao.Visible = EmModoCompraCartao;

        if (EmModoTransferencia && _telaTransferencias is not null)
            await _telaTransferencias.AtualizarDadosAsync();
        else if (EmModoCompraCartao || formularioAlternativoAberto)
            await AtualizarFormularioAtualAsync();
    }

    private void AtualizarCamposAvaliacao()
    {
        bool saida = cmbTipoTransacao.SelectedItem is OpcaoMovimentacao { Tipo: TipoTransacao.Saida };
        foreach (Control controle in new Control[] { lblImportanciaTransacao, cmbImportanciaTransacao,
            lblPlanejamentoTransacao, cmbPlanejamentoTransacao, lblUrgenciaTransacao, cmbUrgenciaTransacao,
            lblJustificativaTransacao, txtJustificativaTransacao }) controle.Visible = saida;
    }

    private void CarregarOpcoes()
    {
        cmbTipoTransacao.DisplayMember = nameof(OpcaoMovimentacao.Nome);
        cmbTipoTransacao.DataSource = new OpcaoMovimentacao[]
        {
            new("Entrada", TipoTransacao.Entrada),
            new("Saída", TipoTransacao.Saida),
            new("Transferência entre contas", null),
            new("Compra no cartão", null, CompraCartao: true)
        };
        cmbImportanciaTransacao.DataSource = Enum.GetValues<ImportanciaTransacao>();
        cmbPlanejamentoTransacao.DataSource = Enum.GetValues<PlanejamentoTransacao>();
        cmbUrgenciaTransacao.DataSource = Enum.GetValues<UrgenciaTransacao>();
    }
}
