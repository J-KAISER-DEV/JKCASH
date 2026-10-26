using System.Diagnostics;
using JKCASH.Application.Excecoes;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;

namespace JKCASH.WinForms.Telas;

public partial class UcTransacoes : UserControl
{
    private readonly TransacaoServico? _servico;
    private readonly IContaRepositorio? _contas;
    private readonly ICategoriaRepositorio? _categorias;
    private readonly ITransacaoRepositorio? _transacoes;
    private bool _registrando, _opcoesCarregadas;

    public UcTransacoes()
    {
        InitializeComponent();
        CarregarOpcoes();
        ConfigurarExtrato();
    }

    public UcTransacoes(IContaRepositorio contas, ICategoriaRepositorio categorias,
        TransacaoServico servico, ITransacaoRepositorio transacoes) : this()
    {
        ArgumentNullException.ThrowIfNull(contas);
        ArgumentNullException.ThrowIfNull(categorias);
        ArgumentNullException.ThrowIfNull(servico);
        ArgumentNullException.ThrowIfNull(transacoes);
        _contas = contas;
        _categorias = categorias;
        _servico = servico;
        _transacoes = transacoes;
        Load += async (_, _) =>
        {
            try
            {
                await CarregarContasECategoriasAsync();
                if (!IsDisposed) await CarregarContasDoExtratoAsync();
            }
            catch (Exception erro)
            {
                Debug.WriteLine(erro);
                if (!IsDisposed) MessageBox.Show("Não foi possível carregar as opções. Reabra a tela para atualizar os dados.",
                    "JKCASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        btnRegistrarTransacao.Click += async (_, _) => await RegistrarTransacaoAsync();
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
        if (_registrando || !_opcoesCarregadas || _servico is null || _contas is null) return;
        bool salva = false;
        try
        {
            if (cmbContaTransacao.SelectedValue is not Guid contaId ||
                cmbCategoriaTransacao.SelectedValue is not Guid categoriaId)
                throw new ArgumentException("Selecione uma conta e uma categoria.");
            if (cmbTipoTransacao.SelectedItem is not TipoTransacao tipo ||
                cmbImportanciaTransacao.SelectedItem is not ImportanciaTransacao importancia ||
                cmbPlanejamentoTransacao.SelectedItem is not PlanejamentoTransacao planejamento ||
                cmbUrgenciaTransacao.SelectedItem is not UrgenciaTransacao urgencia)
                throw new ArgumentException("Preencha todas as opções da movimentação.");

            // Captura os controles na thread visual antes de acessar o banco em segundo plano.
            decimal valor = numValorTransacao.Value;
            string descricao = txtDescricaoTransacao.Text, justificativa = txtJustificativaTransacao.Text;
            DateTime data = dtpDataTransacao.Value.Date;
            _registrando = true;
            AtualizarEstadoRegistro();
            Conta conta = await Task.Run(() =>
            {
                Conta atual = _contas.ListarTodas().FirstOrDefault(item => item.Id == contaId)
                    ?? throw new ConflitoFinanceiroException("A conta não está mais disponível. Atualize os dados.");
                _servico.Registrar(atual, tipo, valor, descricao, data, categoriaId,
                    importancia, planejamento, urgencia, justificativa);
                return atual;
            });
            salva = true;
            if (IsDisposed) return;
            numValorTransacao.Value = 0m;
            txtDescricaoTransacao.Clear();
            txtJustificativaTransacao.Clear();
            await CarregarContasECategoriasAsync();
            if (IsDisposed) return;
            await CarregarContasDoExtratoAsync();
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
                await CarregarContasDoExtratoAsync();
                mensagem = erro.Message + "\nOs dados foram atualizados. Confira o preenchimento antes de tentar novamente.";
            }
            catch (Exception falhaConsulta)
            {
                Debug.WriteLine(falhaConsulta);
                _opcoesCarregadas = false;
                mensagem = "A operação não foi confirmada e não foi possível atualizar os dados. Reabra a tela para consultar novamente.";
            }
            if (!IsDisposed) MessageBox.Show(mensagem, "Dados atualizados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed) return;
            string mensagem = salva
                ? "A transação foi salva, mas a consulta não pôde ser atualizada. Consulte o extrato antes de fazer outro registro."
                : erro is ArgumentException or InvalidOperationException ? erro.Message
                : "Não foi possível confirmar a transação. Consulte o extrato antes de tentar novamente.";
            if (!salva && erro is not (ArgumentException or InvalidOperationException)) _opcoesCarregadas = false;
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
        tlpDadosTransacao.Enabled = !_registrando;
        txtDescricaoTransacao.Enabled = !_registrando;
        txtJustificativaTransacao.Enabled = !_registrando;
        btnRegistrarTransacao.Enabled = !_registrando && _opcoesCarregadas;
    }

    private void CarregarOpcoes()
    {
        cmbTipoTransacao.DataSource = Enum.GetValues<TipoTransacao>();
        cmbImportanciaTransacao.DataSource = Enum.GetValues<ImportanciaTransacao>();
        cmbPlanejamentoTransacao.DataSource = Enum.GetValues<PlanejamentoTransacao>();
        cmbUrgenciaTransacao.DataSource = Enum.GetValues<UrgenciaTransacao>();
    }
}
