namespace JKCASH.WinForms.Telas;

public partial class UcCompraCartao
{
    private void InitializeComponent()
    {
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
            SuspendLayout();
            tlpDadosCompra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numParcelasCompra).BeginInit();
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
            AutoScroll = true;
            BackColor = Color.White;
            Controls.Add(tlpDadosCompra);
            Controls.Add(lblLimiteCompra);
            Controls.Add(lblDescricaoCompra);
            Controls.Add(txtDescricaoCompra);
            Controls.Add(lblJustificativaCompra);
            Controls.Add(txtJustificativaCompra);
            Controls.Add(btnRegistrarCompra);
            Controls.Add(txtResultadoCompra);
            AutoScrollMinSize = new Size(0, 710);
            Name = "UcCompraCartao";
            Padding = new Padding(20);
            Size = new Size(942, 720);
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);
            ((System.ComponentModel.ISupportInitialize)numParcelasCompra).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValorCompra).EndInit();
            tlpDadosCompra.ResumeLayout(false);
            tlpDadosCompra.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
    }

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
}
