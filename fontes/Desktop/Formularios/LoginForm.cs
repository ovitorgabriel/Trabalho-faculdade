using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Desktop.Formularios;

public class LoginForm : Form
{
    private readonly AutenticacaoService _autenticacao;

    private readonly TextBox _txtEmail = new();
    private readonly TextBox _txtSenha = new();
    private readonly Label _lblErro = new();
    private readonly Button _btnEntrar = new();
    private readonly Button _btnFechar = new();

    public LoginForm(AutenticacaoService autenticacao)
    {
        _autenticacao = autenticacao;

        Text = "Aprender Mais - Entrar";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(380, 350);

        var titulo = new Label
        {
            Text = "Aprender Mais",
            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 20)
        };

        var lblEmail = new Label { Text = "E-mail", AutoSize = true, Location = new Point(24, 80) };
        _txtEmail.Location = new Point(24, 100);
        _txtEmail.Width = 332;

        var lblSenha = new Label { Text = "Senha", AutoSize = true, Location = new Point(24, 140) };
        _txtSenha.Location = new Point(24, 160);
        _txtSenha.Width = 332;
        _txtSenha.UseSystemPasswordChar = true;

        _lblErro.ForeColor = Color.Firebrick;
        _lblErro.Location = new Point(24, 198);
        _lblErro.Size = new Size(332, 36);

        _btnEntrar.Text = "Entrar";
        _btnEntrar.Location = new Point(24, 240);
        _btnEntrar.Size = new Size(160, 36);
        _btnEntrar.Click += BtnEntrar_Click;

        _btnFechar.Text = "Fechar";
        _btnFechar.Location = new Point(196, 240);
        _btnFechar.Size = new Size(160, 36);
        _btnFechar.DialogResult = DialogResult.Cancel;

        var lblDemo = new Label
        {
            Text = "Versão de demonstração (senha 123456):\r\n" +
                   "aluno@aprendermais.com\r\n" +
                   "secretaria@aprendermais.com\r\n" +
                   "admin@aprendermais.com",
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(24, 292)
        };

        Controls.AddRange(new Control[]
        {
            titulo, lblEmail, _txtEmail, lblSenha, _txtSenha, _lblErro, _btnEntrar, _btnFechar, lblDemo
        });

        AcceptButton = _btnEntrar;
        CancelButton = _btnFechar;
    }

    private async void BtnEntrar_Click(object? sender, EventArgs e)
    {
        _lblErro.Text = string.Empty;
        _btnEntrar.Enabled = false;

        try
        {
            await _autenticacao.EntrarAsync(_txtEmail.Text, _txtSenha.Text);
            DialogResult = DialogResult.OK;
        }
        catch (DominioException ex)
        {
            _lblErro.Text = ex.Message;
            _txtSenha.Clear();
            _txtSenha.Focus();
        }
        catch (Exception ex)
        {
            _lblErro.Text = "Erro inesperado: " + ex.Message;
        }
        finally
        {
            _btnEntrar.Enabled = true;
        }
    }
}
