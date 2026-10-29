using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Desktop.Formularios;

/// <summary>
/// Janela principal. As abas exibidas dependem do perfil do usuário autenticado.
/// </summary>
public class PrincipalForm : Form
{
    private readonly ConteudoService _conteudos;
    private readonly MatriculaService _matriculas;
    private readonly UsuarioService _usuarios;
    private readonly Usuario _usuario;

    private readonly TabControl _abas = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _gridCatalogo = GridHelper.Criar();
    private readonly DataGridView _gridMatriculas = GridHelper.Criar();
    private readonly DataGridView _gridGestao = GridHelper.Criar();
    private readonly DataGridView _gridUsuarios = GridHelper.Criar();

    /// <summary>Verdadeiro quando o usuário clicou em "Sair da conta" (volta ao login).</summary>
    public bool SairDaConta { get; private set; }

    public PrincipalForm(
        SessaoUsuario sessao,
        ConteudoService conteudos,
        MatriculaService matriculas,
        UsuarioService usuarios)
    {
        _conteudos = conteudos;
        _matriculas = matriculas;
        _usuarios = usuarios;
        _usuario = sessao.Atual ?? throw new InvalidOperationException("Nenhum usuário autenticado.");

        Text = "Aprender Mais";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1000, 620);
        MinimumSize = new Size(820, 480);

        // Controle que preenche o espaço é adicionado primeiro; o painel do topo depois.
        Controls.Add(_abas);
        Controls.Add(CriarPainelTopo());

        _abas.TabPages.Add(CriarAbaCatalogo());
        switch (_usuario.Perfil)
        {
            case PerfilUsuario.Aluno:
                _abas.TabPages.Add(CriarAbaMinhasMatriculas());
                break;
            case PerfilUsuario.Secretaria:
                _abas.TabPages.Add(CriarAbaGestaoConteudos());
                break;
            case PerfilUsuario.Admin:
                _abas.TabPages.Add(CriarAbaUsuarios());
                break;
        }

        Shown += async (_, _) => await RecarregarAsync();
    }

    // ------------------------------------------------------------------ montagem da tela

    private Panel CriarPainelTopo()
    {
        var painel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(12, 6, 12, 6)
        };

        var lblUsuario = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Text = $"{_usuario.Nome}  -  {RotuloPerfil(_usuario.Perfil)}"
        };

        var btnSair = new Button
        {
            Dock = DockStyle.Right,
            Width = 120,
            Text = "Sair da conta"
        };
        btnSair.Click += (_, _) =>
        {
            SairDaConta = true;
            Close();
        };

        painel.Controls.Add(lblUsuario);
        painel.Controls.Add(btnSair);
        return painel;
    }

    private TabPage CriarAbaCatalogo()
    {
        var aba = new TabPage("Catálogo de conteúdos");

        GridHelper.Coluna(_gridCatalogo, nameof(ConteudoDto.Titulo), "Título", 220);
        GridHelper.Coluna(_gridCatalogo, nameof(ConteudoDto.Categoria), "Categoria", 120);
        GridHelper.Coluna(_gridCatalogo, nameof(ConteudoDto.Nivel), "Nível", 100);
        GridHelper.Coluna(_gridCatalogo, nameof(ConteudoDto.Duracao), "Duração", 70);
        GridHelper.Coluna(_gridCatalogo, nameof(ConteudoDto.Aulas), "Aulas", 60);

        aba.Controls.Add(_gridCatalogo);

        if (_usuario is Aluno)
        {
            var btnMatricular = new Button { Text = "Matricular-me no conteúdo selecionado", AutoSize = true };
            btnMatricular.Click += async (_, _) => await MatricularAsync();
            aba.Controls.Add(CriarRodape(btnMatricular));
        }

        return aba;
    }

    private TabPage CriarAbaMinhasMatriculas()
    {
        var aba = new TabPage("Minhas matrículas");

        GridHelper.Coluna(_gridMatriculas, nameof(MatriculaDto.Conteudo), "Conteúdo", 220);
        GridHelper.Coluna(_gridMatriculas, nameof(MatriculaDto.Status), "Situação", 100);
        GridHelper.Coluna(_gridMatriculas, nameof(MatriculaDto.ProgressoTexto), "Progresso", 80);
        GridHelper.Coluna(_gridMatriculas, nameof(MatriculaDto.DataMatricula), "Matriculado em", 100);

        aba.Controls.Add(_gridMatriculas);

        var btnCancelar = new Button { Text = "Cancelar matrícula selecionada", AutoSize = true };
        btnCancelar.Click += async (_, _) => await CancelarMatriculaAsync();
        aba.Controls.Add(CriarRodape(btnCancelar));

        return aba;
    }

    private TabPage CriarAbaGestaoConteudos()
    {
        var aba = new TabPage("Gerenciar conteúdos");

        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Titulo), "Título", 220);
        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Categoria), "Categoria", 120);
        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Nivel), "Nível", 100);
        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Duracao), "Duração", 70);
        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Aulas), "Aulas", 60);
        GridHelper.Coluna(_gridGestao, nameof(ConteudoDto.Situacao), "Situação", 80);

        aba.Controls.Add(_gridGestao);

        var btnNovo = new Button { Text = "Novo conteúdo...", AutoSize = true };
        btnNovo.Click += async (_, _) => await NovoConteudoAsync();

        var btnAlternar = new Button { Text = "Ativar / desativar selecionado", AutoSize = true };
        btnAlternar.Click += async (_, _) => await AlternarConteudoAsync();

        aba.Controls.Add(CriarRodape(btnNovo, btnAlternar));
        return aba;
    }

    private TabPage CriarAbaUsuarios()
    {
        var aba = new TabPage("Usuários");

        GridHelper.Coluna(_gridUsuarios, nameof(UsuarioDto.Nome), "Nome", 180);
        GridHelper.Coluna(_gridUsuarios, nameof(UsuarioDto.Email), "E-mail", 220);
        GridHelper.Coluna(_gridUsuarios, nameof(UsuarioDto.Perfil), "Perfil", 100);
        GridHelper.Coluna(_gridUsuarios, nameof(UsuarioDto.Situacao), "Situação", 80);

        aba.Controls.Add(_gridUsuarios);

        var btnAlternar = new Button { Text = "Ativar / desativar selecionado", AutoSize = true };
        btnAlternar.Click += async (_, _) => await AlternarUsuarioAsync();
        aba.Controls.Add(CriarRodape(btnAlternar));

        return aba;
    }

    private static FlowLayoutPanel CriarRodape(params Control[] botoes)
    {
        var painel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(8)
        };
        painel.Controls.AddRange(botoes);
        return painel;
    }

    // ------------------------------------------------------------------ carga de dados

    private async Task RecarregarAsync()
    {
        await ExecutarAsync(async () =>
        {
            _gridCatalogo.DataSource = (await _conteudos.ListarAtivosAsync()).ToList();

            if (_usuario is Aluno)
                _gridMatriculas.DataSource = (await _matriculas.ListarDoAlunoAsync(_usuario)).ToList();

            if (_usuario is Secretaria)
                _gridGestao.DataSource = (await _conteudos.ListarTodosAsync(_usuario)).ToList();

            if (_usuario is Administrador)
                _gridUsuarios.DataSource = (await _usuarios.ListarAsync(_usuario)).ToList();
        });
    }

    // ------------------------------------------------------------------ ações

    private async Task MatricularAsync()
    {
        if (_gridCatalogo.CurrentRow?.DataBoundItem is not ConteudoDto conteudo)
        {
            Aviso("Selecione um conteúdo no catálogo.");
            return;
        }

        await ExecutarAsync(async () =>
        {
            await _matriculas.MatricularAsync(conteudo.Id, _usuario);
            MessageBox.Show(this, $"Matrícula realizada em \"{conteudo.Titulo}\".", "Aprender Mais",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RecarregarAsync();
        });
    }

    private async Task CancelarMatriculaAsync()
    {
        if (_gridMatriculas.CurrentRow?.DataBoundItem is not MatriculaDto matricula)
        {
            Aviso("Selecione uma matrícula.");
            return;
        }

        var confirmar = MessageBox.Show(this, $"Cancelar a matrícula em \"{matricula.Conteudo}\"?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirmar != DialogResult.Yes)
            return;

        await ExecutarAsync(async () =>
        {
            await _matriculas.CancelarAsync(matricula.IdMatricula, _usuario);
            await RecarregarAsync();
        });
    }

    private async Task NovoConteudoAsync()
    {
        using var janela = new NovoConteudoForm(dto => _conteudos.CriarAsync(dto, _usuario));
        if (janela.ShowDialog(this) == DialogResult.OK)
            await RecarregarAsync();
    }

    private async Task AlternarConteudoAsync()
    {
        if (_gridGestao.CurrentRow?.DataBoundItem is not ConteudoDto conteudo)
        {
            Aviso("Selecione um conteúdo.");
            return;
        }

        await ExecutarAsync(async () =>
        {
            await _conteudos.AlterarAtivoAsync(conteudo.Id, !conteudo.Ativo, _usuario);
            await RecarregarAsync();
        });
    }

    private async Task AlternarUsuarioAsync()
    {
        if (_gridUsuarios.CurrentRow?.DataBoundItem is not UsuarioDto usuario)
        {
            Aviso("Selecione um usuário.");
            return;
        }

        await ExecutarAsync(async () =>
        {
            await _usuarios.AlterarAtivoAsync(usuario.Id, !usuario.Ativo, _usuario);
            await RecarregarAsync();
        });
    }

    // ------------------------------------------------------------------ utilidades

    /// <summary>Executa uma ação e mostra erros de regra de negócio ao usuário sem derrubar o programa.</summary>
    private async Task ExecutarAsync(Func<Task> acao)
    {
        try
        {
            await acao();
        }
        catch (DominioException ex)
        {
            Aviso(ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Erro inesperado: " + ex.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Aviso(string mensagem) =>
        MessageBox.Show(this, mensagem, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private static string RotuloPerfil(PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Aluno => "Aluno",
        PerfilUsuario.Secretaria => "Secretaria",
        PerfilUsuario.Admin => "Administrador",
        _ => perfil.ToString()
    };
}
