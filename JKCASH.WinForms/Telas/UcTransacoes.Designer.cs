namespace JKCASH.WinForms.Telas
{
    partial class UcTransacoes
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
            lblTituloTransacoes = new Label();
            lblTransacoes = new Label();
            pnlCadastroTransacao = new Panel();
            btnRegistrarTransacao = new Button();
            txtJustificativaTransacao = new TextBox();
            lblJustificativaTransacao = new Label();
            txtDescricaoTransacao = new TextBox();
            lblDescricaoTransacao = new Label();
            tlpDadosTransacao = new TableLayoutPanel();
            cmbUrgenciaTransacao = new ComboBox();
            cmbPlanejamentoTransacao = new ComboBox();
            lblUrgenciaTransacao = new Label();
            cmbImportanciaTransacao = new ComboBox();
            lblContTransacao = new Label();
            lblTipoTransacao = new Label();
            cmbContaTransacao = new ComboBox();
            cmbTipoTransacao = new ComboBox();
            lblValorTransacao = new Label();
            lblDataTransacao = new Label();
            numValorTransacao = new NumericUpDown();
            dtpDataTransacao = new DateTimePicker();
            lblCategoriaTransacao = new Label();
            lblImportanciaTransacao = new Label();
            cmbCategoriaTransacao = new ComboBox();
            lblPlanejamentoTransacao = new Label();
            pnlCadastroTransacao.SuspendLayout();
            tlpDadosTransacao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValorTransacao).BeginInit();
            SuspendLayout();
            // 
            // lblTituloTransacoes
            // 
            lblTituloTransacoes.AutoSize = true;
            lblTituloTransacoes.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTituloTransacoes.ForeColor = Color.FromArgb(22, 36, 55);
            lblTituloTransacoes.Location = new Point(24, 20);
            lblTituloTransacoes.Name = "lblTituloTransacoes";
            lblTituloTransacoes.Size = new Size(198, 47);
            lblTituloTransacoes.TabIndex = 0;
            lblTituloTransacoes.Text = "Transações";
            // 
            // lblTransacoes
            // 
            lblTransacoes.AutoSize = true;
            lblTransacoes.ForeColor = Color.FromArgb(105, 122, 143);
            lblTransacoes.Location = new Point(27, 78);
            lblTransacoes.Name = "lblTransacoes";
            lblTransacoes.Size = new Size(275, 19);
            lblTransacoes.TabIndex = 1;
            lblTransacoes.Text = "Registre entradas, saídas e transferências entre contas.";
            // 
            // pnlCadastroTransacao
            // 
            pnlCadastroTransacao.AutoScroll = true;
            pnlCadastroTransacao.BackColor = Color.White;
            pnlCadastroTransacao.Controls.Add(btnRegistrarTransacao);
            pnlCadastroTransacao.Controls.Add(txtJustificativaTransacao);
            pnlCadastroTransacao.Controls.Add(lblJustificativaTransacao);
            pnlCadastroTransacao.Controls.Add(txtDescricaoTransacao);
            pnlCadastroTransacao.Controls.Add(lblDescricaoTransacao);
            pnlCadastroTransacao.Controls.Add(tlpDadosTransacao);
            pnlCadastroTransacao.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlCadastroTransacao.Location = new Point(24, 164);
            pnlCadastroTransacao.Name = "pnlCadastroTransacao";
            pnlCadastroTransacao.Padding = new Padding(20);
            pnlCadastroTransacao.Size = new Size(950, 706);
            pnlCadastroTransacao.TabIndex = 3;

            // 
            // btnRegistrarTransacao
            // 
            btnRegistrarTransacao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRegistrarTransacao.BackColor = Color.FromArgb(22, 134, 92);
            btnRegistrarTransacao.Cursor = Cursors.Hand;
            btnRegistrarTransacao.FlatAppearance.BorderSize = 0;
            btnRegistrarTransacao.FlatStyle = FlatStyle.Flat;
            btnRegistrarTransacao.ForeColor = Color.White;
            btnRegistrarTransacao.Location = new Point(739, 560);
            btnRegistrarTransacao.Name = "btnRegistrarTransacao";
            btnRegistrarTransacao.Size = new Size(180, 40);
            btnRegistrarTransacao.TabIndex = 5;
            btnRegistrarTransacao.Text = "Registrar Transação";
            btnRegistrarTransacao.UseVisualStyleBackColor = false;
            // 
            // txtJustificativaTransacao
            // 
            txtJustificativaTransacao.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJustificativaTransacao.Location = new Point(23, 424);
            txtJustificativaTransacao.MaxLength = 500;
            txtJustificativaTransacao.Multiline = true;
            txtJustificativaTransacao.Name = "txtJustificativaTransacao";
            txtJustificativaTransacao.PlaceholderText = "Explique o motivo da movimentação.";
            txtJustificativaTransacao.ScrollBars = ScrollBars.Vertical;
            txtJustificativaTransacao.Size = new Size(896, 112);
            txtJustificativaTransacao.TabIndex = 4;
            // 
            // lblJustificativaTransacao
            // 
            lblJustificativaTransacao.AutoSize = true;
            lblJustificativaTransacao.Location = new Point(23, 398);
            lblJustificativaTransacao.Name = "lblJustificativaTransacao";
            lblJustificativaTransacao.Size = new Size(78, 19);
            lblJustificativaTransacao.TabIndex = 3;
            lblJustificativaTransacao.Text = "Justificativa";
            // 
            // txtDescricaoTransacao
            // 
            txtDescricaoTransacao.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescricaoTransacao.Location = new Point(23, 352);
            txtDescricaoTransacao.MaxLength = 200;
            txtDescricaoTransacao.Name = "txtDescricaoTransacao";
            txtDescricaoTransacao.PlaceholderText = "Ex: Compra de livros";
            txtDescricaoTransacao.Size = new Size(896, 25);
            txtDescricaoTransacao.TabIndex = 2;
            // 
            // lblDescricaoTransacao
            // 
            lblDescricaoTransacao.AutoSize = true;
            lblDescricaoTransacao.Location = new Point(23, 326);
            lblDescricaoTransacao.Name = "lblDescricaoTransacao";
            lblDescricaoTransacao.Size = new Size(67, 19);
            lblDescricaoTransacao.TabIndex = 1;
            lblDescricaoTransacao.Text = "Descrição";
            // 
            // tlpDadosTransacao
            // 
            tlpDadosTransacao.ColumnCount = 2;
            tlpDadosTransacao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosTransacao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosTransacao.Controls.Add(cmbUrgenciaTransacao, 1, 7);
            tlpDadosTransacao.Controls.Add(cmbPlanejamentoTransacao, 0, 7);
            tlpDadosTransacao.Controls.Add(lblUrgenciaTransacao, 1, 6);
            tlpDadosTransacao.Controls.Add(cmbImportanciaTransacao, 1, 5);
            tlpDadosTransacao.Controls.Add(lblContTransacao, 0, 0);
            tlpDadosTransacao.Controls.Add(cmbContaTransacao, 0, 1);
            tlpDadosTransacao.Controls.Add(lblValorTransacao, 0, 2);
            tlpDadosTransacao.Controls.Add(lblDataTransacao, 1, 2);
            tlpDadosTransacao.Controls.Add(numValorTransacao, 0, 3);
            tlpDadosTransacao.Controls.Add(dtpDataTransacao, 1, 3);
            tlpDadosTransacao.Controls.Add(lblCategoriaTransacao, 0, 4);
            tlpDadosTransacao.Controls.Add(lblImportanciaTransacao, 1, 4);
            tlpDadosTransacao.Controls.Add(cmbCategoriaTransacao, 0, 5);
            tlpDadosTransacao.Controls.Add(lblPlanejamentoTransacao, 0, 6);
            tlpDadosTransacao.Dock = DockStyle.Top;
            tlpDadosTransacao.Location = new Point(20, 20);
            tlpDadosTransacao.Name = "tlpDadosTransacao";
            tlpDadosTransacao.RowCount = 8;
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosTransacao.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosTransacao.Size = new Size(902, 288);
            tlpDadosTransacao.TabIndex = 0;
            // 
            // cmbUrgenciaTransacao
            // 
            cmbUrgenciaTransacao.Dock = DockStyle.Top;
            cmbUrgenciaTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUrgenciaTransacao.FormattingEnabled = true;
            cmbUrgenciaTransacao.Location = new Point(454, 243);
            cmbUrgenciaTransacao.Name = "cmbUrgenciaTransacao";
            cmbUrgenciaTransacao.Size = new Size(445, 25);
            cmbUrgenciaTransacao.TabIndex = 15;
            // 
            // cmbPlanejamentoTransacao
            // 
            cmbPlanejamentoTransacao.Dock = DockStyle.Top;
            cmbPlanejamentoTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlanejamentoTransacao.FormattingEnabled = true;
            cmbPlanejamentoTransacao.Location = new Point(3, 243);
            cmbPlanejamentoTransacao.Name = "cmbPlanejamentoTransacao";
            cmbPlanejamentoTransacao.Size = new Size(445, 25);
            cmbPlanejamentoTransacao.TabIndex = 14;
            // 
            // lblUrgenciaTransacao
            // 
            lblUrgenciaTransacao.AutoSize = true;
            lblUrgenciaTransacao.Location = new Point(454, 216);
            lblUrgenciaTransacao.Name = "lblUrgenciaTransacao";
            lblUrgenciaTransacao.Size = new Size(63, 19);
            lblUrgenciaTransacao.TabIndex = 13;
            lblUrgenciaTransacao.Text = "Urgencia";
            // 
            // cmbImportanciaTransacao
            // 
            cmbImportanciaTransacao.Dock = DockStyle.Top;
            cmbImportanciaTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImportanciaTransacao.FormattingEnabled = true;
            cmbImportanciaTransacao.Location = new Point(454, 171);
            cmbImportanciaTransacao.Name = "cmbImportanciaTransacao";
            cmbImportanciaTransacao.Size = new Size(445, 25);
            cmbImportanciaTransacao.TabIndex = 11;
            // 
            // lblContTransacao
            // 
            lblContTransacao.AutoSize = true;
            lblContTransacao.Location = new Point(3, 0);
            lblContTransacao.Name = "lblContTransacao";
            lblContTransacao.Size = new Size(46, 19);
            lblContTransacao.TabIndex = 0;
            lblContTransacao.Text = "Conta";
            // 
            // lblTipoTransacao
            // 
            lblTipoTransacao.AutoSize = true;
            lblTipoTransacao.Location = new Point(24, 126);
            lblTipoTransacao.Name = "lblTipoTransacao";
            lblTipoTransacao.Size = new Size(148, 19);
            lblTipoTransacao.TabIndex = 1;
            lblTipoTransacao.Text = "Tipo de movimentação";
            // 
            // cmbContaTransacao
            // 
            cmbContaTransacao.Dock = DockStyle.Top;
            cmbContaTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbContaTransacao.FormattingEnabled = true;
            cmbContaTransacao.Location = new Point(3, 27);
            cmbContaTransacao.Name = "cmbContaTransacao";
            cmbContaTransacao.Size = new Size(445, 25);
            cmbContaTransacao.TabIndex = 2;
            // 
            // cmbTipoTransacao
            // 
                        cmbTipoTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoTransacao.FormattingEnabled = true;
            cmbTipoTransacao.Location = new Point(194, 123);
            cmbTipoTransacao.Name = "cmbTipoTransacao";
            cmbTipoTransacao.Size = new Size(265, 25);
            cmbTipoTransacao.TabIndex = 2;
            // 
            // lblValorTransacao
            // 
            lblValorTransacao.AutoSize = true;
            lblValorTransacao.Location = new Point(3, 72);
            lblValorTransacao.Name = "lblValorTransacao";
            lblValorTransacao.Size = new Size(68, 19);
            lblValorTransacao.TabIndex = 4;
            lblValorTransacao.Text = "Valor (R$)";
            // 
            // lblDataTransacao
            // 
            lblDataTransacao.AutoSize = true;
            lblDataTransacao.Location = new Point(454, 72);
            lblDataTransacao.Name = "lblDataTransacao";
            lblDataTransacao.Size = new Size(151, 19);
            lblDataTransacao.TabIndex = 5;
            lblDataTransacao.Text = "Data da movimentação";
            // 
            // numValorTransacao
            // 
            numValorTransacao.DecimalPlaces = 2;
            numValorTransacao.Dock = DockStyle.Top;
            numValorTransacao.Location = new Point(3, 99);
            numValorTransacao.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            numValorTransacao.Name = "numValorTransacao";
            numValorTransacao.Size = new Size(445, 25);
            numValorTransacao.TabIndex = 6;
            numValorTransacao.ThousandsSeparator = true;
            // 
            // dtpDataTransacao
            // 
            dtpDataTransacao.CustomFormat = "dd/MM/yyyy";
            dtpDataTransacao.Dock = DockStyle.Top;
            dtpDataTransacao.Format = DateTimePickerFormat.Custom;
            dtpDataTransacao.Location = new Point(454, 99);
            dtpDataTransacao.Name = "dtpDataTransacao";
            dtpDataTransacao.Size = new Size(445, 25);
            dtpDataTransacao.TabIndex = 7;
            // 
            // lblCategoriaTransacao
            // 
            lblCategoriaTransacao.AutoSize = true;
            lblCategoriaTransacao.Location = new Point(3, 144);
            lblCategoriaTransacao.Name = "lblCategoriaTransacao";
            lblCategoriaTransacao.Size = new Size(68, 19);
            lblCategoriaTransacao.TabIndex = 8;
            lblCategoriaTransacao.Text = "Categoria";
            // 
            // lblImportanciaTransacao
            // 
            lblImportanciaTransacao.AutoSize = true;
            lblImportanciaTransacao.Location = new Point(454, 144);
            lblImportanciaTransacao.Name = "lblImportanciaTransacao";
            lblImportanciaTransacao.Size = new Size(82, 19);
            lblImportanciaTransacao.TabIndex = 9;
            lblImportanciaTransacao.Text = "Importancia";
            // 
            // cmbCategoriaTransacao
            // 
            cmbCategoriaTransacao.Dock = DockStyle.Top;
            cmbCategoriaTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaTransacao.FormattingEnabled = true;
            cmbCategoriaTransacao.Location = new Point(3, 171);
            cmbCategoriaTransacao.Name = "cmbCategoriaTransacao";
            cmbCategoriaTransacao.Size = new Size(445, 25);
            cmbCategoriaTransacao.TabIndex = 10;
            // 
            // lblPlanejamentoTransacao
            // 
            lblPlanejamentoTransacao.AutoSize = true;
            lblPlanejamentoTransacao.Location = new Point(3, 216);
            lblPlanejamentoTransacao.Name = "lblPlanejamentoTransacao";
            lblPlanejamentoTransacao.Size = new Size(92, 19);
            lblPlanejamentoTransacao.TabIndex = 12;
            lblPlanejamentoTransacao.Text = "Planejamento";
            // 
            // UcTransacoes
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(pnlCadastroTransacao);
            Controls.Add(lblTipoTransacao);
            Controls.Add(cmbTipoTransacao);
            Controls.Add(lblTransacoes);
            Controls.Add(lblTituloTransacoes);
            Font = new Font("Segoe UI", 10F);
            Name = "UcTransacoes";
            Size = new Size(1000, 900);
            pnlCadastroTransacao.ResumeLayout(false);
            pnlCadastroTransacao.PerformLayout();
            tlpDadosTransacao.ResumeLayout(false);
            tlpDadosTransacao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValorTransacao).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTituloTransacoes;
        private Label lblTransacoes;
        private Panel pnlCadastroTransacao;
        private TableLayoutPanel tlpDadosTransacao;
        private Label lblContTransacao;
        private Label lblTipoTransacao;
        private ComboBox cmbContaTransacao;
        private ComboBox cmbTipoTransacao;
        private Label lblValorTransacao;
        private Label lblDataTransacao;
        private NumericUpDown numValorTransacao;
        private DateTimePicker dtpDataTransacao;
        private ComboBox cmbImportanciaTransacao;
        private Label lblCategoriaTransacao;
        private Label lblImportanciaTransacao;
        private ComboBox cmbCategoriaTransacao;
        private ComboBox cmbUrgenciaTransacao;
        private ComboBox cmbPlanejamentoTransacao;
        private Label lblUrgenciaTransacao;
        private Label lblPlanejamentoTransacao;
        private TextBox txtJustificativaTransacao;
        private Label lblJustificativaTransacao;
        private TextBox txtDescricaoTransacao;
        private Label lblDescricaoTransacao;
        private Button btnRegistrarTransacao;
    }
}
