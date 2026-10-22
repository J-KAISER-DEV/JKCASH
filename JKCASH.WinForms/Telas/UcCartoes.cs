using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
namespace JKCASH.WinForms.Telas
{
    public partial class UcCartoes : UserControl
    {
        private readonly ICartaoCreditoRepositorio? _cartoes;
        private readonly IContaRepositorio? _contas;
        public UcCartoes()
        {
            InitializeComponent();
            ConfigurarTabelaCartoes();
            ConfigurarCompras();

        }

        public UcCartoes(
            ICartaoCreditoRepositorio cartoes,
            IContaRepositorio contas
            ) : this()
        {
            ArgumentNullException.ThrowIfNull(cartoes);
            ArgumentNullException.ThrowIfNull(contas);

            _cartoes = cartoes;
            _contas = contas;
            Load += (_, _) =>
            {
                CarregarContas();
                CarregarCartoes();
            };
            btnSalvarCartao.Click += async (_, _) => await SalvarCartaoAsync();
        }

        private void CarregarContas()
        {
            if (_contas is null)
                return;

            btnSalvarCartao.Enabled = false;

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

                cmbContaCartao.DisplayMember = nameof(Conta.Nome);
                cmbContaCartao.ValueMember = nameof(Conta.Id);
                cmbContaCartao.DataSource = contas;

                btnSalvarCartao.Enabled = contas.Count > 0;

                if (contas.Count == 0)
                {
                    txtResultadoCompra.Text = "Cadastre uma conta ativa para vincular o cartão.";
                }
            }
            catch (Exception erro)
            {
                System.Diagnostics.Debug.WriteLine(erro);
                cmbContaCartao.DataSource = null;
                txtResultadoCompra.Text = "Não foi possível carregar as contas. Confira a conexão e atualize os dados.";
            }
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

        private void CarregarCartoes()
        {
            if (_cartoes is null || _contas is null)
                return;

            try
            {
                var contas = _contas.ListarTodas()
                    .ToDictionary(conta => conta.Id);

                var cartoes = _cartoes.ListarTodos()
                    .Select(cartao => new
                    {
                        cartao.Nome,

                        Conta = contas.TryGetValue(
                            cartao.ContaId, out Conta? conta)
                            ? $"{conta.Nome} ({conta.Banco})"
                            : "Conta não encontrada",

                        cartao.Limite,
                        cartao.ValorUtilizado,
                        cartao.LimiteDisponivel,
                        cartao.DiaFechamento,
                        cartao.DiaVencimento
                    })
                    .ToList();

                dgvCartoes.DataSource = cartoes;
            }
            catch (Exception erro)
            {
                System.Diagnostics.Debug.WriteLine(erro);
                dgvCartoes.DataSource = null;
                txtResultadoCompra.Text = "Não foi possível carregar os cartões. Confira a conexão e atualize os dados.";
            }
        }

        private async Task SalvarCartaoAsync()
        {
            if (_operacaoCompra || _cartoes is null || _contas is null)
                return;

            try
            {
                if (cmbContaCartao.SelectedValue is not Guid contaId)
                {
                    throw new ArgumentException(
                        "Selecione a conta vinculada ao cartão.");
                }

                bool contaValida = _contas.ListarTodas()
                    .Any(conta => conta.Id == contaId && conta.Ativa);

                if (!contaValida)
                {
                    throw new ArgumentException(
                        "A conta selecionada não existe ou está inativa.");
                }

                CartaoCredito cartao = new(
                    contaId,
                    txtNomeCartao.Text,
                    numLimiteCartao.Value,
                    (int)numDiaFechamentoCartao.Value,
                    (int)numDiaVencimentoCartao.Value,
                    chkCompraNaProximaFatura.Checked);

                _cartoes.Adicionar(cartao);

                txtNomeCartao.Clear();
                numLimiteCartao.Value = 0m;

                MessageBox.Show(
                    "Cartão cadastrado com sucesso!",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CarregarCartoes();
                await CarregarDadosComprasAsync(cartao.Id);
                txtNomeCartao.Focus();
            }
            catch (ArgumentException erro)
            {
                MessageBox.Show(
                    erro.Message,
                    "Confira os dados do cartão",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possível cadastrar o cartão.\n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

    }
}
