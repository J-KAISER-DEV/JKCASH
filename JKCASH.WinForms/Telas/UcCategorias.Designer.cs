namespace JKCASH.WinForms.Telas
{
    partial class UcCategorias
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            lblTituloCategorias = new Label();
            lblDescricaoCategorias = new Label();
            pnlCadastroCategoria = new Panel();
            lblNomeCategoria = new Label();
            txtNomeCategoria = new TextBox();
            btnSalvarCategoria = new Button();
            pnListaCategorias = new Panel();
            dgvCategorias = new DataGridView();
            pnlCadastroCategoria.SuspendLayout();
            pnListaCategorias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).BeginInit();
            SuspendLayout();
            // 
            // lblTituloCategorias
            // 
            lblTituloCategorias.AutoSize = true;
            lblTituloCategorias.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTituloCategorias.ForeColor = Color.FromArgb(22, 36, 55);
            lblTituloCategorias.Location = new Point(24, 20);
            lblTituloCategorias.Name = "lblTituloCategorias";
            lblTituloCategorias.Size = new Size(195, 47);
            lblTituloCategorias.TabIndex = 0;
            lblTituloCategorias.Text = "Categorias";
            // 
            // lblDescricaoCategorias
            // 
            lblDescricaoCategorias.AutoSize = true;
            lblDescricaoCategorias.ForeColor = Color.FromArgb(105, 122, 143);
            lblDescricaoCategorias.Location = new Point(27, 78);
            lblDescricaoCategorias.Name = "lblDescricaoCategorias";
            lblDescricaoCategorias.Size = new Size(280, 19);
            lblDescricaoCategorias.TabIndex = 1;
            lblDescricaoCategorias.Text = "Organize suas movimentações por categoria";
            // 
            // pnlCadastroCategoria
            // 
            pnlCadastroCategoria.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCadastroCategoria.BackColor = Color.White;
            pnlCadastroCategoria.Controls.Add(btnSalvarCategoria);
            pnlCadastroCategoria.Controls.Add(txtNomeCategoria);
            pnlCadastroCategoria.Controls.Add(lblNomeCategoria);
            pnlCadastroCategoria.Location = new Point(24, 120);
            pnlCadastroCategoria.Name = "pnlCadastroCategoria";
            pnlCadastroCategoria.Padding = new Padding(20);
            pnlCadastroCategoria.Size = new Size(950, 150);
            pnlCadastroCategoria.TabIndex = 2;
            // 
            // lblNomeCategoria
            // 
            lblNomeCategoria.AutoSize = true;
            lblNomeCategoria.Font = new Font("Segoe UI", 10F);
            lblNomeCategoria.Location = new Point(20, 20);
            lblNomeCategoria.Name = "lblNomeCategoria";
            lblNomeCategoria.Size = new Size(125, 19);
            lblNomeCategoria.TabIndex = 0;
            lblNomeCategoria.Text = "Nome da categoria";
            // 
            // txtNomeCategoria
            // 
            txtNomeCategoria.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNomeCategoria.Location = new Point(20, 52);
            txtNomeCategoria.MaxLength = 100;
            txtNomeCategoria.Name = "txtNomeCategoria";
            txtNomeCategoria.PlaceholderText = "Ex.: Alimentação, Estudos, Transporte";
            txtNomeCategoria.Size = new Size(600, 25);
            txtNomeCategoria.TabIndex = 1;
            // 
            // btnSalvarCategoria
            // 
            btnSalvarCategoria.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalvarCategoria.BackColor = Color.FromArgb(22, 134, 92);
            btnSalvarCategoria.Cursor = Cursors.Hand;
            btnSalvarCategoria.FlatAppearance.BorderSize = 0;
            btnSalvarCategoria.FlatStyle = FlatStyle.Flat;
            btnSalvarCategoria.ForeColor = Color.White;
            btnSalvarCategoria.Location = new Point(650, 46);
            btnSalvarCategoria.Name = "btnSalvarCategoria";
            btnSalvarCategoria.Size = new Size(180, 40);
            btnSalvarCategoria.TabIndex = 2;
            btnSalvarCategoria.Text = "Salvar Categoria";
            btnSalvarCategoria.UseVisualStyleBackColor = false;
            // 
            // pnListaCategorias
            // 
            pnListaCategorias.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnListaCategorias.BackColor = Color.White;
            pnListaCategorias.Controls.Add(dgvCategorias);
            pnListaCategorias.Location = new Point(24, 290);
            pnListaCategorias.Name = "pnListaCategorias";
            pnListaCategorias.Padding = new Padding(20);
            pnListaCategorias.Size = new Size(950, 380);
            pnListaCategorias.TabIndex = 3;
            // 
            // dgvCategorias
            // 
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.AllowUserToDeleteRows = false;
            dgvCategorias.AllowUserToResizeColumns = false;
            dgvCategorias.AllowUserToResizeRows = false;
            dgvCategorias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategorias.BackgroundColor = Color.White;
            dgvCategorias.BorderStyle = BorderStyle.None;
            dgvCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategorias.Dock = DockStyle.Fill;
            dgvCategorias.Location = new Point(20, 20);
            dgvCategorias.MultiSelect = false;
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.ReadOnly = true;
            dgvCategorias.RowHeadersVisible = false;
            dgvCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategorias.Size = new Size(910, 340);
            dgvCategorias.TabIndex = 0;
            // 
            // UcCategorias
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(pnListaCategorias);
            Controls.Add(pnlCadastroCategoria);
            Controls.Add(lblDescricaoCategorias);
            Controls.Add(lblTituloCategorias);
            Font = new Font("Segoe UI", 10F);
            Name = "UcCategorias";
            Size = new Size(1000, 700);
            pnlCadastroCategoria.ResumeLayout(false);
            pnlCadastroCategoria.PerformLayout();
            pnListaCategorias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTituloCategorias;
        private Label lblDescricaoCategorias;
        private Panel pnlCadastroCategoria;
        private TextBox txtNomeCategoria;
        private Label lblNomeCategoria;
        private Button btnSalvarCategoria;
        private Panel pnListaCategorias;
        private DataGridView dgvCategorias;
    }
}
