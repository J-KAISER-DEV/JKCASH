namespace JKCASH.WinForms.Telas
{
    partial class UcCartoes
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
            lblTituloCartoes = new Label();
            lblDescricaoCartao = new Label();
            pnlListaCartoes = new Panel();
            dgvCartoes = new DataGridView();
            tlpDadosCartao = new TableLayoutPanel();
            chkCompraNaProximaFatura = new CheckBox();
            numDiaVencimentoCartao = new NumericUpDown();
            lblDiaVencimentoCartao = new Label();
            numLimiteCartao = new NumericUpDown();
            txtNomeCartao = new TextBox();
            cmbContaCartao = new ComboBox();
            lblContaCartao = new Label();
            lblNomeCartao = new Label();
            lblLimiteCartao = new Label();
            lblDiaFechamentoCartao = new Label();
            numDiaFechamentoCartao = new NumericUpDown();
            lblRegraFechamentoCartao = new Label();
            btnSalvarCartao = new Button();
            tabCartoes = new TabControl();
            tabCadastroCartoes = new TabPage();
            pnlCadastroCartao = new Panel();
            txtResultadoCartao = new TextBox();
            btnAtualizarCartoes = new Button();
            pnlListaCartoes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCartoes).BeginInit();
            tlpDadosCartao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDiaVencimentoCartao).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLimiteCartao).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDiaFechamentoCartao).BeginInit();
            tabCartoes.SuspendLayout();
            tabCadastroCartoes.SuspendLayout();
            pnlCadastroCartao.SuspendLayout();
            SuspendLayout();
            // 
            // lblTituloCartoes
            // 
            lblTituloCartoes.AutoSize = true;
            lblTituloCartoes.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTituloCartoes.ForeColor = Color.FromArgb(20, 38, 61);
            lblTituloCartoes.Location = new Point(24, 27);
            lblTituloCartoes.Name = "lblTituloCartoes";
            lblTituloCartoes.Size = new Size(295, 45);
            lblTituloCartoes.TabIndex = 1;
            lblTituloCartoes.Text = "Cartões de crédito";
            // 
            // lblDescricaoCartao
            // 
            lblDescricaoCartao.AutoSize = true;
            lblDescricaoCartao.Font = new Font("Segoe UI", 10F);
            lblDescricaoCartao.ForeColor = Color.FromArgb(100, 116, 139);
            lblDescricaoCartao.Location = new Point(24, 86);
            lblDescricaoCartao.Name = "lblDescricaoCartao";
            lblDescricaoCartao.Size = new Size(292, 19);
            lblDescricaoCartao.TabIndex = 2;
            lblDescricaoCartao.Text = "Cadastre cartões e confira seus limites.";
            // 
            // pnlListaCartoes
            // 
            pnlListaCartoes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlListaCartoes.BackColor = Color.White;
            pnlListaCartoes.Controls.Add(dgvCartoes);
            pnlListaCartoes.Location = new Point(20, 418);
            pnlListaCartoes.Name = "pnlListaCartoes";
            pnlListaCartoes.Padding = new Padding(20);
            pnlListaCartoes.Size = new Size(900, 280);
            pnlListaCartoes.TabIndex = 3;
            // 
            // dgvCartoes
            // 
            dgvCartoes.AllowUserToAddRows = false;
            dgvCartoes.AllowUserToDeleteRows = false;
            dgvCartoes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCartoes.BackgroundColor = Color.White;
            dgvCartoes.BorderStyle = BorderStyle.None;
            dgvCartoes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCartoes.Dock = DockStyle.Fill;
            dgvCartoes.Location = new Point(20, 20);
            dgvCartoes.MultiSelect = false;
            dgvCartoes.Name = "dgvCartoes";
            dgvCartoes.ReadOnly = true;
            dgvCartoes.RowHeadersVisible = false;
            dgvCartoes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCartoes.Size = new Size(860, 240);
            dgvCartoes.TabIndex = 0;
            // 
            // tlpDadosCartao
            // 
            tlpDadosCartao.ColumnCount = 2;
            tlpDadosCartao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosCartao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosCartao.Controls.Add(lblRegraFechamentoCartao, 1, 4);
            tlpDadosCartao.Controls.Add(numDiaFechamentoCartao, 1, 3);
            tlpDadosCartao.Controls.Add(lblDiaFechamentoCartao, 1, 2);
            tlpDadosCartao.Controls.Add(lblLimiteCartao, 0, 2);
            tlpDadosCartao.Controls.Add(lblNomeCartao, 1, 0);
            tlpDadosCartao.Controls.Add(lblContaCartao, 0, 0);
            tlpDadosCartao.Controls.Add(cmbContaCartao, 0, 1);
            tlpDadosCartao.Controls.Add(txtNomeCartao, 1, 1);
            tlpDadosCartao.Controls.Add(numLimiteCartao, 0, 3);
            tlpDadosCartao.Controls.Add(lblDiaVencimentoCartao, 0, 4);
            tlpDadosCartao.Controls.Add(numDiaVencimentoCartao, 0, 5);
            tlpDadosCartao.Controls.Add(chkCompraNaProximaFatura, 1, 5);
            tlpDadosCartao.Dock = DockStyle.Top;
            tlpDadosCartao.Location = new Point(20, 23);
            tlpDadosCartao.Name = "tlpDadosCartao";
            tlpDadosCartao.RowCount = 6;
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            tlpDadosCartao.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            tlpDadosCartao.Size = new Size(860, 245);
            tlpDadosCartao.TabIndex = 0;
            // 
            // chkCompraNaProximaFatura
            // 
            chkCompraNaProximaFatura.AutoSize = true;
            chkCompraNaProximaFatura.Checked = true;
            chkCompraNaProximaFatura.CheckState = CheckState.Checked;
            chkCompraNaProximaFatura.Location = new Point(433, 192);
            chkCompraNaProximaFatura.Name = "chkCompraNaProximaFatura";
            chkCompraNaProximaFatura.Size = new Size(200, 23);
            chkCompraNaProximaFatura.TabIndex = 11;
            chkCompraNaProximaFatura.Text = "Enviar para a próxima fatura";
            chkCompraNaProximaFatura.UseVisualStyleBackColor = true;
            // 
            // numDiaVencimentoCartao
            // 
            numDiaVencimentoCartao.Dock = DockStyle.Top;
            numDiaVencimentoCartao.Location = new Point(3, 192);
            numDiaVencimentoCartao.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            numDiaVencimentoCartao.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDiaVencimentoCartao.Name = "numDiaVencimentoCartao";
            numDiaVencimentoCartao.Size = new Size(424, 25);
            numDiaVencimentoCartao.TabIndex = 10;
            numDiaVencimentoCartao.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblDiaVencimentoCartao
            // 
            lblDiaVencimentoCartao.AutoSize = true;
            lblDiaVencimentoCartao.Location = new Point(3, 162);
            lblDiaVencimentoCartao.Name = "lblDiaVencimentoCartao";
            lblDiaVencimentoCartao.Size = new Size(123, 19);
            lblDiaVencimentoCartao.TabIndex = 8;
            lblDiaVencimentoCartao.Text = "Dia de vencimento";
            // 
            // numLimiteCartao
            // 
            numLimiteCartao.DecimalPlaces = 2;
            numLimiteCartao.Dock = DockStyle.Top;
            numLimiteCartao.Location = new Point(3, 111);
            numLimiteCartao.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numLimiteCartao.Name = "numLimiteCartao";
            numLimiteCartao.Size = new Size(424, 25);
            numLimiteCartao.TabIndex = 6;
            numLimiteCartao.ThousandsSeparator = true;
            // 
            // txtNomeCartao
            // 
            txtNomeCartao.Dock = DockStyle.Top;
            txtNomeCartao.Location = new Point(433, 30);
            txtNomeCartao.MaxLength = 100;
            txtNomeCartao.Name = "txtNomeCartao";
            txtNomeCartao.Size = new Size(424, 25);
            txtNomeCartao.TabIndex = 3;
            // 
            // cmbContaCartao
            // 
            cmbContaCartao.Dock = DockStyle.Top;
            cmbContaCartao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbContaCartao.FormattingEnabled = true;
            cmbContaCartao.Location = new Point(3, 30);
            cmbContaCartao.Name = "cmbContaCartao";
            cmbContaCartao.Size = new Size(424, 25);
            cmbContaCartao.TabIndex = 2;
            // 
            // lblContaCartao
            // 
            lblContaCartao.AutoSize = true;
            lblContaCartao.Location = new Point(3, 0);
            lblContaCartao.Name = "lblContaCartao";
            lblContaCartao.Size = new Size(107, 19);
            lblContaCartao.TabIndex = 0;
            lblContaCartao.Text = "Conta vinculada";
            // 
            // lblNomeCartao
            // 
            lblNomeCartao.AutoSize = true;
            lblNomeCartao.Location = new Point(433, 0);
            lblNomeCartao.Name = "lblNomeCartao";
            lblNomeCartao.Size = new Size(108, 19);
            lblNomeCartao.TabIndex = 1;
            lblNomeCartao.Text = "Nome do cartão";
            // 
            // lblLimiteCartao
            // 
            lblLimiteCartao.AutoSize = true;
            lblLimiteCartao.Location = new Point(3, 81);
            lblLimiteCartao.Name = "lblLimiteCartao";
            lblLimiteCartao.Size = new Size(108, 19);
            lblLimiteCartao.TabIndex = 4;
            lblLimiteCartao.Text = "Limite do cartão";
            // 
            // lblDiaFechamentoCartao
            // 
            lblDiaFechamentoCartao.AutoSize = true;
            lblDiaFechamentoCartao.Location = new Point(433, 81);
            lblDiaFechamentoCartao.Name = "lblDiaFechamentoCartao";
            lblDiaFechamentoCartao.Size = new Size(124, 19);
            lblDiaFechamentoCartao.TabIndex = 5;
            lblDiaFechamentoCartao.Text = "Dia de fechamento";
            // 
            // numDiaFechamentoCartao
            // 
            numDiaFechamentoCartao.Dock = DockStyle.Top;
            numDiaFechamentoCartao.Location = new Point(433, 111);
            numDiaFechamentoCartao.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            numDiaFechamentoCartao.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDiaFechamentoCartao.Name = "numDiaFechamentoCartao";
            numDiaFechamentoCartao.Size = new Size(424, 25);
            numDiaFechamentoCartao.TabIndex = 7;
            numDiaFechamentoCartao.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblRegraFechamentoCartao
            // 
            lblRegraFechamentoCartao.AutoSize = true;
            lblRegraFechamentoCartao.Location = new Point(433, 162);
            lblRegraFechamentoCartao.Name = "lblRegraFechamentoCartao";
            lblRegraFechamentoCartao.Size = new Size(202, 19);
            lblRegraFechamentoCartao.TabIndex = 9;
            lblRegraFechamentoCartao.Text = "Compras no dia do fechamento";
            // 
            // btnSalvarCartao
            // 
            btnSalvarCartao.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalvarCartao.BackColor = Color.FromArgb(22, 163, 74);
            btnSalvarCartao.Cursor = Cursors.Hand;
            btnSalvarCartao.FlatAppearance.BorderSize = 0;
            btnSalvarCartao.FlatStyle = FlatStyle.Flat;
            btnSalvarCartao.ForeColor = Color.White;
            btnSalvarCartao.Location = new Point(700, 310);
            btnSalvarCartao.Name = "btnSalvarCartao";
            btnSalvarCartao.Size = new Size(180, 40);
            btnSalvarCartao.TabIndex = 3;
            btnSalvarCartao.Text = "Cadastrar cartão";
            btnSalvarCartao.UseVisualStyleBackColor = false;
            // 
            // tabCartoes
            // 
            tabCartoes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabCartoes.Controls.Add(tabCadastroCartoes);
            tabCartoes.Location = new Point(24, 120);
            tabCartoes.Name = "tabCartoes";
            tabCartoes.SelectedIndex = 0;
            tabCartoes.Size = new Size(950, 750);
            tabCartoes.TabIndex = 4;
            // 
            // tabCadastroCartoes
            // 
            tabCadastroCartoes.BackColor = Color.White;
            tabCadastroCartoes.Controls.Add(pnlCadastroCartao);
            tabCadastroCartoes.Controls.Add(pnlListaCartoes);
            tabCadastroCartoes.Location = new Point(4, 26);
            tabCadastroCartoes.Name = "tabCadastroCartoes";
            tabCadastroCartoes.Padding = new Padding(20);
            tabCadastroCartoes.Size = new Size(942, 720);
            tabCadastroCartoes.TabIndex = 0;
            tabCadastroCartoes.Text = "Cartões cadastrados";
            // 
            // pnlCadastroCartao
            // 
            pnlCadastroCartao.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCadastroCartao.BackColor = Color.White;
            pnlCadastroCartao.Controls.Add(txtResultadoCartao);
            pnlCadastroCartao.Controls.Add(btnSalvarCartao);
            pnlCadastroCartao.Controls.Add(tlpDadosCartao);
            pnlCadastroCartao.Location = new Point(20, 20);
            pnlCadastroCartao.Name = "pnlCadastroCartao";
            pnlCadastroCartao.Padding = new Padding(20, 23, 20, 23);
            pnlCadastroCartao.Size = new Size(900, 374);
            pnlCadastroCartao.TabIndex = 0;
            // 
            txtResultadoCartao.Name = "txtResultadoCartao";
            txtResultadoCartao.Location = new Point(20, 305);
            txtResultadoCartao.Size = new Size(650, 60);
            txtResultadoCartao.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtResultadoCartao.Multiline = true;
            txtResultadoCartao.ReadOnly = true;
            txtResultadoCartao.BorderStyle = BorderStyle.None;
            txtResultadoCartao.BackColor = Color.White;
            txtResultadoCartao.ScrollBars = ScrollBars.Vertical;
            txtResultadoCartao.Text = "";
            txtResultadoCartao.TabStop = false;
            //
            // btnAtualizarCartoes
            //
            btnAtualizarCartoes.Name = "btnAtualizarCartoes";
            btnAtualizarCartoes.Location = new Point(794, 70);
            btnAtualizarCartoes.Size = new Size(180, 36);
            btnAtualizarCartoes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAtualizarCartoes.Text = "Atualizar dados";
            btnAtualizarCartoes.FlatStyle = FlatStyle.Flat;
            btnAtualizarCartoes.BackColor = Color.White;
            btnAtualizarCartoes.Cursor = Cursors.Hand;
            btnAtualizarCartoes.TabIndex = 5;
            //
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(tabCartoes);
            Controls.Add(btnAtualizarCartoes);
            Controls.Add(lblDescricaoCartao);
            Controls.Add(lblTituloCartoes);
            Font = new Font("Segoe UI", 10F);
            Name = "UcCartoes";
            Size = new Size(1000, 907);
            pnlListaCartoes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCartoes).EndInit();
            tlpDadosCartao.ResumeLayout(false);
            tlpDadosCartao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDiaVencimentoCartao).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLimiteCartao).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDiaFechamentoCartao).EndInit();
            tabCartoes.ResumeLayout(false);
            tabCadastroCartoes.ResumeLayout(false);
            pnlCadastroCartao.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblTituloCartoes;
        private Label lblDescricaoCartao;
        private Panel pnlListaCartoes;
        private DataGridView dgvCartoes;
        private TableLayoutPanel tlpDadosCartao;
        private Label lblRegraFechamentoCartao;
        private NumericUpDown numDiaFechamentoCartao;
        private Label lblDiaFechamentoCartao;
        private Label lblLimiteCartao;
        private Label lblNomeCartao;
        private Label lblContaCartao;
        private ComboBox cmbContaCartao;
        private TextBox txtNomeCartao;
        private NumericUpDown numLimiteCartao;
        private Label lblDiaVencimentoCartao;
        private NumericUpDown numDiaVencimentoCartao;
        private CheckBox chkCompraNaProximaFatura;
        private Button btnSalvarCartao;
        private TabControl tabCartoes;
        private TabPage tabCadastroCartoes;
        private Panel pnlCadastroCartao;
        private TextBox txtResultadoCartao;
        private Button btnAtualizarCartoes;
    }
}
