using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Excecoes;
using Xunit;

namespace AprenderMais.Testes;

public class ServicosTests
{
    // ---------------------------------------------------------------- matrícula

    [Fact]
    public async Task Aluno_se_matricula_em_conteudo_ativo()
    {
        var ambiente = new Ambiente();
        var bruno = await ambiente.ObterAsync<Aluno>("bruno@aprendermais.com");

        var dto = await ambiente.CriarMatriculaService().MatricularAsync(2, bruno);

        Assert.Equal("UX/UI Design", dto.Conteudo);
        Assert.Equal("Em andamento", dto.Status);
    }

    [Fact]
    public async Task Matricula_duplicada_e_rejeitada()
    {
        var ambiente = new Ambiente();
        var marina = await ambiente.ObterAsync<Aluno>("aluno@aprendermais.com");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarMatriculaService().MatricularAsync(1, marina));
    }

    [Fact]
    public async Task Conteudo_inativo_nao_aceita_matricula()
    {
        var ambiente = new Ambiente();
        var bruno = await ambiente.ObterAsync<Aluno>("bruno@aprendermais.com");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarMatriculaService().MatricularAsync(4, bruno));
    }

    [Fact]
    public async Task Somente_aluno_se_matricula()
    {
        var ambiente = new Ambiente();
        var secretaria = await ambiente.ObterAsync<Secretaria>("secretaria@aprendermais.com");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarMatriculaService().MatricularAsync(1, secretaria));
    }

    [Fact]
    public async Task Cancelar_e_matricular_de_novo_reativa_a_matricula()
    {
        var ambiente = new Ambiente();
        var servico = ambiente.CriarMatriculaService();
        var bruno = await ambiente.ObterAsync<Aluno>("bruno@aprendermais.com");

        var criada = await servico.MatricularAsync(2, bruno);
        await servico.CancelarAsync(criada.IdMatricula, bruno);
        var reativada = await servico.MatricularAsync(2, bruno);

        Assert.Equal(criada.IdMatricula, reativada.IdMatricula);
        Assert.Equal("Em andamento", reativada.Status);
    }

    [Fact]
    public async Task Aluno_nao_cancela_matricula_de_outro_aluno()
    {
        var ambiente = new Ambiente();
        var bruno = await ambiente.ObterAsync<Aluno>("bruno@aprendermais.com");

        // Matrícula 1 pertence à Marina.
        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarMatriculaService().CancelarAsync(1, bruno));
    }

    // ---------------------------------------------------------------- conteúdos

    [Fact]
    public async Task Catalogo_lista_somente_conteudos_ativos()
    {
        var lista = await new Ambiente().CriarConteudoService().ListarAtivosAsync();

        Assert.Equal(3, lista.Count);
        Assert.DoesNotContain(lista, c => c.Titulo == "Fundamentos de T.I.");
    }

    [Fact]
    public async Task Secretaria_cria_conteudo()
    {
        var ambiente = new Ambiente();
        var secretaria = await ambiente.ObterAsync<Secretaria>("secretaria@aprendermais.com");
        var dto = new NovoConteudoDto("Banco de Dados", "SQL e modelagem", "Dados", "Intermediário", "20h", 30);

        var criado = await ambiente.CriarConteudoService().CriarAsync(dto, secretaria);

        Assert.Equal("Banco de Dados", criado.Titulo);
        Assert.True(criado.Id > 0);
    }

    [Fact]
    public async Task Aluno_nao_cria_conteudo()
    {
        var ambiente = new Ambiente();
        var marina = await ambiente.ObterAsync<Aluno>("aluno@aprendermais.com");
        var dto = new NovoConteudoDto("Qualquer", null, "Dados", "Iniciante", "1h", 1);

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarConteudoService().CriarAsync(dto, marina));
    }

    [Fact]
    public async Task Conteudo_sem_titulo_nao_e_salvo()
    {
        var ambiente = new Ambiente();
        var secretaria = await ambiente.ObterAsync<Secretaria>("secretaria@aprendermais.com");
        var dto = new NovoConteudoDto("", null, "Dados", "Iniciante", "1h", 1);

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarConteudoService().CriarAsync(dto, secretaria));
    }

    // ---------------------------------------------------------------- usuários

    [Fact]
    public async Task Admin_lista_todos_os_usuarios()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>("admin@aprendermais.com");

        var lista = await ambiente.CriarUsuarioService().ListarAsync(admin);

        Assert.Equal(5, lista.Count);
    }

    [Fact]
    public async Task Admin_nao_desativa_a_propria_conta()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>("admin@aprendermais.com");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().AlterarAtivoAsync(admin.Id, false, admin));
    }

    [Fact]
    public async Task Aluno_nao_lista_usuarios()
    {
        var ambiente = new Ambiente();
        var marina = await ambiente.ObterAsync<Aluno>("aluno@aprendermais.com");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().ListarAsync(marina));
    }

    // ---------------------------------------------------------- cadastro de usuários

    [Fact]
    public async Task Admin_cadastra_secretaria_com_senha_hash()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>(
            "admin@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "111.222.333-44",
            "Nova Secretaria",
            "nova.secretaria@aprendermais.com",
            "SenhaSegura123");

        var criado = await ambiente.CriarUsuarioService()
            .CriarAsync(dto, admin);

        Assert.Equal("Secretaria", criado.Perfil);
        Assert.True(criado.Ativo);

        var salvo = await ambiente.Usuarios
            .ObterPorEmailAsync(dto.Email);

        Assert.NotNull(salvo);
        Assert.NotEqual(dto.Senha, salvo.SenhaHash);
        Assert.True(ambiente.Hasher.Verificar(
            dto.Senha, salvo.SenhaHash));
    }

    [Fact]
    public async Task Secretaria_cadastra_aluno()
    {
        var ambiente = new Ambiente();
        var secretaria = await ambiente.ObterAsync<Secretaria>(
            "secretaria@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "111.222.333-45",
            "Novo Aluno",
            "novo.aluno@aprendermais.com",
            "SenhaSegura123");

        var criado = await ambiente.CriarUsuarioService()
            .CriarAsync(dto, secretaria);

        Assert.Equal("Aluno", criado.Perfil);

        var salvo = await ambiente.Usuarios
            .ObterPorEmailAsync(dto.Email);

        Assert.IsType<Aluno>(salvo);
    }

    [Fact]
    public async Task Aluno_nao_pode_cadastrar_usuario()
    {
        var ambiente = new Ambiente();
        var aluno = await ambiente.ObterAsync<Aluno>(
            "aluno@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "111.222.333-46",
            "Outro Aluno",
            "outro.aluno@aprendermais.com",
            "SenhaSegura123");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().CriarAsync(dto, aluno));
    }


    [Fact]
    public async Task Cadastro_rejeita_email_duplicado()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>(
            "admin@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "111.222.333-47",
            "Pessoa Nova",
            "aluno@aprendermais.com",
            "SenhaSegura123");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().CriarAsync(dto, admin));
    }

    [Fact]
    public async Task Cadastro_rejeita_cpf_duplicado()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>(
            "admin@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "123.456.789-10",
            "Pessoa Nova",
            "pessoa.nova@aprendermais.com",
            "SenhaSegura123");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().CriarAsync(dto, admin));
    }

    [Fact]
    public async Task Cadastro_rejeita_senha_curta()
    {
        var ambiente = new Ambiente();
        var admin = await ambiente.ObterAsync<Administrador>(
            "admin@aprendermais.com");

        var dto = new NovoUsuarioDto(
            "111.222.333-48",
            "Pessoa Nova",
            "pessoa.senha@aprendermais.com",
            "1234567");

        await Assert.ThrowsAsync<DominioException>(() =>
            ambiente.CriarUsuarioService().CriarAsync(dto, admin));
    }
}
