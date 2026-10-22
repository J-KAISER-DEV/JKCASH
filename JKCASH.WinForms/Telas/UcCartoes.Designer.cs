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
            tabCompraCartao = new TabPage();
            tabCadastroCartoes = new TabPage();
            pnlCadastroCartao = new Panel();
            tlpDadosCompra = new TableLayoutPanel();
            lblCartaoCompra = new Label();
            lblValorCompra = new Label();
            cmbCartaoCompra = new ComboBox();
            numValorCompra = new NumericUpDown();
            lblDataCompra = new Label();
            lblParcelasCompra = new Label();
            dtpDataCompra = new DateTimePicker();
            numParcelasCompra = new NumericUpDown();
            lblCategoriaCompra = new Label();
            lblImportanciaCompra = new Label();
            cmbCategoriaCompra = new ComboBox();
            cmbImportanciaCompra = new ComboBox();
            lblPlanejamentoCompra = new Label();
            lblUrgenciaCompra = new Label();
            cmbPlanejamentoCompra = new ComboBox();
            cmbUrgenciaCompra = new ComboBox();
            lblLimiteCompra = new Label();
            lblDescricaoCompra = new Label();
            txtDescricaoCompra = new TextBox();
            lblJustificativaCompra = new Label();
            txtJustificativaCompra = new TextBox();
            btnRegistrarCompra = new Button();
            txtResultadoCompra = new TextBox();
            btnAtualizarCartoes = new Button();
            tabHistoricoCompras = new TabPage();
            tlpHistoricoCompras = new TableLayoutPanel();
            cmbCartaoHistorico = new ComboBox();
            dgvComprasCartao = new DataGridView();
            lblDetalhesCompra = new Label();
            txtDetalhesCompra = new TextBox();
            pnlListaCartoes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCartoes).BeginInit();
            tlpDadosCartao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDiaVencimentoCartao).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLimiteCartao).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDiaFechamentoCartao).BeginInit();
            tabCartoes.SuspendLayout();
            tabCompraCartao.SuspendLayout();
            tabCadastroCartoes.SuspendLayout();
            pnlCadastroCartao.SuspendLayout();
            tlpDadosCompra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).BeginInit();
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
            lblDescricaoCartao.Text = "Gerencie seus cartões, limites e compras.";
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
            tabCartoes.Controls.Add(tabCompraCartao);
            tabCartoes.Controls.Add(tabHistoricoCompras);
            tabCartoes.Location = new Point(24, 120);
            tabCartoes.Name = "tabCartoes";
            tabCartoes.SelectedIndex = 0;
            tabCartoes.Size = new Size(950, 750);
            tabCartoes.TabIndex = 4;
            // 
            // tabCompraCartao
            // 
            tabCompraCartao.AutoScroll = true;
            tabCompraCartao.BackColor = Color.White;
            tabCompraCartao.Controls.Add(tlpDadosCompra);
            tabCompraCartao.Controls.Add(lblLimiteCompra);
            tabCompraCartao.Controls.Add(lblDescricaoCompra);
            tabCompraCartao.Controls.Add(txtDescricaoCompra);
            tabCompraCartao.Controls.Add(lblJustificativaCompra);
            tabCompraCartao.Controls.Add(txtJustificativaCompra);
            tabCompraCartao.Controls.Add(btnRegistrarCompra);
            tabCompraCartao.Controls.Add(txtResultadoCompra);
            tabCompraCartao.AutoScrollMinSize = new Size(0, 710);
            tabCompraCartao.Location = new Point(4, 26);
            tabCompraCartao.Name = "tabCompraCartao";
            tabCompraCartao.Padding = new Padding(20);
            tabCompraCartao.Size = new Size(942, 720);
            tabCompraCartao.TabIndex = 1;
            tabCompraCartao.Text = "Registrar compra";
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
            pnlCadastroCartao.Controls.Add(btnSalvarCartao);
            pnlCadastroCartao.Controls.Add(tlpDadosCartao);
            pnlCadastroCartao.Location = new Point(20, 20);
            pnlCadastroCartao.Name = "pnlCadastroCartao";
            pnlCadastroCartao.Padding = new Padding(20, 23, 20, 23);
            pnlCadastroCartao.Size = new Size(900, 374);
            pnlCadastroCartao.TabIndex = 0;
            // 
            // tlpDadosCompra
            // 
            tlpDadosCompra.ColumnCount = 2;
            tlpDadosCompra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosCompra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDadosCompra.Controls.Add(lblCartaoCompra, 0, 0);
            tlpDadosCompra.Controls.Add(lblValorCompra, 1, 0);
            tlpDadosCompra.Controls.Add(cmbCartaoCompra, 0, 1);
            tlpDadosCompra.Controls.Add(numValorCompra, 1, 1);
            tlpDadosCompra.Controls.Add(lblDataCompra, 0, 2);
            tlpDadosCompra.Controls.Add(lblParcelasCompra, 1, 2);
            tlpDadosCompra.Controls.Add(dtpDataCompra, 0, 3);
            tlpDadosCompra.Controls.Add(numParcelasCompra, 1, 3);
            tlpDadosCompra.Controls.Add(lblCategoriaCompra, 0, 4);
            tlpDadosCompra.Controls.Add(lblImportanciaCompra, 1, 4);
            tlpDadosCompra.Controls.Add(cmbCategoriaCompra, 0, 5);
            tlpDadosCompra.Controls.Add(cmbImportanciaCompra, 1, 5);
            tlpDadosCompra.Controls.Add(lblPlanejamentoCompra, 0, 6);
            tlpDadosCompra.Controls.Add(lblUrgenciaCompra, 1, 6);
            tlpDadosCompra.Controls.Add(cmbPlanejamentoCompra, 0, 7);
            tlpDadosCompra.Controls.Add(cmbUrgenciaCompra, 1, 7);
            tlpDadosCompra.Dock = DockStyle.Top;
            tlpDadosCompra.Location = new Point(20, 20);
            tlpDadosCompra.Name = "tlpDadosCompra";
            tlpDadosCompra.RowCount = 8;
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDadosCompra.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpDadosCompra.Size = new Size(902, 288);
            tlpDadosCompra.TabIndex = 0;
            // 
            // lblCartaoCompra
            // 
            lblCartaoCompra.AutoSize = true;
            lblCartaoCompra.Location = new Point(3, 0);
            lblCartaoCompra.Name = "lblCartaoCompra";
            lblCartaoCompra.Size = new Size(104, 19);
            lblCartaoCompra.TabIndex = 0;
            lblCartaoCompra.Text = "Cartão utilizado";
            // 
            // lblValorCompra
            // 
            lblValorCompra.AutoSize = true;
            lblValorCompra.Location = new Point(454, 0);
            lblValorCompra.Name = "lblValorCompra";
            lblValorCompra.Size = new Size(142, 19);
            lblValorCompra.TabIndex = 1;
            lblValorCompra.Text = "Valor total da compra";
            // 
            // cmbCartaoCompra
            // 
            cmbCartaoCompra.Dock = DockStyle.Top;
            cmbCartaoCompra.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCartaoCompra.FormattingEnabled = true;
            cmbCartaoCompra.Location = new Point(3, 27);
            cmbCartaoCompra.Name = "cmbCartaoCompra";
            cmbCartaoCompra.Size = new Size(445, 25);
            cmbCartaoCompra.TabIndex = 2;
            // 
            // numValorCompra
            // 
            numValorCompra.DecimalPlaces = 2;
            numValorCompra.Dock = DockStyle.Top;
            numValorCompra.Location = new Point(454, 27);
            numValorCompra.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            numValorCompra.Name = "numValorCompra";
            numValorCompra.Size = new Size(445, 25);
            numValorCompra.TabIndex = 3;
            numValorCompra.ThousandsSeparator = true;
            // 
            //
            // lblDataCompra
            //
            lblDataCompra.Name = "lblDataCompra";
            lblDataCompra.AutoSize = true;
            lblDataCompra.Text = "Data da compra";
            lblDataCompra.Location = new Point(3, 72);
            lblDataCompra.TabIndex = 4;
            //
            // lblParcelasCompra
            //
            lblParcelasCompra.Name = "lblParcelasCompra";
            lblParcelasCompra.AutoSize = true;
            lblParcelasCompra.Text = "Quantidade de parcelas (1 = à vista)";
            lblParcelasCompra.Location = new Point(454, 72);
            lblParcelasCompra.TabIndex = 5;
            //
            // dtpDataCompra
            //
            dtpDataCompra.Name = "dtpDataCompra";
            dtpDataCompra.Dock = DockStyle.Top;
            dtpDataCompra.Format = DateTimePickerFormat.Custom;
            dtpDataCompra.CustomFormat = "dd/MM/yyyy";
            dtpDataCompra.Location = new Point(3, 99);
            dtpDataCompra.Size = new Size(445, 25);
            dtpDataCompra.TabIndex = 6;
            //
            // numParcelasCompra
            //
            numParcelasCompra.Name = "numParcelasCompra";
            numParcelasCompra.Dock = DockStyle.Top;
            numParcelasCompra.Minimum = 1m;
            numParcelasCompra.Maximum = 120m;
            numParcelasCompra.Value = 1m;
            numParcelasCompra.Location = new Point(454, 99);
            numParcelasCompra.Size = new Size(445, 25);
            numParcelasCompra.TabIndex = 7;
            //
            // lblCategoriaCompra
            //
            lblCategoriaCompra.Name = "lblCategoriaCompra";
            lblCategoriaCompra.AutoSize = true;
            lblCategoriaCompra.Text = "Categoria";
            lblCategoriaCompra.Location = new Point(3, 144);
            lblCategoriaCompra.TabIndex = 8;
            //
            // lblImportanciaCompra
            //
            lblImportanciaCompra.Name = "lblImportanciaCompra";
            lblImportanciaCompra.AutoSize = true;
            lblImportanciaCompra.Text = "Importância";
            lblImportanciaCompra.Location = new Point(454, 144);
            lblImportanciaCompra.TabIndex = 9;
            //
            // cmbCategoriaCompra
            //
            cmbCategoriaCompra.Name = "cmbCategoriaCompra";
            cmbCategoriaCompra.Dock = DockStyle.Top;
            cmbCategoriaCompra.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaCompra.Location = new Point(3, 171);
            cmbCategoriaCompra.Size = new Size(445, 25);
            cmbCategoriaCompra.TabIndex = 10;
            //
            // cmbImportanciaCompra
            //
            cmbImportanciaCompra.Name = "cmbImportanciaCompra";
            cmbImportanciaCompra.Dock = DockStyle.Top;
            cmbImportanciaCompra.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImportanciaCompra.Location = new Point(454, 171);
            cmbImportanciaCompra.Size = new Size(445, 25);
            cmbImportanciaCompra.TabIndex = 11;
            //
            // lblPlanejamentoCompra
            //
            lblPlanejamentoCompra.Name = "lblPlanejamentoCompra";
            lblPlanejamentoCompra.AutoSize = true;
            lblPlanejamentoCompra.Text = "Planejamento";
            lblPlanejamentoCompra.Location = new Point(3, 216);
            lblPlanejamentoCompra.TabIndex = 12;
            //
            // lblUrgenciaCompra
            //
            lblUrgenciaCompra.Name = "lblUrgenciaCompra";
            lblUrgenciaCompra.AutoSize = true;
            lblUrgenciaCompra.Text = "Urgência";
            lblUrgenciaCompra.Location = new Point(454, 216);
            lblUrgenciaCompra.TabIndex = 13;
            //
            // cmbPlanejamentoCompra
            //
            cmbPlanejamentoCompra.Name = "cmbPlanejamentoCompra";
            cmbPlanejamentoCompra.Dock = DockStyle.Top;
            cmbPlanejamentoCompra.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlanejamentoCompra.Location = new Point(3, 243);
            cmbPlanejamentoCompra.Size = new Size(445, 25);
            cmbPlanejamentoCompra.TabIndex = 14;
            //
            // cmbUrgenciaCompra
            //
            cmbUrgenciaCompra.Name = "cmbUrgenciaCompra";
            cmbUrgenciaCompra.Dock = DockStyle.Top;
            cmbUrgenciaCompra.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUrgenciaCompra.Location = new Point(454, 243);
            cmbUrgenciaCompra.Size = new Size(445, 25);
            cmbUrgenciaCompra.TabIndex = 15;
            //
            // lblLimiteCompra
            //
            lblLimiteCompra.Name = "lblLimiteCompra";
            lblLimiteCompra.Location = new Point(20, 312);
            lblLimiteCompra.Size = new Size(902, 26);
            lblLimiteCompra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblLimiteCompra.Text = "Selecione um cartão para consultar o limite disponível.";
            lblLimiteCompra.ForeColor = Color.FromArgb(105, 122, 143);
            lblLimiteCompra.AutoEllipsis = true;
            //
            // lblDescricaoCompra
            //
            lblDescricaoCompra.Name = "lblDescricaoCompra";
            lblDescricaoCompra.AutoSize = true;
            lblDescricaoCompra.Location = new Point(20, 346);
            lblDescricaoCompra.Text = "Descrição";
            //
            // txtDescricaoCompra
            //
            txtDescricaoCompra.Name = "txtDescricaoCompra";
            txtDescricaoCompra.Location = new Point(20, 370);
            txtDescricaoCompra.Size = new Size(902, 25);
            txtDescricaoCompra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescricaoCompra.MaxLength = 200;
            txtDescricaoCompra.TabIndex = 16;
            //
            // lblJustificativaCompra
            //
            lblJustificativaCompra.Name = "lblJustificativaCompra";
            lblJustificativaCompra.AutoSize = true;
            lblJustificativaCompra.Location = new Point(20, 408);
            lblJustificativaCompra.Text = "Justificativa";
            //
            // txtJustificativaCompra
            //
            txtJustificativaCompra.Name = "txtJustificativaCompra";
            txtJustificativaCompra.Location = new Point(20, 432);
            txtJustificativaCompra.Size = new Size(902, 84);
            txtJustificativaCompra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJustificativaCompra.Multiline = true;
            txtJustificativaCompra.MaxLength = 500;
            txtJustificativaCompra.ScrollBars = ScrollBars.Vertical;
            txtJustificativaCompra.TabIndex = 17;
            //
            // btnRegistrarCompra
            //
            btnRegistrarCompra.Name = "btnRegistrarCompra";
            btnRegistrarCompra.Location = new Point(722, 536);
            btnRegistrarCompra.Size = new Size(200, 40);
            btnRegistrarCompra.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRegistrarCompra.Text = "Registrar compra";
            btnRegistrarCompra.BackColor = Color.FromArgb(22, 134, 92);
            btnRegistrarCompra.ForeColor = Color.White;
            btnRegistrarCompra.FlatStyle = FlatStyle.Flat;
            btnRegistrarCompra.FlatAppearance.BorderSize = 0;
            btnRegistrarCompra.UseVisualStyleBackColor = false;
            btnRegistrarCompra.Enabled = false;
            btnRegistrarCompra.Cursor = Cursors.Hand;
            btnRegistrarCompra.TabIndex = 18;
            //
            // txtResultadoCompra
            //
            txtResultadoCompra.Name = "txtResultadoCompra";
            txtResultadoCompra.Location = new Point(20, 590);
            txtResultadoCompra.Size = new Size(902, 100);
            txtResultadoCompra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtResultadoCompra.Multiline = true;
            txtResultadoCompra.ReadOnly = true;
            txtResultadoCompra.BorderStyle = BorderStyle.None;
            txtResultadoCompra.BackColor = Color.White;
            txtResultadoCompra.ScrollBars = ScrollBars.Vertical;
            txtResultadoCompra.Text = "O valor total da compra compromete o limite. O pagamento da fatura movimenta o saldo da conta.";
            txtResultadoCompra.TabStop = false;
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
            // tabHistoricoCompras
            //
            tabHistoricoCompras.Name = "tabHistoricoCompras";
            tabHistoricoCompras.Text = "Compras do cartão";
            tabHistoricoCompras.BackColor = Color.White;
            tabHistoricoCompras.Padding = new Padding(20);
            tabHistoricoCompras.Size = new Size(942, 720);
            //
            // tlpHistoricoCompras
            //
            tlpHistoricoCompras.Name = "tlpHistoricoCompras";
            tlpHistoricoCompras.Dock = DockStyle.Fill;
            tlpHistoricoCompras.ColumnCount = 1;
            tlpHistoricoCompras.RowCount = 4;
            tlpHistoricoCompras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpHistoricoCompras.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tlpHistoricoCompras.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tlpHistoricoCompras.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpHistoricoCompras.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            //
            // cmbCartaoHistorico
            //
            cmbCartaoHistorico.Name = "cmbCartaoHistorico";
            cmbCartaoHistorico.Dock = DockStyle.Top;
            cmbCartaoHistorico.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCartaoHistorico.AccessibleName = "Cartão do histórico de compras";
            //
            // dgvComprasCartao
            //
            dgvComprasCartao.Name = "dgvComprasCartao";
            dgvComprasCartao.Dock = DockStyle.Fill;
            //
            // lblDetalhesCompra
            //
            lblDetalhesCompra.Name = "lblDetalhesCompra";
            lblDetalhesCompra.Dock = DockStyle.Fill;
            lblDetalhesCompra.Text = "Detalhes e parcelas da compra selecionada";
            lblDetalhesCompra.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txtDetalhesCompra
            //
            txtDetalhesCompra.Name = "txtDetalhesCompra";
            txtDetalhesCompra.Dock = DockStyle.Fill;
            txtDetalhesCompra.Multiline = true;
            txtDetalhesCompra.ReadOnly = true;
            txtDetalhesCompra.BackColor = Color.White;
            txtDetalhesCompra.BorderStyle = BorderStyle.None;
            txtDetalhesCompra.ScrollBars = ScrollBars.Vertical;
            tabHistoricoCompras.Controls.Add(tlpHistoricoCompras);
            tlpHistoricoCompras.Controls.Add(cmbCartaoHistorico, 0, 0);
            tlpHistoricoCompras.Controls.Add(dgvComprasCartao, 0, 1);
            tlpHistoricoCompras.Controls.Add(lblDetalhesCompra, 0, 2);
            tlpHistoricoCompras.Controls.Add(txtDetalhesCompra, 0, 3);
            //
            // UcCartoes
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
            tlpDadosCompra.ResumeLayout(false);
            tlpDadosCompra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).EndInit();
            tabCompraCartao.ResumeLayout(false);
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
        private TabPage tabCompraCartao;
        private TableLayoutPanel tlpDadosCompra;
        private Label lblCartaoCompra;
        private Label lblValorCompra;
        private ComboBox cmbCartaoCompra;
        private NumericUpDown numValorCompra;
        private Label lblDataCompra;
        private Label lblParcelasCompra;
        private DateTimePicker dtpDataCompra;
        private NumericUpDown numParcelasCompra;
        private Label lblCategoriaCompra;
        private Label lblImportanciaCompra;
        private ComboBox cmbCategoriaCompra;
        private ComboBox cmbImportanciaCompra;
        private Label lblPlanejamentoCompra;
        private Label lblUrgenciaCompra;
        private ComboBox cmbPlanejamentoCompra;
        private ComboBox cmbUrgenciaCompra;
        private Label lblLimiteCompra;
        private Label lblDescricaoCompra;
        private TextBox txtDescricaoCompra;
        private Label lblJustificativaCompra;
        private TextBox txtJustificativaCompra;
        private Button btnRegistrarCompra;
        private TextBox txtResultadoCompra;
        private Button btnAtualizarCartoes;
        private TabPage tabHistoricoCompras;
        private TableLayoutPanel tlpHistoricoCompras;
        private ComboBox cmbCartaoHistorico;
        private DataGridView dgvComprasCartao;
        private Label lblDetalhesCompra;
        private TextBox txtDetalhesCompra;
    }
}
