using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Aplicacao.Servicos;

public class UsuarioService
{
    private readonly IUsuarioRepositorio _usuarios;

    public UsuarioService(IUsuarioRepositorio usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(Usuario solicitante)
    {
        ExigirAdmin(solicitante);

        var lista = await _usuarios.ListarAsync();
        return lista
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioDto(u.Id, u.Nome, u.Email, RotuloPerfil(u.Perfil), u.Ativo))
            .ToList();
    }

    public async Task AlterarAtivoAsync(int idUsuario, bool ativo, Usuario solicitante)
    {
        ExigirAdmin(solicitante);

        if (idUsuario == solicitante.Id && !ativo)
            throw new DominioException("Você não pode desativar o seu próprio usuário.");

        var usuario = await _usuarios.ObterPorIdAsync(idUsuario)
            ?? throw new DominioException("Usuário não encontrado.");

        usuario.Ativo = ativo;
        await _usuarios.AtualizarAsync(usuario);
    }

    private static void ExigirAdmin(Usuario solicitante)
    {
        if (solicitante is not Administrador)
            throw new DominioException("Somente o administrador pode gerenciar usuários.");
    }

    private static string RotuloPerfil(PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Aluno => "Aluno",
        PerfilUsuario.Secretaria => "Secretaria",
        PerfilUsuario.Admin => "Administrador",
        _ => perfil.ToString()
    };
}
