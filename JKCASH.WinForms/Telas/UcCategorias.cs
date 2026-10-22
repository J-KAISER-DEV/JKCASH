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
    public partial class UcCategorias : UserControl
    {
        private readonly ICategoriaRepositorio? _repositorio;
        public UcCategorias()
        {
            InitializeComponent();
        }

        public UcCategorias(ICategoriaRepositorio repositorio) : this()
        {
            ArgumentNullException.ThrowIfNull(repositorio);

            _repositorio = repositorio;

            Load += (_, _) => CarregarCategorias();
            btnSalvarCategoria.Click += (_, _) => SalvarCategoria();
        }

        private void CarregarCategorias()
        {
            if (_repositorio is null)
            {
                return;
            }

            try
            {
                dgvCategorias.DataSource = _repositorio.ListarTodas();

                if (dgvCategorias.Columns[nameof(Categoria.Id)] is DataGridViewColumn colunaId)
                    colunaId.Visible = false;
            }
           
            catch (Exception erro) 
            {
                MessageBox.Show(
                    $"Não foi possivel carregar as categorias. \n {erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }
        private void SalvarCategoria()
        {
            if (_repositorio is null)
                return;

            try
            {
                Categoria categoria = new(txtNomeCategoria.Text);

                _repositorio.Adicionar(categoria);

                txtNomeCategoria.Clear();

                MessageBox.Show(
                    "Categoria salva com sucesso!",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CarregarCategorias();

                txtNomeCategoria.Focus();
            }
            catch (ArgumentException erro)
            {
                MessageBox.Show(
                    erro.Message,
                    "Confira o nome da categoria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception erro)
            {
                MessageBox.Show(
                    $"Não foi possível salvar a categoria.\n{erro.Message}",
                    "JKCASH",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
