using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JKCASH.Domain.Enums;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Domain.Entidades;
namespace JKCASH.WinForms.Telas
{
    public partial class UcTransacoes : UserControl
    {
        private readonly TransacaoServico? _servico;
        private readonly IContaRepositorio? _contas;
        private readonly ICategoriaRepositorio? _categorias;
        private readonly ITransacaoRepositorio? _transacoes;
        public UcTransacoes()
        {
            InitializeComponent();
            CarregarOpcoes();
            ConfigurarExtrato();

        }
        public UcTransacoes(
            IContaRepositorio contas,
            ICategoriaRepositorio categorias,
            TransacaoServico servico,
            ITransacaoRepositorio transacoes)
            : this()
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
                CarregarContasECategorias();
                await CarregarContasDoExtratoAsync();
            };
            btnRegistrarTransacao.Click += (_, _) => RegistrarTransacao();
        }

        private void CarregarContasECategorias()
        {
            if (_contas is null || _categorias is null)
                return;

            btnRegistrarTransacao.Enabled = false;

            try
            {
                var contas = _contas.ListarTodas()
                    .Where(conta => conta.Ativa)
                    .Select(conta => new
                    {
                        conta.Id,
                        Nome = $"{conta.Nome} ({conta.Banco})"
                    })
                    .ToList();

                var categorias = _categorias.ListarTodas();

                cmbContaTransacao.DisplayMember = "Nome";
                cmbContaTransacao.ValueMember = "Id";
                cmbContaTransacao.DataSource = contas;

                cmbCategoriaTransacao.DisplayMember = "Nome";
                cmbCategoriaTransacao.ValueMember = "Id";
                cmbCategoriaTransacao.DataSource = categorias;

                btnRegistrarTransacao.Enabled =
                    contas.Count > 0 && categorias.Count > 0;

                if (!btnRegistrarTransacao.Enabled)
                {
                    MessageBox.Show(
                        "Cadastre pelo menos uma conta ativa e uma categoria.",
                        "JKCASH",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possível carregar as opções.\n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RegistrarTransacao()
        {
            if (_servico is null || _contas is null)
                return;

            try
            {
                if (cmbContaTransacao.SelectedValue is not Guid contaId ||
                    cmbCategoriaTransacao.SelectedValue is not Guid categoriaId)
                {
                    throw new ArgumentException(
                        "Selecione uma conta e uma categoria.");
                }

                if (cmbTipoTransacao.SelectedItem is not TipoTransacao tipo ||
                    cmbImportanciaTransacao.SelectedItem is not ImportanciaTransacao importancia ||
                    cmbPlanejamentoTransacao.SelectedItem is not PlanejamentoTransacao planejamento ||
                    cmbUrgenciaTransacao.SelectedItem is not UrgenciaTransacao urgencia)
                {
                    throw new ArgumentException(
                        "Preencha todas as opções da movimentação.");
                }

                Conta conta = _contas.ListarTodas()
                    .FirstOrDefault(conta => conta.Id == contaId)
                    ?? throw new ArgumentException("Conta não encontrada.");

                _servico.Registrar(
                    conta,
                    tipo,
                    numValorTransacao.Value,
                    txtDescricaoTransacao.Text,
                    dtpDataTransacao.Value.Date,
                    categoriaId,
                    importancia,
                    planejamento,
                    urgencia,
                    txtJustificativaTransacao.Text);

                numValorTransacao.Value = 0m;
                txtDescricaoTransacao.Clear();
                txtJustificativaTransacao.Clear();

                MessageBox.Show(
                    $"Transação registrada!\nSaldo atualizado: {conta.Saldo.SaldoAtual:C2}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                numValorTransacao.Focus();
            }
            catch (ArgumentException erro)
            {
                MessageBox.Show(
                    erro.Message,
                    "Confira os dados da transação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possível registrar a transação.\n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarOpcoes()
        {
            cmbTipoTransacao.DataSource =
                Enum.GetValues<TipoTransacao>();

            cmbImportanciaTransacao.DataSource =
                Enum.GetValues<ImportanciaTransacao>();

            cmbPlanejamentoTransacao.DataSource =
                Enum.GetValues<PlanejamentoTransacao>();

            cmbUrgenciaTransacao.DataSource =
                Enum.GetValues<UrgenciaTransacao>();
        }
    }
}
