namespace JKCASH.WinForms.Telas;

partial class UcFaturas
{
    private System.ComponentModel.IContainer components = new System.ComponentModel.Container();
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            tlpFaturas = new TableLayoutPanel();
            lblTituloFaturas = new Label();
            lblDescricaoFaturas = new Label();
            pnlFiltrosFaturas = new TableLayoutPanel();
            cmbCartaoFaturas = new ComboBox();
            btnAtualizarFaturas = new Button();
            lblLimiteFatura = new Label();
            dgvFaturas = new DataGridView();
            lblResumoFatura = new Label();
            tabDetalhesFatura = new TabControl();
            tabParcelasFatura = new TabPage();
            tabPagamentosFatura = new TabPage();
            dgvParcelasFatura = new DataGridView();
            dgvPagamentosFatura = new DataGridView();
            pnlPagamentoFatura = new TableLayoutPanel();
            lblContaPagamento = new Label();
            lblCategoriaPagamento = new Label();
            lblValorPagamento = new Label();
            cmbContaPagamento = new ComboBox();
            cmbCategoriaPagamento = new ComboBox();
            numValorPagamento = new NumericUpDown();
            lblOrientacaoPagamento = new Label();
            btnValorPendente = new Button();
            btnPagarFatura = new Button();
            lblStatusFaturas = new Label();
            SuspendLayout();
            // tlpFaturas
            tlpFaturas.Name = "tlpFaturas";
            tlpFaturas.Dock = DockStyle.Top;
            tlpFaturas.Height = 900;
            tlpFaturas.Padding = new Padding(24, 16, 24, 24);
            tlpFaturas.ColumnCount = 1;
            tlpFaturas.RowCount = 9;
            tlpFaturas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 230F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            tlpFaturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));
            // lblTituloFaturas
            lblTituloFaturas.Name = "lblTituloFaturas";
            lblTituloFaturas.Text = "Faturas";
            lblTituloFaturas.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTituloFaturas.ForeColor = Color.FromArgb(20, 38, 61);
            lblTituloFaturas.Dock = DockStyle.Fill;
            // lblDescricaoFaturas
            lblDescricaoFaturas.Name = "lblDescricaoFaturas";
            lblDescricaoFaturas.Text = "Acompanhe parcelas e registre pagamentos totais ou parciais.";
            lblDescricaoFaturas.Dock = DockStyle.Fill;
            lblDescricaoFaturas.ForeColor = Color.FromArgb(105, 122, 143);
            // pnlFiltrosFaturas
            pnlFiltrosFaturas.Name = "pnlFiltrosFaturas";
            pnlFiltrosFaturas.Dock = DockStyle.Fill;
            pnlFiltrosFaturas.ColumnCount = 2;
            pnlFiltrosFaturas.RowCount = 1;
            pnlFiltrosFaturas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFiltrosFaturas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            // cmbCartaoFaturas
            cmbCartaoFaturas.Name = "cmbCartaoFaturas";
            cmbCartaoFaturas.Dock = DockStyle.Top;
            cmbCartaoFaturas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCartaoFaturas.AccessibleName = "Cartão das faturas";
            // btnAtualizarFaturas
            btnAtualizarFaturas.Name = "btnAtualizarFaturas";
            btnAtualizarFaturas.Dock = DockStyle.Fill;
            btnAtualizarFaturas.Text = "Atualizar dados";
            btnAtualizarFaturas.FlatStyle = FlatStyle.Flat;
            btnAtualizarFaturas.BackColor = Color.White;
            btnAtualizarFaturas.Cursor = Cursors.Hand;
            // lblLimiteFatura
            lblLimiteFatura.Name = "lblLimiteFatura";
            lblLimiteFatura.Dock = DockStyle.Fill;
            lblLimiteFatura.AutoEllipsis = true;
            lblLimiteFatura.Text = "Selecione um cartão.";
            lblLimiteFatura.ForeColor = Color.FromArgb(105, 122, 143);
            // dgvFaturas
            dgvFaturas.Name = "dgvFaturas";
            dgvFaturas.Dock = DockStyle.Fill;
            // lblResumoFatura
            lblResumoFatura.Name = "lblResumoFatura";
            lblResumoFatura.Dock = DockStyle.Fill;
            lblResumoFatura.Text = "Selecione uma fatura para consultar os detalhes.";
            lblResumoFatura.AutoEllipsis = true;
            // tabDetalhesFatura
            tabDetalhesFatura.Name = "tabDetalhesFatura";
            tabDetalhesFatura.Dock = DockStyle.Fill;
            // tabParcelasFatura
            tabParcelasFatura.Name = "tabParcelasFatura";
            tabParcelasFatura.Text = "Parcelas da fatura";
            tabParcelasFatura.BackColor = Color.White;
            tabParcelasFatura.Padding = new Padding(12);
            // tabPagamentosFatura
            tabPagamentosFatura.Name = "tabPagamentosFatura";
            tabPagamentosFatura.Text = "Pagamentos confirmados";
            tabPagamentosFatura.BackColor = Color.White;
            tabPagamentosFatura.Padding = new Padding(12);
            // dgvParcelasFatura
            dgvParcelasFatura.Name = "dgvParcelasFatura";
            dgvParcelasFatura.Dock = DockStyle.Fill;
            // dgvPagamentosFatura
            dgvPagamentosFatura.Name = "dgvPagamentosFatura";
            dgvPagamentosFatura.Dock = DockStyle.Fill;
            // pnlPagamentoFatura
            pnlPagamentoFatura.Name = "pnlPagamentoFatura";
            pnlPagamentoFatura.Dock = DockStyle.Fill;
            pnlPagamentoFatura.BackColor = Color.White;
            pnlPagamentoFatura.Padding = new Padding(12);
            pnlPagamentoFatura.ColumnCount = 3;
            pnlPagamentoFatura.RowCount = 4;
            pnlPagamentoFatura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            pnlPagamentoFatura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            pnlPagamentoFatura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlPagamentoFatura.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            pnlPagamentoFatura.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            pnlPagamentoFatura.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            pnlPagamentoFatura.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            // lblContaPagamento
            lblContaPagamento.Name = "lblContaPagamento";
            lblContaPagamento.Text = "Conta usada no pagamento";
            lblContaPagamento.AutoSize = true;
            // lblCategoriaPagamento
            lblCategoriaPagamento.Name = "lblCategoriaPagamento";
            lblCategoriaPagamento.Text = "Categoria da saída";
            lblCategoriaPagamento.AutoSize = true;
            // lblValorPagamento
            lblValorPagamento.Name = "lblValorPagamento";
            lblValorPagamento.Text = "Valor pago";
            lblValorPagamento.AutoSize = true;
            // cmbContaPagamento
            cmbContaPagamento.Name = "cmbContaPagamento";
            cmbContaPagamento.Dock = DockStyle.Top;
            cmbContaPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            // cmbCategoriaPagamento
            cmbCategoriaPagamento.Name = "cmbCategoriaPagamento";
            cmbCategoriaPagamento.Dock = DockStyle.Top;
            cmbCategoriaPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            // numValorPagamento
            numValorPagamento.Name = "numValorPagamento";
            numValorPagamento.Dock = DockStyle.Top;
            numValorPagamento.DecimalPlaces = 2;
            numValorPagamento.Maximum = 10000000m;
            numValorPagamento.ThousandsSeparator = true;
            // lblOrientacaoPagamento
            lblOrientacaoPagamento.Name = "lblOrientacaoPagamento";
            lblOrientacaoPagamento.Text = "Registre um pagamento já confirmado. A data será a de hoje; a conta pode ser de outro banco.";
            lblOrientacaoPagamento.Dock = DockStyle.Fill;
            lblOrientacaoPagamento.AutoEllipsis = true;
            lblOrientacaoPagamento.ForeColor = Color.FromArgb(105, 122, 143);
            // btnValorPendente
            btnValorPendente.Name = "btnValorPendente";
            btnValorPendente.Text = "Preencher valor pendente";
            btnValorPendente.Dock = DockStyle.Fill;
            btnValorPendente.FlatStyle = FlatStyle.Flat;
            btnValorPendente.BackColor = Color.White;
            btnValorPendente.Enabled = false;
            btnValorPendente.Cursor = Cursors.Hand;
            // btnPagarFatura
            btnPagarFatura.Name = "btnPagarFatura";
            btnPagarFatura.Text = "Confirmar pagamento";
            btnPagarFatura.Dock = DockStyle.Fill;
            btnPagarFatura.FlatStyle = FlatStyle.Flat;
            btnPagarFatura.FlatAppearance.BorderSize = 0;
            btnPagarFatura.BackColor = Color.FromArgb(22, 134, 92);
            btnPagarFatura.ForeColor = Color.White;
            btnPagarFatura.UseVisualStyleBackColor = false;
            btnPagarFatura.Enabled = false;
            btnPagarFatura.Cursor = Cursors.Hand;
            // lblStatusFaturas
            lblStatusFaturas.Name = "lblStatusFaturas";
            lblStatusFaturas.Text = "Carregando faturas…";
            lblStatusFaturas.Dock = DockStyle.Fill;
            lblStatusFaturas.AutoEllipsis = true;
            lblStatusFaturas.ForeColor = Color.FromArgb(105, 122, 143);
            tlpFaturas.Controls.Add(lblTituloFaturas, 0, 0);
            tlpFaturas.Controls.Add(lblDescricaoFaturas, 0, 1);
            tlpFaturas.Controls.Add(pnlFiltrosFaturas, 0, 2);
            tlpFaturas.Controls.Add(lblLimiteFatura, 0, 3);
            tlpFaturas.Controls.Add(dgvFaturas, 0, 4);
            tlpFaturas.Controls.Add(lblResumoFatura, 0, 5);
            tlpFaturas.Controls.Add(tabDetalhesFatura, 0, 6);
            tlpFaturas.Controls.Add(pnlPagamentoFatura, 0, 7);
            tlpFaturas.Controls.Add(lblStatusFaturas, 0, 8);
            pnlFiltrosFaturas.Controls.Add(cmbCartaoFaturas, 0, 0);
            pnlFiltrosFaturas.Controls.Add(btnAtualizarFaturas, 1, 0);
            pnlPagamentoFatura.Controls.Add(lblContaPagamento, 0, 0);
            pnlPagamentoFatura.Controls.Add(lblCategoriaPagamento, 1, 0);
            pnlPagamentoFatura.Controls.Add(lblValorPagamento, 2, 0);
            pnlPagamentoFatura.Controls.Add(cmbContaPagamento, 0, 1);
            pnlPagamentoFatura.Controls.Add(cmbCategoriaPagamento, 1, 1);
            pnlPagamentoFatura.Controls.Add(numValorPagamento, 2, 1);
            pnlPagamentoFatura.Controls.Add(lblOrientacaoPagamento, 0, 2);
            pnlPagamentoFatura.Controls.Add(btnValorPendente, 0, 3);
            pnlPagamentoFatura.Controls.Add(btnPagarFatura, 2, 3);
            pnlPagamentoFatura.SetColumnSpan(lblOrientacaoPagamento, 3);
            tabDetalhesFatura.Controls.Add(tabParcelasFatura);
            tabDetalhesFatura.Controls.Add(tabPagamentosFatura);
            tabParcelasFatura.Controls.Add(dgvParcelasFatura);
            tabPagamentosFatura.Controls.Add(dgvPagamentosFatura);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(7F, 17F);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(244, 246, 248);
            AutoScroll = true;
            AutoScrollMinSize = new Size(0, 900);
            Size = new Size(1000, 900);
            Name = "UcFaturas";
            Controls.Add(tlpFaturas);
            ResumeLayout(false);
    }

    private TableLayoutPanel tlpFaturas = null!;
    private Label lblTituloFaturas = null!;
    private Label lblDescricaoFaturas = null!;
    private TableLayoutPanel pnlFiltrosFaturas = null!;
    private ComboBox cmbCartaoFaturas = null!;
    private Button btnAtualizarFaturas = null!;
    private Label lblLimiteFatura = null!;
    private DataGridView dgvFaturas = null!;
    private Label lblResumoFatura = null!;
    private TabControl tabDetalhesFatura = null!;
    private TabPage tabParcelasFatura = null!;
    private TabPage tabPagamentosFatura = null!;
    private DataGridView dgvParcelasFatura = null!;
    private DataGridView dgvPagamentosFatura = null!;
    private TableLayoutPanel pnlPagamentoFatura = null!;
    private Label lblContaPagamento = null!;
    private Label lblCategoriaPagamento = null!;
    private Label lblValorPagamento = null!;
    private ComboBox cmbContaPagamento = null!;
    private ComboBox cmbCategoriaPagamento = null!;
    private NumericUpDown numValorPagamento = null!;
    private Label lblOrientacaoPagamento = null!;
    private Button btnValorPendente = null!;
    private Button btnPagarFatura = null!;
    private Label lblStatusFaturas = null!;
}

