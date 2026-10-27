using JKCASH.Application.Modelos;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms;

public sealed class FrmBackups : Form
{
    private readonly DataGridView _tabela = new();
    public BackupResumo? Escolhido => _tabela.CurrentRow?.DataBoundItem is Linha linha ? linha.Backup : null;

    public FrmBackups(IReadOnlyList<BackupResumo> backups)
    {
        Text = "JKCASH — Escolher backup";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(720, 360);
        MinimumSize = new Size(650, 350);
        BackColor = EstiloVisual.Fundo;
        Font = new Font("Segoe UI", 10);
        Padding = new Padding(18);
        var aviso = EstiloVisual.Rotulo("Escolha uma cópia para baixar e conferir antes de restaurar.");
        aviso.Dock = DockStyle.Top;
        aviso.Height = 42;
        _tabela.Dock = DockStyle.Fill;
        _tabela.ReadOnly = true;
        _tabela.AllowUserToAddRows = false;
        _tabela.AllowUserToDeleteRows = false;
        _tabela.MultiSelect = false;
        _tabela.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _tabela.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _tabela.RowHeadersVisible = false;
        _tabela.DataSource = backups.Select(b => new Linha(b)).ToList();
        var acoes = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, FlowDirection = FlowDirection.RightToLeft };
        var escolher = EstiloVisual.Botao("Baixar e conferir", true);
        escolher.Width = 180;
        escolher.Enabled = backups.Count > 0;
        escolher.Click += (_, _) => { if (Escolhido is not null) DialogResult = DialogResult.OK; };
        var cancelar = EstiloVisual.Botao("Cancelar");
        cancelar.Width = 120;
        cancelar.DialogResult = DialogResult.Cancel;
        acoes.Controls.Add(escolher);
        acoes.Controls.Add(cancelar);
        Controls.Add(_tabela);
        Controls.Add(aviso);
        Controls.Add(acoes);
        CancelButton = cancelar;
        AcceptButton = escolher;
    }

    private sealed class Linha(BackupResumo backup)
    {
        [System.ComponentModel.Browsable(false)]
        public BackupResumo Backup { get; } = backup;
        public string Data => Backup.CriadoEmUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
        public string Computador => Backup.Maquina;
        public string Tamanho => $"{Backup.TamanhoBytes / 1024m:N1} KB";
        public int Formato => Backup.VersaoFormato;
    }
}
