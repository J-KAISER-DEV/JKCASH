namespace JKCASH.WinForms.Telas
{
    partial class UcExtrato
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
            lblTitulo = new Label();
            lblDescricao = new Label();
            pnlFiltrosExtrato = new Panel();
            dtpDataFinalExtrato = new DateTimePicker();
            dtpDataInicialExtrato = new DateTimePicker();
            lblDataFinalExtrato = new Label();
            lblDataInicialExtrato = new Label();
            chkTodasContasExtrato = new CheckBox();
            lblContasExtrato = new Label();
            clbContasExtrato = new CheckedListBox();
            btnConsultarExtrato = new Button();
            tabExtrato = new TabControl();
            tabMovimentacoes = new TabPage();
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
            tabRelatorioGastos = new TabPage();
            dgvGastosPorCategoria = new DataGridView();
            pnlFiltrosExtrato.SuspendLayout();
            tabExtrato.SuspendLayout();
            tabMovimentacoes.SuspendLayout();
            pnlResultadoExtrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splExtrato).BeginInit();
            splExtrato.Panel1.SuspendLayout();
            splExtrato.Panel2.SuspendLayout();
            splExtrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvExtrato).BeginInit();
            pnlAcoesExtrato.SuspendLayout();
            tabRelatorioGastos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGastosPorCategoria).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(22, 36, 55);
            lblTitulo.Location = new Point(24, 20);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(140, 47);
            lblTitulo.TabIndex = 3;
            lblTitulo.Text = "Extrato";
            // 
            // lblDescricao
            // 
            lblDescricao.AutoSize = true;
            lblDescricao.ForeColor = Color.FromArgb(105, 122, 143);
            lblDescricao.Location = new Point(27, 78);
            lblDescricao.Name = "lblDescricao";
            lblDescricao.Size = new Size(359, 19);
            lblDescricao.TabIndex = 2;
            lblDescricao.Text = "Consulte movimentações de contas e compras no cartão por período.";
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
            pnlFiltrosExtrato.Location = new Point(24, 120);
            pnlFiltrosExtrato.Name = "pnlFiltrosExtrato";
            pnlFiltrosExtrato.Size = new Size(950, 210);
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
            // tabExtrato
            // 
            tabExtrato.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabExtrato.Controls.Add(tabMovimentacoes);
            tabExtrato.Controls.Add(tabRelatorioGastos);
            tabExtrato.Location = new Point(24, 346);
            tabExtrato.Name = "tabExtrato";
            tabExtrato.SelectedIndex = 0;
            tabExtrato.Size = new Size(950, 524);
            tabExtrato.TabIndex = 4;
            // 
            // tabMovimentacoes
            // 
            tabMovimentacoes.Controls.Add(pnlResultadoExtrato);
            tabMovimentacoes.Location = new Point(4, 26);
            tabMovimentacoes.Name = "tabMovimentacoes";
            tabMovimentacoes.Padding = new Padding(3);
            tabMovimentacoes.Size = new Size(942, 494);
            tabMovimentacoes.TabIndex = 0;
            tabMovimentacoes.Text = "Movimentações";
            tabMovimentacoes.UseVisualStyleBackColor = true;
            // 
            // pnlResultadoExtrato
            // 
            pnlResultadoExtrato.BackColor = Color.White;
            pnlResultadoExtrato.Controls.Add(splExtrato);
            pnlResultadoExtrato.Controls.Add(lblTotaisExtrato);
            pnlResultadoExtrato.Controls.Add(pnlAcoesExtrato);
            pnlResultadoExtrato.Controls.Add(lblStatusExtrato);
            pnlResultadoExtrato.Dock = DockStyle.Fill;
            pnlResultadoExtrato.Location = new Point(3, 3);
            pnlResultadoExtrato.Name = "pnlResultadoExtrato";
            pnlResultadoExtrato.Size = new Size(936, 488);
            pnlResultadoExtrato.TabIndex = 2;
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
            splExtrato.Size = new Size(936, 343);
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
            dgvExtrato.Size = new Size(936, 343);
            dgvExtrato.TabIndex = 0;
            // 
            // txtDetalhesExtrato
            // 
            txtDetalhesExtrato.BackColor = Color.White;
            txtDetalhesExtrato.BorderStyle = BorderStyle.None;
            txtDetalhesExtrato.Dock = DockStyle.Fill;
            txtDetalhesExtrato.ForeColor = Color.FromArgb(22, 36, 55);
            txtDetalhesExtrato.Location = new Point(12, 0);
            txtDetalhesExtrato.Multiline = true;
            txtDetalhesExtrato.Name = "txtDetalhesExtrato";
            txtDetalhesExtrato.ReadOnly = true;
            txtDetalhesExtrato.ScrollBars = ScrollBars.Vertical;
            txtDetalhesExtrato.Size = new Size(84, 100);
            txtDetalhesExtrato.TabIndex = 0;
            // 
            // lblTituloDetalhesExtrato
            // 
            lblTituloDetalhesExtrato.Dock = DockStyle.Fill;
            lblTituloDetalhesExtrato.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTituloDetalhesExtrato.Location = new Point(12, 0);
            lblTituloDetalhesExtrato.Name = "lblTituloDetalhesExtrato";
            lblTituloDetalhesExtrato.Size = new Size(84, 100);
            lblTituloDetalhesExtrato.TabIndex = 1;
            lblTituloDetalhesExtrato.Text = "Detalhes da transação";
            // 
            // lblTotaisExtrato
            // 
            lblTotaisExtrato.Dock = DockStyle.Bottom;
            lblTotaisExtrato.ForeColor = Color.FromArgb(22, 36, 55);
            lblTotaisExtrato.Location = new Point(0, 416);
            lblTotaisExtrato.Name = "lblTotaisExtrato";
            lblTotaisExtrato.Padding = new Padding(0, 8, 0, 0);
            lblTotaisExtrato.Size = new Size(936, 72);
            lblTotaisExtrato.TabIndex = 1;
            // 
            // pnlAcoesExtrato
            // 
            pnlAcoesExtrato.Controls.Add(btnDetalhesExtrato);
            pnlAcoesExtrato.Controls.Add(btnFecharDetalhesExtrato);
            pnlAcoesExtrato.Dock = DockStyle.Top;
            pnlAcoesExtrato.Location = new Point(0, 30);
            pnlAcoesExtrato.Name = "pnlAcoesExtrato";
            pnlAcoesExtrato.Size = new Size(936, 43);
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
            lblStatusExtrato.Size = new Size(936, 30);
            lblStatusExtrato.TabIndex = 3;
            lblStatusExtrato.Text = "Selecione o período e consulte o extrato.";
            lblStatusExtrato.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabRelatorioGastos
            // 
            tabRelatorioGastos.Controls.Add(dgvGastosPorCategoria);
            tabRelatorioGastos.Location = new Point(4, 26);
            tabRelatorioGastos.Name = "tabRelatorioGastos";
            tabRelatorioGastos.Padding = new Padding(3);
            tabRelatorioGastos.Size = new Size(942, 494);
            tabRelatorioGastos.TabIndex = 1;
            tabRelatorioGastos.Text = "Relatório de gastos";
            tabRelatorioGastos.UseVisualStyleBackColor = true;
            // 
            // dgvGastosPorCategoria
            // 
            dgvGastosPorCategoria.AllowUserToAddRows = false;
            dgvGastosPorCategoria.AllowUserToDeleteRows = false;
            dgvGastosPorCategoria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGastosPorCategoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGastosPorCategoria.Dock = DockStyle.Fill;
            dgvGastosPorCategoria.Location = new Point(3, 3);
            dgvGastosPorCategoria.MultiSelect = false;
            dgvGastosPorCategoria.Name = "dgvGastosPorCategoria";
            dgvGastosPorCategoria.ReadOnly = true;
            dgvGastosPorCategoria.RowHeadersVisible = false;
            dgvGastosPorCategoria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGastosPorCategoria.Size = new Size(936, 488);
            dgvGastosPorCategoria.TabIndex = 0;
            // 
            // UcExtrato
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(244, 246, 248);
            Controls.Add(tabExtrato);
            Controls.Add(pnlFiltrosExtrato);
            Controls.Add(lblDescricao);
            Controls.Add(lblTitulo);
            Font = new Font("Segoe UI", 10F);
            Name = "UcExtrato";
            Size = new Size(1000, 900);
            pnlFiltrosExtrato.ResumeLayout(false);
            pnlFiltrosExtrato.PerformLayout();
            tabExtrato.ResumeLayout(false);
            tabMovimentacoes.ResumeLayout(false);
            pnlResultadoExtrato.ResumeLayout(false);
            splExtrato.Panel1.ResumeLayout(false);
            splExtrato.Panel2.ResumeLayout(false);
            splExtrato.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splExtrato).EndInit();
            splExtrato.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvExtrato).EndInit();
            pnlAcoesExtrato.ResumeLayout(false);
            tabRelatorioGastos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGastosPorCategoria).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblDescricao;
        private Panel pnlFiltrosExtrato;
        private DateTimePicker dtpDataFinalExtrato;
        private DateTimePicker dtpDataInicialExtrato;
        private Label lblDataFinalExtrato;
        private Label lblDataInicialExtrato;
        private CheckBox chkTodasContasExtrato;
        private Label lblContasExtrato;
        private CheckedListBox clbContasExtrato;
        private Button btnConsultarExtrato;
        private TabControl tabExtrato;
        private TabPage tabMovimentacoes;
        private Panel pnlResultadoExtrato;
        private SplitContainer splExtrato;
        private DataGridView dgvExtrato;
        private TextBox txtDetalhesExtrato;
        private Label lblTituloDetalhesExtrato;
        private Label lblTotaisExtrato;
        private FlowLayoutPanel pnlAcoesExtrato;
        private Button btnDetalhesExtrato;
        private Button btnFecharDetalhesExtrato;
        private Label lblStatusExtrato;
        private TabPage tabRelatorioGastos;
        private DataGridView dgvGastosPorCategoria;
    }
}
