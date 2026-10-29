using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Desktop.Formularios;

/// <summary>
/// Janela de cadastro de conteúdo. Só fecha com sucesso quando o serviço aceita os dados.
/// </summary>
public class NovoConteudoForm : Form
{
    private readonly Func<NovoConteudoDto, Task> _salvar;
    private int _proximaLinha;

    private readonly TextBox _titulo = new();
    private readonly TextBox _categoria = new();
    private readonly ComboBox _nivel = new();
    private readonly TextBox _duracao = new();
    private readonly NumericUpDown _aulas = new();
    private readonly TextBox _descricao = new();
    private readonly Button _btnSalvar = new();
    private readonly Button _btnCancelar = new();

    public NovoConteudoForm(Func<NovoConteudoDto, Task> salvar)
    {
        _salvar = salvar;

        Text = "Novo conteúdo";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 440);

        _nivel.DropDownStyle = ComboBoxStyle.DropDownList;
        _nivel.Items.AddRange(new object[] { "Iniciante", "Intermediário", "Avançado" });
        _nivel.SelectedIndex = 0;

        _duracao.Text = "10h";

        _aulas.Minimum = 0;
        _aulas.Maximum = 500;
        _aulas.Value = 10;

        _descricao.Multiline = true;
        _descricao.AcceptsReturn = true;
        _descricao.ScrollBars = ScrollBars.Vertical;
        _descricao.Height = 110;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(16)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        AdicionarLinha(layout, "Título", _titulo);
        AdicionarLinha(layout, "Categoria", _categoria);
        AdicionarLinha(layout, "Nível", _nivel);
        AdicionarLinha(layout, "Duração", _duracao);
        AdicionarLinha(layout, "Aulas", _aulas);
        AdicionarLinha(layout, "Descrição", _descricao);

        _btnSalvar.Text = "Salvar";
        _btnSalvar.Size = new Size(100, 32);
        _btnSalvar.Click += Salvar_Click;

        _btnCancelar.Text = "Cancelar";
        _btnCancelar.Size = new Size(100, 32);
        _btnCancelar.DialogResult = DialogResult.Cancel;

        var botoes = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill,
            AutoSize = true
        };
        botoes.Controls.Add(_btnCancelar);
        botoes.Controls.Add(_btnSalvar);

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(botoes, 0, _proximaLinha++);
        layout.SetColumnSpan(botoes, 2);

        Controls.Add(layout);

        AcceptButton = _btnSalvar;
        CancelButton = _btnCancelar;
    }

    private void AdicionarLinha(TableLayoutPanel layout, string rotulo, Control campo)
    {
        var linha = _proximaLinha++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label
        {
            Text = rotulo,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 12, 8)
        }, 0, linha);

        campo.Dock = DockStyle.Fill;
        campo.Margin = new Padding(0, 4, 0, 4);
        layout.Controls.Add(campo, 1, linha);
    }

    private async void Salvar_Click(object? sender, EventArgs e)
    {
        _btnSalvar.Enabled = false;

        try
        {
            var dto = new NovoConteudoDto(
                _titulo.Text,
                _descricao.Text,
                _categoria.Text,
                _nivel.Text,
                _duracao.Text,
                (int)_aulas.Value);

            await _salvar(dto);
            DialogResult = DialogResult.OK;
        }
        catch (DominioException ex)
        {
            MessageBox.Show(this, ex.Message, "Dados inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Erro inesperado: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _btnSalvar.Enabled = true;
        }
    }
}
