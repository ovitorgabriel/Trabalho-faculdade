namespace AprenderMais.Desktop;

/// <summary>
/// Cria tabelas (DataGridView) somente leitura com colunas definidas em código.
/// </summary>
internal static class GridHelper
{
    public static DataGridView Criar() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        AutoGenerateColumns = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = SystemColors.Window,
        BorderStyle = BorderStyle.None
    };

    public static void Coluna(DataGridView grid, string propriedade, string titulo, float peso = 100f)
    {
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propriedade,
            HeaderText = titulo,
            FillWeight = peso
        });
    }
}
