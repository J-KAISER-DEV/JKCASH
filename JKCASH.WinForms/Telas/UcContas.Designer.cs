namespace JKCASH.WinForms.Telas
{
    partial class UcContas
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
            lbTituloContas = new Label();
            lbDescricaoContas = new Label();
            pnlCadastroConta = new Panel();
            btnSalvarConta = new Button();
            tlpDadosConta = new TableLayoutPanel();
            lblNomeConta = new Label();
            lblNomeBanco = new Label();
            txtNomeConta = new TextBox();
            txtBancoConta = new TextBox();
            lblTipoConta = new Label();
            lblDestinacaoConta = new Label();
            cmbTipoConta = new ComboBox();
            cmbDestinacaoConta = new ComboBox();
            lblSadoInicial = new Label();
            lblLimiteChequeEspecial = new Label();
            numSaldoInicial = new NumericUpDown();
            numLimiteChequeEspecial = new NumericUpDown();
            pnListaContas = new Panel();
            dgvContas = new DataGridView();
            pnlCadastroConta.SuspendLayout();
            tlpDadosConta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSaldoInicial).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLimiteChequeEspecial).BeginInit();
            pnListaContas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvContas).BeginInit();
            SuspendLayout();
            // 
            // lbTituloContas
            // 
            lbTituloContas.AutoSize = true;
            lbTituloContas.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbTituloContas.ForeColor = Color.FromArgb(22, 36, 55);
            lbTituloContas.Location = new Point(24, 20);
            lbTituloContas.Name = "lbTituloContas";
            lbTituloContas.Size = new Size(132, 47);
            lbTituloContas.TabIndex = 0;
            lbTituloContas.Text = "Contas";
            // 
            // lbDescricaoContas
            // 
            lbDescricaoContas.AutoSize = true;
            lbDescricaoContas.ForeColor = Color.FromArgb(105, 122, 143);
            lbDescricaoContas.Location = new Point(27, 78);
            lbDescricaoContas.Name = "lbDescricaoContas";
            lbDescricaoContas.Size = new Size(227, 19);
            lbDescricaoContas.TabIndex = 1;
            lbDescricaoContas.Text = "Cadastre e acompanhe suas contas.";
            // 
            // pnlCadastroConta
            // 
            pnlCadastroConta.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCadastroConta.BackColor = Color.White;
            pnlCadastroConta.Controls.Add(btnSalvarConta);
            pnlCadastroConta.Controls.Add(tlpDadosConta);
            pnlCadastroConta.Location = new Point(24, 120);
            pnlCadastroConta.Name = "pnlCadastroConta";
            pnlCadastroConta.Padding = new Padding(20);
            pnlCadastroConta.Size = new Size(950, 330);
            pnlCadastroConta.TabIndex = 2;
            // 
            // btnSalvarConta
            // 
            btnSalvarConta.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalvarConta.BackColor = Color.FromArgb(22, 134, 92);
            btnSalvarConta.Cursor = Cursors.Hand;
            btnSalvarConta.FlatAppearance.BorderSize = 0;
            btnSalvarConta.FlatStyle = FlatStyle.Flat;
            btnSalvarConta.ForeColor = Color.White;
            btnSalvarConta.Location = new Point(730, 270);
            btnSalvarConta.Name = "btnSalvarConta";
            btnSalvarConta.Size = new Size(180, 40);
            btnSalvarConta.TabIndex = 1;
            btnSalvarConta.Text = "Salvar conta";
            btnSalvarConta.UseVisualStyleBackColor = false;
            // 
            // tlpDadosConta
            // 
            tlpDadosConta.ColumnCount = 2;
            tlpDadosConta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosConta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosConta.Controls.Add(lblNomeConta, 0, 0);
            tlpDadosConta.Controls.Add(lblNomeBanco, 1, 0);
            tlpDadosConta.Controls.Add(txtNomeConta, 0, 1);
            tlpDadosConta.Controls.Add(txtBancoConta, 1, 1);
            tlpDadosConta.Controls.Add(lblTipoConta, 0, 2);
            tlpDadosConta.Controls.Add(lblDestinacaoConta, 1, 2);
            tlpDadosConta.Controls.Add(cmbTipoConta, 0, 3);
            tlpDadosConta.Controls.Add(cmbDestinacaoConta, 1, 3);
            tlpDadosConta.Controls.Add(lblSadoInicial, 0, 4);
            tlpDadosConta.Controls.Add(lblLimiteChequeEspecial, 1, 4);
            tlpDadosConta.Controls.Add(numSaldoInicial, 0, 5);
            tlpDadosConta.Controls.Add(numLimiteChequeEspecial, 1, 5);
            tlpDadosConta.Dock = DockStyle.Top;
            tlpDadosConta.Location = new Point(20, 20);
            tlpDadosConta.Name = "tlpDadosConta";
            tlpDadosConta.RowCount = 6;
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Percent, 66.6666641F));
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosConta.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosConta.Size = new Size(910, 216);
            tlpDadosConta.TabIndex = 0;
            tlpDadosConta.Paint += tlpDadosConta_Paint;
            // 
            // lblNomeConta
            // 
            lblNomeConta.AutoSize = true;
            lblNomeConta.Location = new Point(3, 0);
            lblNomeConta.Name = "lblNomeConta";
            lblNomeConta.Size = new Size(103, 19);
            lblNomeConta.TabIndex = 0;
            lblNomeConta.Text = "Nome da conta";
            // 
            // lblNomeBanco
            // 
            lblNomeBanco.AutoSize = true;
            lblNomeBanco.Location = new Point(458, 0);
            lblNomeBanco.Name = "lblNomeBanco";
            lblNomeBanco.Size = new Size(46, 19);
            lblNomeBanco.TabIndex = 1;
            lblNomeBanco.Text = "Banco";
            // 
            // txtNomeConta
            // 
            txtNomeConta.Dock = DockStyle.Top;
            txtNomeConta.Location = new Point(3, 27);
            txtNomeConta.MaxLength = 100;
            txtNomeConta.Name = "txtNomeConta";
            txtNomeConta.Size = new Size(449, 25);
            txtNomeConta.TabIndex = 2;
            // 
            // txtBancoConta
            // 
            txtBancoConta.Dock = DockStyle.Top;
            txtBancoConta.Location = new Point(458, 27);
            txtBancoConta.MaxLength = 100;
            txtBancoConta.Name = "txtBancoConta";
            txtBancoConta.Size = new Size(449, 25);
            txtBancoConta.TabIndex = 3;
            // 
            // lblTipoConta
            // 
            lblTipoConta.AutoSize = true;
            lblTipoConta.Location = new Point(3, 72);
            lblTipoConta.Name = "lblTipoConta";
            lblTipoConta.Size = new Size(92, 19);
            lblTipoConta.TabIndex = 4;
            lblTipoConta.Text = "Tipo de conta";
            // 
            // lblDestinacaoConta
            // 
            lblDestinacaoConta.AutoSize = true;
            lblDestinacaoConta.Location = new Point(458, 72);
            lblDestinacaoConta.Name = "lblDestinacaoConta";
            lblDestinacaoConta.Size = new Size(76, 19);
            lblDestinacaoConta.TabIndex = 5;
            lblDestinacaoConta.Text = "Destinação";
            // 
            // cmbTipoConta
            // 
            cmbTipoConta.Dock = DockStyle.Top;
            cmbTipoConta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoConta.FormattingEnabled = true;
            cmbTipoConta.Location = new Point(3, 99);
            cmbTipoConta.Name = "cmbTipoConta";
            cmbTipoConta.Size = new Size(449, 25);
            cmbTipoConta.TabIndex = 6;
            // 
            // cmbDestinacaoConta
            // 
            cmbDestinacaoConta.Dock = DockStyle.Top;
            cmbDestinacaoConta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDestinacaoConta.FormattingEnabled = true;
            cmbDestinacaoConta.Location = new Point(458, 99);
            cmbDestinacaoConta.Name = "cmbDestinacaoConta";
            cmbDestinacaoConta.Size = new Size(449, 25);
            cmbDestinacaoConta.TabIndex = 7;
            // 
            // lblSadoInicial
            // 
            lblSadoInicial.AutoSize = true;
            lblSadoInicial.Location = new Point(3, 144);
            lblSadoInicial.Name = "lblSadoInicial";
            lblSadoInicial.Size = new Size(111, 19);
            lblSadoInicial.TabIndex = 8;
            lblSadoInicial.Text = "Saldo inicial  (R$)";
            // 
            // lblLimiteChequeEspecial
            // 
            lblLimiteChequeEspecial.AutoSize = true;
            lblLimiteChequeEspecial.Location = new Point(458, 144);
            lblLimiteChequeEspecial.Name = "lblLimiteChequeEspecial";
            lblLimiteChequeEspecial.Size = new Size(193, 19);
            lblLimiteChequeEspecial.TabIndex = 9;
            lblLimiteChequeEspecial.Text = "Limite do cheque especial (R$)";
            // 
            // numSaldoInicial
            // 
            numSaldoInicial.DecimalPlaces = 2;
            numSaldoInicial.Dock = DockStyle.Top;
            numSaldoInicial.Location = new Point(3, 171);
            numSaldoInicial.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            numSaldoInicial.Minimum = new decimal(new int[] { 10000000, 0, 0, int.MinValue });
            numSaldoInicial.Name = "numSaldoInicial";
            numSaldoInicial.Size = new Size(449, 25);
            numSaldoInicial.TabIndex = 10;
            numSaldoInicial.ThousandsSeparator = true;
            // 
            // numLimiteChequeEspecial
            // 
            numLimiteChequeEspecial.DecimalPlaces = 2;
            numLimiteChequeEspecial.Dock = DockStyle.Top;
            numLimiteChequeEspecial.Location = new Point(458, 171);
            numLimiteChequeEspecial.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            numLimiteChequeEspecial.Name = "numLimiteChequeEspecial";
            numLimiteChequeEspecial.Size = new Size(449, 25);
            numLimiteChequeEspecial.TabIndex = 11;
            numLimiteChequeEspecial.ThousandsSeparator = true;
            // 
            // pnListaContas
            // 
            pnListaContas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnListaContas.BackColor = Color.White;
            pnListaContas.Controls.Add(dgvContas);
            pnListaContas.Location = new Point(24, 470);
            pnListaContas.Name = "pnListaContas";
            pnListaContas.Padding = new Padding(20);
            pnListaContas.Size = new Size(950, 200);
            pnListaContas.TabIndex = 3;
            // 
            // dgvContas
            // 
            dgvContas.AllowUserToAddRows = false;
            dgvContas.AllowUserToDeleteRows = false;
            dgvContas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContas.BackgroundColor = Color.White;
            dgvContas.BorderStyle = BorderStyle.None;
            dgvContas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContas.Dock = DockStyle.Fill;
            dgvContas.Location = new Point(20, 20);
            dgvContas.MultiSelect = false;
            dgvContas.Name = "dgvContas";
            dgvContas.ReadOnly = true;
            dgvContas.RowHeadersVisible = false;
            dgvContas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContas.Size = new Size(910, 160);
            dgvContas.TabIndex = 0;
            // 
            // UcContas
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(pnListaContas);
            Controls.Add(pnlCadastroConta);
            Controls.Add(lbDescricaoContas);
            Controls.Add(lbTituloContas);
            Font = new Font("Segoe UI", 10F);
            Name = "UcContas";
            Size = new Size(1000, 700);
            pnlCadastroConta.ResumeLayout(false);
            tlpDadosConta.ResumeLayout(false);
            tlpDadosConta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSaldoInicial).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLimiteChequeEspecial).EndInit();
            pnListaContas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvContas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbTituloContas;
        private Label lbDescricaoContas;
        private Panel pnlCadastroConta;
        private TableLayoutPanel tlpDadosConta;
        private Label lblNomeConta;
        private Label lblNomeBanco;
        private TextBox txtNomeConta;
        private TextBox txtBancoConta;
        private Label lblTipoConta;
        private Label lblDestinacaoConta;
        private ComboBox cmbTipoConta;
        private ComboBox cmbDestinacaoConta;
        private Label lblSadoInicial;
        private Label lblLimiteChequeEspecial;
        private NumericUpDown numSaldoInicial;
        private NumericUpDown numLimiteChequeEspecial;
        private Button btnSalvarConta;
        private Panel pnListaContas;
        private DataGridView dgvContas;
    }
}
