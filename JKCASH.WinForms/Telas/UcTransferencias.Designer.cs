namespace JKCASH.WinForms.Telas;

partial class UcTransferencias
{
    private System.ComponentModel.IContainer components = new System.ComponentModel.Container();
    protected override void Dispose(bool disposing)
    {
        if (disposing) components.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
            tlpTransferencia = new TableLayoutPanel();
            lblOrigemTransferencia = new Label();
            lblDestinoTransferencia = new Label();
            lblValorTransferencia = new Label();
            lblCategoriaTransferencia = new Label();
            lblImportanciaTransferencia = new Label();
            lblPlanejamentoTransferencia = new Label();
            lblUrgenciaTransferencia = new Label();
            lblDataTransferencia = new Label();
            cmbOrigemTransferencia = new ComboBox();
            cmbDestinoTransferencia = new ComboBox();
            cmbCategoriaTransferencia = new ComboBox();
            cmbImportanciaTransferencia = new ComboBox();
            cmbPlanejamentoTransferencia = new ComboBox();
            cmbUrgenciaTransferencia = new ComboBox();
            numValorTransferencia = new NumericUpDown();
            lblHojeTransferencia = new Label();
            lblSaldosTransferencia = new Label();
            lblDescricaoTransferencia = new Label();
            txtDescricaoTransferencia = new TextBox();
            lblJustificativaTransferencia = new Label();
            txtJustificativaTransferencia = new TextBox();
            btnAtualizarTransferencia = new Button();
            btnConfirmarTransferencia = new Button();
            txtResultadoTransferencia = new TextBox();
            SuspendLayout();
            // tlpTransferencia
            tlpTransferencia.Name = "tlpTransferencia";
            tlpTransferencia.Dock = DockStyle.Top;
            tlpTransferencia.ColumnCount = 2;
            tlpTransferencia.RowCount = 8;
            tlpTransferencia.Location = new Point(20, 20);
            tlpTransferencia.Size = new Size(860, 288);
            tlpTransferencia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpTransferencia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpTransferencia.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            // lblOrigemTransferencia
            lblOrigemTransferencia.Name = "lblOrigemTransferencia";
            lblOrigemTransferencia.AutoSize = true;
            lblOrigemTransferencia.Text = "Conta de origem";
            lblOrigemTransferencia.Location = new Point(3, 0);
            // lblDestinoTransferencia
            lblDestinoTransferencia.Name = "lblDestinoTransferencia";
            lblDestinoTransferencia.AutoSize = true;
            lblDestinoTransferencia.Text = "Conta de destino";
            lblDestinoTransferencia.Location = new Point(433, 0);
            // lblValorTransferencia
            lblValorTransferencia.Name = "lblValorTransferencia";
            lblValorTransferencia.AutoSize = true;
            lblValorTransferencia.Text = "Valor da transferência";
            lblValorTransferencia.Location = new Point(3, 72);
            // lblCategoriaTransferencia
            lblCategoriaTransferencia.Name = "lblCategoriaTransferencia";
            lblCategoriaTransferencia.AutoSize = true;
            lblCategoriaTransferencia.Text = "Categoria";
            lblCategoriaTransferencia.Location = new Point(433, 72);
            // lblImportanciaTransferencia
            lblImportanciaTransferencia.Name = "lblImportanciaTransferencia";
            lblImportanciaTransferencia.AutoSize = true;
            lblImportanciaTransferencia.Text = "Importância";
            lblImportanciaTransferencia.Location = new Point(3, 144);
            // lblPlanejamentoTransferencia
            lblPlanejamentoTransferencia.Name = "lblPlanejamentoTransferencia";
            lblPlanejamentoTransferencia.AutoSize = true;
            lblPlanejamentoTransferencia.Text = "Planejamento";
            lblPlanejamentoTransferencia.Location = new Point(433, 144);
            // lblUrgenciaTransferencia
            lblUrgenciaTransferencia.Name = "lblUrgenciaTransferencia";
            lblUrgenciaTransferencia.AutoSize = true;
            lblUrgenciaTransferencia.Text = "Urgência";
            lblUrgenciaTransferencia.Location = new Point(3, 216);
            // lblDataTransferencia
            lblDataTransferencia.Name = "lblDataTransferencia";
            lblDataTransferencia.AutoSize = true;
            lblDataTransferencia.Text = "Data da movimentação";
            lblDataTransferencia.Location = new Point(433, 216);
            // cmbOrigemTransferencia
            cmbOrigemTransferencia.Name = "cmbOrigemTransferencia";
            cmbOrigemTransferencia.Dock = DockStyle.Top;
            cmbOrigemTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrigemTransferencia.Location = new Point(3, 27);
            cmbOrigemTransferencia.Size = new Size(424, 25);
            cmbOrigemTransferencia.TabIndex = 2;
            // cmbDestinoTransferencia
            cmbDestinoTransferencia.Name = "cmbDestinoTransferencia";
            cmbDestinoTransferencia.Dock = DockStyle.Top;
            cmbDestinoTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDestinoTransferencia.Location = new Point(433, 27);
            cmbDestinoTransferencia.Size = new Size(424, 25);
            cmbDestinoTransferencia.TabIndex = 3;
            // cmbCategoriaTransferencia
            cmbCategoriaTransferencia.Name = "cmbCategoriaTransferencia";
            cmbCategoriaTransferencia.Dock = DockStyle.Top;
            cmbCategoriaTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoriaTransferencia.Location = new Point(433, 99);
            cmbCategoriaTransferencia.Size = new Size(424, 25);
            cmbCategoriaTransferencia.TabIndex = 7;
            // cmbImportanciaTransferencia
            cmbImportanciaTransferencia.Name = "cmbImportanciaTransferencia";
            cmbImportanciaTransferencia.Dock = DockStyle.Top;
            cmbImportanciaTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImportanciaTransferencia.Location = new Point(3, 171);
            cmbImportanciaTransferencia.Size = new Size(424, 25);
            cmbImportanciaTransferencia.TabIndex = 10;
            // cmbPlanejamentoTransferencia
            cmbPlanejamentoTransferencia.Name = "cmbPlanejamentoTransferencia";
            cmbPlanejamentoTransferencia.Dock = DockStyle.Top;
            cmbPlanejamentoTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlanejamentoTransferencia.Location = new Point(433, 171);
            cmbPlanejamentoTransferencia.Size = new Size(424, 25);
            cmbPlanejamentoTransferencia.TabIndex = 11;
            // cmbUrgenciaTransferencia
            cmbUrgenciaTransferencia.Name = "cmbUrgenciaTransferencia";
            cmbUrgenciaTransferencia.Dock = DockStyle.Top;
            cmbUrgenciaTransferencia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUrgenciaTransferencia.Location = new Point(3, 243);
            cmbUrgenciaTransferencia.Size = new Size(424, 25);
            cmbUrgenciaTransferencia.TabIndex = 14;
            // numValorTransferencia
            numValorTransferencia.Name = "numValorTransferencia";
            numValorTransferencia.Dock = DockStyle.Top;
            numValorTransferencia.DecimalPlaces = 2;
            numValorTransferencia.Maximum = 10000000m;
            numValorTransferencia.ThousandsSeparator = true;
            numValorTransferencia.Size = new Size(424, 25);
            numValorTransferencia.TabIndex = 6;
            // lblHojeTransferencia
            lblHojeTransferencia.Name = "lblHojeTransferencia";
            lblHojeTransferencia.Dock = DockStyle.Fill;
            lblHojeTransferencia.Text = "Data de hoje";
            lblHojeTransferencia.TextAlign = ContentAlignment.TopLeft;
            // lblSaldosTransferencia
            lblSaldosTransferencia.Name = "lblSaldosTransferencia";
            lblSaldosTransferencia.Location = new Point(20, 316);
            lblSaldosTransferencia.Size = new Size(860, 48);
            lblSaldosTransferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSaldosTransferencia.Text = "Selecione duas contas ativas para consultar os saldos.";
            lblSaldosTransferencia.AutoEllipsis = true;
            lblSaldosTransferencia.ForeColor = Color.FromArgb(105, 122, 143);
            // lblDescricaoTransferencia
            lblDescricaoTransferencia.Name = "lblDescricaoTransferencia";
            lblDescricaoTransferencia.AutoSize = true;
            lblDescricaoTransferencia.Location = new Point(20, 372);
            lblDescricaoTransferencia.Text = "Descrição";
            // txtDescricaoTransferencia
            txtDescricaoTransferencia.Name = "txtDescricaoTransferencia";
            txtDescricaoTransferencia.Location = new Point(20, 396);
            txtDescricaoTransferencia.Size = new Size(860, 25);
            txtDescricaoTransferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescricaoTransferencia.MaxLength = 200;
            txtDescricaoTransferencia.TabIndex = 16;
            // lblJustificativaTransferencia
            lblJustificativaTransferencia.Name = "lblJustificativaTransferencia";
            lblJustificativaTransferencia.AutoSize = true;
            lblJustificativaTransferencia.Location = new Point(20, 434);
            lblJustificativaTransferencia.Text = "Justificativa";
            // txtJustificativaTransferencia
            txtJustificativaTransferencia.Name = "txtJustificativaTransferencia";
            txtJustificativaTransferencia.Location = new Point(20, 458);
            txtJustificativaTransferencia.Size = new Size(860, 76);
            txtJustificativaTransferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJustificativaTransferencia.Multiline = true;
            txtJustificativaTransferencia.ScrollBars = ScrollBars.Vertical;
            txtJustificativaTransferencia.MaxLength = 500;
            txtJustificativaTransferencia.TabIndex = 17;
            // btnAtualizarTransferencia
            btnAtualizarTransferencia.Name = "btnAtualizarTransferencia";
            btnAtualizarTransferencia.Location = new Point(20, 554);
            btnAtualizarTransferencia.Size = new Size(180, 40);
            btnAtualizarTransferencia.Text = "Atualizar contas";
            btnAtualizarTransferencia.FlatStyle = FlatStyle.Flat;
            btnAtualizarTransferencia.BackColor = Color.White;
            btnAtualizarTransferencia.Cursor = Cursors.Hand;
            btnAtualizarTransferencia.TabIndex = 18;
            // btnConfirmarTransferencia
            btnConfirmarTransferencia.Name = "btnConfirmarTransferencia";
            btnConfirmarTransferencia.Location = new Point(640, 554);
            btnConfirmarTransferencia.Size = new Size(240, 40);
            btnConfirmarTransferencia.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirmarTransferencia.Text = "Confirmar transferência";
            btnConfirmarTransferencia.FlatStyle = FlatStyle.Flat;
            btnConfirmarTransferencia.FlatAppearance.BorderSize = 0;
            btnConfirmarTransferencia.BackColor = Color.FromArgb(22, 134, 92);
            btnConfirmarTransferencia.ForeColor = Color.White;
            btnConfirmarTransferencia.UseVisualStyleBackColor = false;
            btnConfirmarTransferencia.Cursor = Cursors.Hand;
            btnConfirmarTransferencia.Enabled = false;
            btnConfirmarTransferencia.TabIndex = 19;
            // txtResultadoTransferencia
            txtResultadoTransferencia.Name = "txtResultadoTransferencia";
            txtResultadoTransferencia.Location = new Point(20, 608);
            txtResultadoTransferencia.Size = new Size(860, 105);
            txtResultadoTransferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtResultadoTransferencia.ReadOnly = true;
            txtResultadoTransferencia.Multiline = true;
            txtResultadoTransferencia.ScrollBars = ScrollBars.Vertical;
            txtResultadoTransferencia.BorderStyle = BorderStyle.None;
            txtResultadoTransferencia.BackColor = Color.White;
            txtResultadoTransferencia.Text = "A transferência gera uma saída e uma entrada vinculadas. O total entre as contas é preservado.";
            txtResultadoTransferencia.TabStop = false;
            Controls.Add(tlpTransferencia);
            tlpTransferencia.Controls.Add(lblOrigemTransferencia, 0, 0);
            tlpTransferencia.Controls.Add(lblDestinoTransferencia, 1, 0);
            tlpTransferencia.Controls.Add(lblValorTransferencia, 0, 2);
            tlpTransferencia.Controls.Add(lblCategoriaTransferencia, 1, 2);
            tlpTransferencia.Controls.Add(lblImportanciaTransferencia, 0, 4);
            tlpTransferencia.Controls.Add(lblPlanejamentoTransferencia, 1, 4);
            tlpTransferencia.Controls.Add(lblUrgenciaTransferencia, 0, 6);
            tlpTransferencia.Controls.Add(lblDataTransferencia, 1, 6);
            tlpTransferencia.Controls.Add(cmbOrigemTransferencia, 0, 1);
            tlpTransferencia.Controls.Add(cmbDestinoTransferencia, 1, 1);
            tlpTransferencia.Controls.Add(cmbCategoriaTransferencia, 1, 3);
            tlpTransferencia.Controls.Add(cmbImportanciaTransferencia, 0, 5);
            tlpTransferencia.Controls.Add(cmbPlanejamentoTransferencia, 1, 5);
            tlpTransferencia.Controls.Add(cmbUrgenciaTransferencia, 0, 7);
            tlpTransferencia.Controls.Add(numValorTransferencia, 0, 3);
            tlpTransferencia.Controls.Add(lblHojeTransferencia, 1, 7);
            Controls.Add(lblSaldosTransferencia);
            Controls.Add(lblDescricaoTransferencia);
            Controls.Add(txtDescricaoTransferencia);
            Controls.Add(lblJustificativaTransferencia);
            Controls.Add(txtJustificativaTransferencia);
            Controls.Add(btnAtualizarTransferencia);
            Controls.Add(btnConfirmarTransferencia);
            Controls.Add(txtResultadoTransferencia);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(7F, 17F);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.White;
            Padding = new Padding(20);
            AutoScroll = true;
            AutoScrollMinSize = new Size(0, 740);
            Size = new Size(900, 740);
            Name = "UcTransferencias";
            ResumeLayout(false);
            PerformLayout();
    }
    private TableLayoutPanel tlpTransferencia = null!;
    private Label lblOrigemTransferencia = null!;
    private Label lblDestinoTransferencia = null!;
    private Label lblValorTransferencia = null!;
    private Label lblCategoriaTransferencia = null!;
    private Label lblImportanciaTransferencia = null!;
    private Label lblPlanejamentoTransferencia = null!;
    private Label lblUrgenciaTransferencia = null!;
    private Label lblDataTransferencia = null!;
    private ComboBox cmbOrigemTransferencia = null!;
    private ComboBox cmbDestinoTransferencia = null!;
    private ComboBox cmbCategoriaTransferencia = null!;
    private ComboBox cmbImportanciaTransferencia = null!;
    private ComboBox cmbPlanejamentoTransferencia = null!;
    private ComboBox cmbUrgenciaTransferencia = null!;
    private NumericUpDown numValorTransferencia = null!;
    private Label lblHojeTransferencia = null!;
    private Label lblSaldosTransferencia = null!;
    private Label lblDescricaoTransferencia = null!;
    private TextBox txtDescricaoTransferencia = null!;
    private Label lblJustificativaTransferencia = null!;
    private TextBox txtJustificativaTransferencia = null!;
    private Button btnAtualizarTransferencia = null!;
    private Button btnConfirmarTransferencia = null!;
    private TextBox txtResultadoTransferencia = null!;
}

