using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Infraestrutura.Memoria;
using AprenderMais.Infraestrutura.Relogio;
using AprenderMais.Infraestrutura.Seguranca;

namespace AprenderMais.Testes;

/// <summary>
/// Monta o sistema completo em memória, com os dados de demonstração, para cada teste.
/// </summary>
internal class Ambiente
{
    public BancoEmMemoria Banco { get; } = new();
    public IPasswordHasher Hasher { get; } = new BCryptPasswordHasher();
    public IRelogio Relogio { get; } = new RelogioSistema();

    public UsuarioRepositorioMemoria Usuarios { get; }
    public ConteudoRepositorioMemoria Conteudos { get; }
    public MatriculaRepositorioMemoria Matriculas { get; }

    public Ambiente()
    {
        DadosIniciais.Popular(Banco, Hasher);
        Usuarios = new UsuarioRepositorioMemoria(Banco);
        Conteudos = new ConteudoRepositorioMemoria(Banco);
        Matriculas = new MatriculaRepositorioMemoria(Banco);
    }

    public AutenticacaoService CriarAutenticacao() => new(Usuarios, Hasher, new SessaoUsuario());
    public ConteudoService CriarConteudoService() => new(Conteudos);
    public MatriculaService CriarMatriculaService() => new(Matriculas, Conteudos, Relogio);
    public UsuarioService CriarUsuarioService() => new(Usuarios);

    public async Task<T> ObterAsync<T>(string email) where T : Usuario
    {
        var usuario = await Usuarios.ObterPorEmailAsync(email);
        return (T)usuario!;
    }
}
