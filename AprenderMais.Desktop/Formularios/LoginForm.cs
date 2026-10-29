
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

    private readonly Panel _painelLogin = new();

    public LoginForm(AutenticacaoService autenticacao)
    {
        _autenticacao = autenticacao;

        Text = "Aprender Mais - Entrar";
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        BackColor = Color.FromArgb(245, 247, 250);
        MinimumSize = new Size(500, 550);

        // Painel central do login
        _painelLogin.Size = new Size(400, 430);
        _painelLogin.BackColor = Color.White;
        _painelLogin.BorderStyle = BorderStyle.FixedSingle;

        Controls.Add(_painelLogin);

        // Título
        var titulo = new Label
        {
            Text = "Aprender Mais",
            Font = new Font("Segoe UI", 22f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(28, 25)
        };

        var subtitulo = new Label
        {
            Text = "Entre com sua conta para continuar",
            Font = new Font("Segoe UI", 10f),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(30, 72)
        };

        // E-mail
        var lblEmail = new Label
        {
            Text = "E-mail",
            AutoSize = true,
            Location = new Point(30, 125)
        };

        _txtEmail.Location = new Point(30, 148);
        _txtEmail.Size = new Size(336, 30);
        _txtEmail.Font = new Font("Segoe UI", 11f);
        _txtEmail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // Senha
        var lblSenha = new Label
        {
            Text = "Senha",
            AutoSize = true,
            Location = new Point(30, 195)
        };

        _txtSenha.Location = new Point(30, 218);
        _txtSenha.Size = new Size(336, 30);
        _txtSenha.Font = new Font("Segoe UI", 11f);
        _txtSenha.UseSystemPasswordChar = true;
        _txtSenha.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // Mensagem de erro
        _lblErro.ForeColor = Color.Firebrick;
        _lblErro.Location = new Point(30, 258);
        _lblErro.Size = new Size(336, 40);
        _lblErro.Font = new Font("Segoe UI", 9f);

        // Botão entrar
        _btnEntrar.Text = "Entrar";
        _btnEntrar.Location = new Point(30, 310);
        _btnEntrar.Size = new Size(160, 40);
        _btnEntrar.BackColor = Color.FromArgb(25, 103, 210);
        _btnEntrar.ForeColor = Color.White;
        _btnEntrar.FlatStyle = FlatStyle.Flat;
        _btnEntrar.Cursor = Cursors.Hand;
        _btnEntrar.Click += BtnEntrar_Click;

        // Botão fechar
        _btnFechar.Text = "Fechar";
        _btnFechar.Location = new Point(206, 310);
        _btnFechar.Size = new Size(160, 40);
        _btnFechar.DialogResult = DialogResult.Cancel;
        _btnFechar.Cursor = Cursors.Hand;

        // Contas para demonstração
        var lblDemo = new Label
        {
            Text = "Contas de demonstração (senha: 123456)\r\n" +
                   "aluno@aprendermais.com\r\n" +
                   "secretaria@aprendermais.com\r\n" +
                   "admin@aprendermais.com",
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(30, 365)
        };

        _painelLogin.Controls.AddRange(new Control[]
        {
            titulo,
            subtitulo,
            lblEmail,
            _txtEmail,
            lblSenha,
            _txtSenha,
            _lblErro,
            _btnEntrar,
            _btnFechar,
            lblDemo
        });

        // Centraliza o painel sempre que a janela mudar de tamanho
        void CentralizarPainel()
        {
            _painelLogin.Left =
                (ClientSize.Width - _painelLogin.Width) / 2;

            _painelLogin.Top =
                (ClientSize.Height - _painelLogin.Height) / 2;
        }

        Resize += (_, _) => CentralizarPainel();
        Shown += (_, _) => CentralizarPainel();

        AcceptButton = _btnEntrar;
        CancelButton = _btnFechar;
    }

    private async void BtnEntrar_Click(object? sender, EventArgs e)
    {
        _lblErro.Text = string.Empty;
        _btnEntrar.Enabled = false;

        try
        {
            await _autenticacao.EntrarAsync(
                _txtEmail.Text,
                _txtSenha.Text
            );

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
