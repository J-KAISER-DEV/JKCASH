using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JKCASH.Domain.Enums;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
namespace JKCASH.WinForms.Telas
{
    public partial class UcContas : UserControl
    {
        private readonly IContaRepositorio? _repositorio;
        public UcContas()
        {
            InitializeComponent();
            CarregarOpcoes();
        }

        public UcContas(IContaRepositorio repositorio) : this()
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            _repositorio = repositorio;
            Load += (_, _) => CarregarContas();
            btnSalvarConta.Click += (_, _) => SalvarConta();
        }

        private void SalvarConta()
        {
            if (_repositorio is null)
                return;

            try
            {
                if (cmbTipoConta.SelectedItem is not TipoConta tipo ||
                    cmbDestinacaoConta.SelectedItem is not DestinacaoConta destinacao)
                {
                    throw new ArgumentException(
                        "Selecione o tipo e a destinação da conta.");
                }

                Conta conta = new(
                    txtNomeConta.Text,
                    txtBancoConta.Text,
                    tipo,
                    destinacao,
                    numSaldoInicial.Value,
                    numLimiteChequeEspecial.Value);

                _repositorio.Adicionar(conta);

                txtNomeConta.Clear();
                txtBancoConta.Clear();
                numSaldoInicial.Value = 0m;
                numLimiteChequeEspecial.Value = 0m;

                MessageBox.Show(
                    "Conta salva com sucesso!",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CarregarContas();
                txtNomeConta.Focus();
            }
            catch (ArgumentException erro)
            {
                MessageBox.Show(
                    erro.Message,
                    "Confira os dados da conta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possível salvar a conta.\n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarContas()
        {
            if (_repositorio is null)
            {
                return;
            }
            try
            {
                var contas = _repositorio.ListarTodas();
                dgvContas.DataSource = contas
                    .Select(conta => new
                    {
                        conta.Id,
                        conta.Nome,
                        conta.Banco,
                        conta.Tipo,
                        conta.Destinacao,
                        SaldoAtual = conta.Saldo.SaldoAtual,
                        LimiteChequeEspecial = conta.Saldo.LimiteChequeEspecial,
                        conta.Ativa
                    }).ToList();
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possivel carregar as contas. \n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
        }

        private void CarregarOpcoes()
        {
            cmbTipoConta.DataSource = Enum.GetValues<TipoConta>();
            cmbDestinacaoConta.DataSource =
                Enum.GetValues<DestinacaoConta>();
        }

        private void tlpDadosConta_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
