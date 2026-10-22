using System.Globalization;

namespace JKCASH.WinForms.Visual;

// Configuração explícita impede que objetos e identificadores virem colunas automáticas.
internal static class TabelaVisual
{
    internal static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    internal static void Configurar(DataGridView tabela,
        params (string Propriedade, string Titulo, string Formato, int Largura)[] colunas)
    {
        tabela.AutoGenerateColumns = false;
        tabela.Columns.Clear();
        tabela.ReadOnly = true;
        tabela.AllowUserToAddRows = false;
        tabela.AllowUserToDeleteRows = false;
        tabela.AllowUserToResizeRows = false;
        tabela.RowHeadersVisible = false;
        tabela.MultiSelect = false;
        tabela.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabela.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabela.BackgroundColor = Color.White;
        tabela.BorderStyle = BorderStyle.None;
        tabela.GridColor = EstiloVisual.Borda;
        tabela.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        tabela.EnableHeadersVisualStyles = false;
        tabela.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = EstiloVisual.Fundo, ForeColor = EstiloVisual.Texto,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        tabela.DefaultCellStyle = new DataGridViewCellStyle
        {
            ForeColor = EstiloVisual.Texto, SelectionForeColor = EstiloVisual.Texto,
            SelectionBackColor = Color.FromArgb(232, 243, 239), FormatProvider = Cultura
        };
        tabela.RowTemplate.Height = 32;
        foreach (var coluna in colunas)
        {
            tabela.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = coluna.Propriedade, DataPropertyName = coluna.Propriedade,
                HeaderText = coluna.Titulo, FillWeight = coluna.Largura,
                MinimumWidth = coluna.Formato == "C2" ? 100 : 80,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = coluna.Formato, FormatProvider = Cultura,
                    Alignment = coluna.Formato == "C2"
                        ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft
                }
            });
        }
    }
}
