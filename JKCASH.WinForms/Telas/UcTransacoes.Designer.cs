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
            tabTransacoes = new TabControl();
            tabCadastroTransacao = new TabPage();
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
            tabExtratoTransacoes = new TabPage();
            pnlFiltrosExtrato = new Panel();
            dtpDataFinalExtrato = new DateTimePicker();
            dtpDataInicialExtrato = new DateTimePicker();
            lblDataFinalExtrato = new Label();
            lblDataInicialExtrato = new Label();
            chkTodasContasExtrato = new CheckBox();
            lblContasExtrato = new Label();
            clbContasExtrato = new CheckedListBox();
            btnConsultarExtrato = new Button();
            pnlResultadoExtrato = new Panel();
            splExtrato = new SplitContainer();
            dgvExtrato = new DataGridView();
            txtDetalhesExtrato = new TextBox();
            lblTituloDetalhesExtrato = new Label();
            lblTotaisExtrato = new Label();
            pnlAcoesExtrato = new FlowLayoutPanel();
            btnDetalhesExtrato = new Button();
            btnFecharDetalhesExtrato = new Button();
            lblStatusExtrato = new Label();
            tabTransacoes.SuspendLayout();
            tabCadastroTransacao.SuspendLayout();
            tlpDadosTransacao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValorTransacao).BeginInit();
            tabExtratoTransacoes.SuspendLayout();
            pnlFiltrosExtrato.SuspendLayout();
            pnlResultadoExtrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splExtrato).BeginInit();
            splExtrato.Panel1.SuspendLayout();
            splExtrato.Panel2.SuspendLayout();
            splExtrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvExtrato).BeginInit();
            pnlAcoesExtrato.SuspendLayout();
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
            lblTransacoes.Text = "Registre e acompanhe suas movimentações";
            // 
            // tabTransacoes
            // 
            tabTransacoes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabTransacoes.Controls.Add(tabCadastroTransacao);
            tabTransacoes.Controls.Add(tabExtratoTransacoes);
            tabTransacoes.Location = new Point(24, 120);
            tabTransacoes.Name = "tabTransacoes";
            tabTransacoes.Padding = new Point(10, 10);
            tabTransacoes.SelectedIndex = 0;
            tabTransacoes.Size = new Size(950, 750);
            tabTransacoes.TabIndex = 2;
            // 
            // tabCadastroTransacao
            // 
            tabCadastroTransacao.AutoScroll = true;
            tabCadastroTransacao.BackColor = Color.White;
            tabCadastroTransacao.Controls.Add(btnRegistrarTransacao);
            tabCadastroTransacao.Controls.Add(txtJustificativaTransacao);
            tabCadastroTransacao.Controls.Add(lblJustificativaTransacao);
            tabCadastroTransacao.Controls.Add(txtDescricaoTransacao);
            tabCadastroTransacao.Controls.Add(lblDescricaoTransacao);
            tabCadastroTransacao.Controls.Add(tlpDadosTransacao);
            tabCadastroTransacao.Location = new Point(4, 40);
            tabCadastroTransacao.Name = "tabCadastroTransacao";
            tabCadastroTransacao.Padding = new Padding(20);
            tabCadastroTransacao.Size = new Size(942, 706);
            tabCadastroTransacao.TabIndex = 0;
            tabCadastroTransacao.Text = "Nova transação";
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
            tlpDadosTransacao.Controls.Add(lblTipoTransacao, 1, 0);
            tlpDadosTransacao.Controls.Add(cmbContaTransacao, 0, 1);
            tlpDadosTransacao.Controls.Add(cmbTipoTransacao, 1, 1);
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
            lblTipoTransacao.Location = new Point(454, 0);
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
            cmbTipoTransacao.Dock = DockStyle.Top;
            cmbTipoTransacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoTransacao.FormattingEnabled = true;
            cmbTipoTransacao.Location = new Point(454, 27);
            cmbTipoTransacao.Name = "cmbTipoTransacao";
            cmbTipoTransacao.Size = new Size(445, 25);
            cmbTipoTransacao.TabIndex = 3;
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
            // tabExtratoTransacoes
            // 
            tabExtratoTransacoes.AutoScroll = true;
            tabExtratoTransacoes.BackColor = Color.White;
            tabExtratoTransacoes.Controls.Add(pnlFiltrosExtrato);
            tabExtratoTransacoes.Controls.Add(pnlResultadoExtrato);
            tabExtratoTransacoes.Location = new Point(4, 40);
            tabExtratoTransacoes.Name = "tabExtratoTransacoes";
            tabExtratoTransacoes.Padding = new Padding(3);
            tabExtratoTransacoes.Size = new Size(942, 706);
            tabExtratoTransacoes.TabIndex = 1;
            tabExtratoTransacoes.Text = "Extrato";
            // 
            // pnlFiltrosExtrato
            // 
            pnlFiltrosExtrato.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlFiltrosExtrato.BackColor = Color.FromArgb(244, 246, 248);
            pnlFiltrosExtrato.Controls.Add(dtpDataFinalExtrato);
            pnlFiltrosExtrato.Controls.Add(dtpDataInicialExtrato);
            pnlFiltrosExtrato.Controls.Add(lblDataFinalExtrato);
            pnlFiltrosExtrato.Controls.Add(lblDataInicialExtrato);
            pnlFiltrosExtrato.Controls.Add(chkTodasContasExtrato);
            pnlFiltrosExtrato.Controls.Add(lblContasExtrato);
            pnlFiltrosExtrato.Controls.Add(clbContasExtrato);
            pnlFiltrosExtrato.Controls.Add(btnConsultarExtrato);
            pnlFiltrosExtrato.Location = new Point(20, 20);
            pnlFiltrosExtrato.Name = "pnlFiltrosExtrato";
            pnlFiltrosExtrato.Size = new Size(900, 210);
            pnlFiltrosExtrato.TabIndex = 0;
            // 
            // dtpDataFinalExtrato
            // 
            dtpDataFinalExtrato.CustomFormat = "dd/MM/yyyy";
            dtpDataFinalExtrato.Format = DateTimePickerFormat.Custom;
            dtpDataFinalExtrato.Location = new Point(220, 44);
            dtpDataFinalExtrato.Name = "dtpDataFinalExtrato";
            dtpDataFinalExtrato.Size = new Size(180, 25);
            dtpDataFinalExtrato.TabIndex = 4;
            // 
            // dtpDataInicialExtrato
            // 
            dtpDataInicialExtrato.CustomFormat = "dd/MM/yyyy";
            dtpDataInicialExtrato.Format = DateTimePickerFormat.Custom;
            dtpDataInicialExtrato.Location = new Point(20, 44);
            dtpDataInicialExtrato.Name = "dtpDataInicialExtrato";
            dtpDataInicialExtrato.Size = new Size(180, 25);
            dtpDataInicialExtrato.TabIndex = 3;
            // 
            // lblDataFinalExtrato
            // 
            lblDataFinalExtrato.AutoSize = true;
            lblDataFinalExtrato.Location = new Point(220, 16);
            lblDataFinalExtrato.Name = "lblDataFinalExtrato";
            lblDataFinalExtrato.Size = new Size(67, 19);
            lblDataFinalExtrato.TabIndex = 2;
            lblDataFinalExtrato.Text = "Data final";
            // 
            // lblDataInicialExtrato
            // 
            lblDataInicialExtrato.AutoSize = true;
            lblDataInicialExtrato.Location = new Point(20, 16);
            lblDataInicialExtrato.Name = "lblDataInicialExtrato";
            lblDataInicialExtrato.Size = new Size(75, 19);
            lblDataInicialExtrato.TabIndex = 1;
            lblDataInicialExtrato.Text = "Data inicial";
            // 
            // chkTodasContasExtrato
            // 
            chkTodasContasExtrato.AutoSize = true;
            chkTodasContasExtrato.Checked = true;
            chkTodasContasExtrato.CheckState = CheckState.Checked;
            chkTodasContasExtrato.Location = new Point(20, 92);
            chkTodasContasExtrato.Name = "chkTodasContasExtrato";
            chkTodasContasExtrato.Size = new Size(124, 23);
            chkTodasContasExtrato.TabIndex = 5;
            chkTodasContasExtrato.Text = "Todas as contas";
            // 
            // lblContasExtrato
            // 
            lblContasExtrato.AutoSize = true;
            lblContasExtrato.Location = new Point(440, 16);
            lblContasExtrato.Name = "lblContasExtrato";
            lblContasExtrato.Size = new Size(256, 19);
            lblContasExtrato.TabIndex = 6;
            lblContasExtrato.Text = "Contas (desmarque Todas para escolher)";
            // 
            // clbContasExtrato
            // 
            clbContasExtrato.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clbContasExtrato.CheckOnClick = true;
            clbContasExtrato.Enabled = false;
            clbContasExtrato.IntegralHeight = false;
            clbContasExtrato.Location = new Point(440, 44);
            clbContasExtrato.Name = "clbContasExtrato";
            clbContasExtrato.Size = new Size(440, 145);
            clbContasExtrato.TabIndex = 6;
            // 
            // btnConsultarExtrato
            // 
            btnConsultarExtrato.BackColor = Color.FromArgb(22, 134, 92);
            btnConsultarExtrato.Cursor = Cursors.Hand;
            btnConsultarExtrato.FlatAppearance.BorderSize = 0;
            btnConsultarExtrato.FlatStyle = FlatStyle.Flat;
            btnConsultarExtrato.ForeColor = Color.White;
            btnConsultarExtrato.Location = new Point(20, 149);
            btnConsultarExtrato.Name = "btnConsultarExtrato";
            btnConsultarExtrato.Size = new Size(180, 40);
            btnConsultarExtrato.TabIndex = 7;
            btnConsultarExtrato.Text = "Consultar extrato";
            btnConsultarExtrato.UseVisualStyleBackColor = false;
            // 
            // pnlResultadoExtrato
            // 
            pnlResultadoExtrato.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlResultadoExtrato.BackColor = Color.White;
            pnlResultadoExtrato.Controls.Add(splExtrato);
            pnlResultadoExtrato.Controls.Add(lblTotaisExtrato);
            pnlResultadoExtrato.Controls.Add(pnlAcoesExtrato);
            pnlResultadoExtrato.Controls.Add(lblStatusExtrato);
            pnlResultadoExtrato.Location = new Point(20, 246);
            pnlResultadoExtrato.Name = "pnlResultadoExtrato";
            pnlResultadoExtrato.Size = new Size(900, 434);
            pnlResultadoExtrato.TabIndex = 1;
            // 
            // splExtrato
            // 
            splExtrato.Dock = DockStyle.Fill;
            splExtrato.Location = new Point(0, 73);
            splExtrato.Name = "splExtrato";
            // 
            // splExtrato.Panel1
            // 
            splExtrato.Panel1.Controls.Add(dgvExtrato);
            splExtrato.Panel1MinSize = 320;
            // 
            // splExtrato.Panel2
            // 
            splExtrato.Panel2.Controls.Add(txtDetalhesExtrato);
            splExtrato.Panel2.Controls.Add(lblTituloDetalhesExtrato);
            splExtrato.Panel2.Padding = new Padding(12, 0, 0, 0);
            splExtrato.Panel2Collapsed = true;
            splExtrato.Panel2MinSize = 240;
            splExtrato.Size = new Size(900, 289);
            splExtrato.SplitterDistance = 320;
            splExtrato.TabIndex = 0;
            // 
            // dgvExtrato
            // 
            dgvExtrato.AllowUserToAddRows = false;
            dgvExtrato.AllowUserToDeleteRows = false;
            dgvExtrato.AllowUserToResizeRows = false;
            dgvExtrato.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvExtrato.BackgroundColor = Color.White;
            dgvExtrato.BorderStyle = BorderStyle.None;
            dgvExtrato.Dock = DockStyle.Fill;
            dgvExtrato.Location = new Point(0, 0);
            dgvExtrato.MultiSelect = false;
            dgvExtrato.Name = "dgvExtrato";
            dgvExtrato.ReadOnly = true;
            dgvExtrato.RowHeadersVisible = false;
            dgvExtrato.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExtrato.Size = new Size(900, 289);
            dgvExtrato.TabIndex = 0;
            // 
            // txtDetalhesExtrato
            // 
            txtDetalhesExtrato.BackColor = Color.White;
            txtDetalhesExtrato.BorderStyle = BorderStyle.None;
            txtDetalhesExtrato.Dock = DockStyle.Fill;
            txtDetalhesExtrato.ForeColor = Color.FromArgb(22, 36, 55);
            txtDetalhesExtrato.Location = new Point(12, 30);
            txtDetalhesExtrato.Multiline = true;
            txtDetalhesExtrato.Name = "txtDetalhesExtrato";
            txtDetalhesExtrato.ReadOnly = true;
            txtDetalhesExtrato.ScrollBars = ScrollBars.Vertical;
            txtDetalhesExtrato.Size = new Size(84, 70);
            txtDetalhesExtrato.TabIndex = 0;
            // 
            // lblTituloDetalhesExtrato
            // 
            lblTituloDetalhesExtrato.Dock = DockStyle.Top;
            lblTituloDetalhesExtrato.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTituloDetalhesExtrato.Location = new Point(12, 0);
            lblTituloDetalhesExtrato.Name = "lblTituloDetalhesExtrato";
            lblTituloDetalhesExtrato.Size = new Size(84, 30);
            lblTituloDetalhesExtrato.TabIndex = 1;
            lblTituloDetalhesExtrato.Text = "Detalhes da transação";
            // 
            // lblTotaisExtrato
            // 
            lblTotaisExtrato.Dock = DockStyle.Bottom;
            lblTotaisExtrato.ForeColor = Color.FromArgb(22, 36, 55);
            lblTotaisExtrato.Location = new Point(0, 362);
            lblTotaisExtrato.Name = "lblTotaisExtrato";
            lblTotaisExtrato.Padding = new Padding(0, 8, 0, 0);
            lblTotaisExtrato.Size = new Size(900, 72);
            lblTotaisExtrato.TabIndex = 1;
            // 
            // pnlAcoesExtrato
            // 
            pnlAcoesExtrato.Controls.Add(btnDetalhesExtrato);
            pnlAcoesExtrato.Controls.Add(btnFecharDetalhesExtrato);
            pnlAcoesExtrato.Dock = DockStyle.Top;
            pnlAcoesExtrato.Location = new Point(0, 30);
            pnlAcoesExtrato.Name = "pnlAcoesExtrato";
            pnlAcoesExtrato.Size = new Size(900, 43);
            pnlAcoesExtrato.TabIndex = 2;
            // 
            // btnDetalhesExtrato
            // 
            btnDetalhesExtrato.BackColor = Color.White;
            btnDetalhesExtrato.Cursor = Cursors.Hand;
            btnDetalhesExtrato.Enabled = false;
            btnDetalhesExtrato.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            btnDetalhesExtrato.FlatStyle = FlatStyle.Flat;
            btnDetalhesExtrato.Location = new Point(3, 3);
            btnDetalhesExtrato.Name = "btnDetalhesExtrato";
            btnDetalhesExtrato.Size = new Size(150, 34);
            btnDetalhesExtrato.TabIndex = 0;
            btnDetalhesExtrato.Text = "Ver detalhes";
            btnDetalhesExtrato.UseVisualStyleBackColor = false;
            // 
            // btnFecharDetalhesExtrato
            // 
            btnFecharDetalhesExtrato.BackColor = Color.White;
            btnFecharDetalhesExtrato.Cursor = Cursors.Hand;
            btnFecharDetalhesExtrato.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            btnFecharDetalhesExtrato.FlatStyle = FlatStyle.Flat;
            btnFecharDetalhesExtrato.Location = new Point(159, 3);
            btnFecharDetalhesExtrato.Name = "btnFecharDetalhesExtrato";
            btnFecharDetalhesExtrato.Size = new Size(150, 34);
            btnFecharDetalhesExtrato.TabIndex = 1;
            btnFecharDetalhesExtrato.Text = "Fechar detalhes";
            btnFecharDetalhesExtrato.UseVisualStyleBackColor = false;
            btnFecharDetalhesExtrato.Visible = false;
            // 
            // lblStatusExtrato
            // 
            lblStatusExtrato.Dock = DockStyle.Top;
            lblStatusExtrato.ForeColor = Color.FromArgb(105, 122, 143);
            lblStatusExtrato.Location = new Point(0, 0);
            lblStatusExtrato.Name = "lblStatusExtrato";
            lblStatusExtrato.Size = new Size(900, 30);
            lblStatusExtrato.TabIndex = 3;
            lblStatusExtrato.Text = "Selecione o período e consulte o extrato.";
            lblStatusExtrato.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // UcTransacoes
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(tabTransacoes);
            Controls.Add(lblTransacoes);
            Controls.Add(lblTituloTransacoes);
            Font = new Font("Segoe UI", 10F);
            Name = "UcTransacoes";
            Size = new Size(1000, 900);
            tabTransacoes.ResumeLayout(false);
            tabCadastroTransacao.ResumeLayout(false);
            tabCadastroTransacao.PerformLayout();
            tlpDadosTransacao.ResumeLayout(false);
            tlpDadosTransacao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValorTransacao).EndInit();
            tabExtratoTransacoes.ResumeLayout(false);
            pnlFiltrosExtrato.ResumeLayout(false);
            pnlFiltrosExtrato.PerformLayout();
            pnlResultadoExtrato.ResumeLayout(false);
            splExtrato.Panel1.ResumeLayout(false);
            splExtrato.Panel2.ResumeLayout(false);
            splExtrato.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splExtrato).EndInit();
            splExtrato.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvExtrato).EndInit();
            pnlAcoesExtrato.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTituloTransacoes;
        private Label lblTransacoes;
        private TabControl tabTransacoes;
        private TabPage tabCadastroTransacao;
        private TabPage tabExtratoTransacoes;
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
        private Panel pnlFiltrosExtrato;
        private DateTimePicker dtpDataFinalExtrato;
        private DateTimePicker dtpDataInicialExtrato;
        private Label lblDataFinalExtrato;
        private Label lblDataInicialExtrato;
        private CheckBox chkTodasContasExtrato;
        private Label lblContasExtrato;
        private CheckedListBox clbContasExtrato;
        private Button btnConsultarExtrato;
        private Panel pnlResultadoExtrato;
        private Label lblStatusExtrato;
        private FlowLayoutPanel pnlAcoesExtrato;
        private Button btnDetalhesExtrato;
        private Button btnFecharDetalhesExtrato;
        private Label lblTotaisExtrato;
        private SplitContainer splExtrato;
        private DataGridView dgvExtrato;
        private Label lblTituloDetalhesExtrato;
        private TextBox txtDetalhesExtrato;
    }
}
