using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Aplicacao.Servicos;

public class AutenticacaoService
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly SessaoUsuario _sessao;

    public AutenticacaoService(IUsuarioRepositorio usuarios, IPasswordHasher hasher, SessaoUsuario sessao)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _sessao = sessao;
    }

    public async Task<Usuario> EntrarAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(senha))
            throw new DominioException("Informe e-mail e senha.");

        var usuario = await _usuarios.ObterPorEmailAsync(email.Trim());

        // Mesma mensagem para e-mail inexistente e senha errada: não revela quais e-mails existem.
        if (usuario is null || !_hasher.Verificar(senha, usuario.SenhaHash))
            throw new DominioException("E-mail ou senha inválidos.");

        if (!usuario.Ativo)
            throw new DominioException("Este usuário está inativo. Procure a administração.");

        _sessao.Entrar(usuario);
        return usuario;
    }

    public void Sair() => _sessao.Sair();
}
