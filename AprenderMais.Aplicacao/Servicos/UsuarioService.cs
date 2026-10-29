
using System.Net.Mail;
using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Aplicacao.Servicos;

public class UsuarioService
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IPasswordHasher _hasher;

    public UsuarioService(
        IUsuarioRepositorio usuarios,
        IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _hasher = hasher;
    }

    public async Task<UsuarioDto> CriarAsync(
        NovoUsuarioDto dto,
        Usuario solicitante)
    {
        if (dto is null)
            throw new DominioException(
                "Informe os dados do usuário.");

        bool criandoSecretaria = solicitante is Administrador;
        bool criandoAluno = solicitante is Secretaria;

        if (!criandoSecretaria && !criandoAluno)
            throw new DominioException(
                "Você não tem permissão para cadastrar usuários.");

        if (string.IsNullOrWhiteSpace(dto.Cpf) ||
            string.IsNullOrWhiteSpace(dto.Nome) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Senha))
        {
            throw new DominioException(
                "Preencha CPF, nome, e-mail e senha.");
        }

        string cpf = new string(
            dto.Cpf.Where(char.IsDigit).ToArray());

        string nome = dto.Nome.Trim();
        string email = dto.Email.Trim();

        if (cpf.Length != 11)
            throw new DominioException(
                "O CPF deve conter 11 dígitos.");

        if (nome.Length < 2)
            throw new DominioException(
                "Informe um nome válido.");

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new DominioException(
                "Informe um e-mail válido.");
        }

        if (dto.Senha.Length < 8)
            throw new DominioException(
                "A senha deve ter pelo menos 8 caracteres.");

        Usuario novoUsuario = criandoSecretaria
            ? new Secretaria()
            : new Aluno();

        novoUsuario.Cpf = cpf;
        novoUsuario.Nome = nome;
        novoUsuario.Email = email;
        novoUsuario.SenhaHash = _hasher.Hash(dto.Senha);
        novoUsuario.Ativo = true;

        await _usuarios.AdicionarAsync(novoUsuario);

        return new UsuarioDto(
            novoUsuario.Id,
            novoUsuario.Nome,
            novoUsuario.Email,
            RotuloPerfil(novoUsuario.Perfil),
            novoUsuario.Ativo);
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(
        Usuario solicitante)
    {
        ExigirAdmin(solicitante);

        var lista = await _usuarios.ListarAsync();

        return lista
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioDto(
                u.Id,
                u.Nome,
                u.Email,
                RotuloPerfil(u.Perfil),
                u.Ativo))
            .ToList();
    }

    public async Task AlterarAtivoAsync(
        int idUsuario,
        bool ativo,
        Usuario solicitante)
    {
        ExigirAdmin(solicitante);

        if (idUsuario == solicitante.Id && !ativo)
            throw new DominioException(
                "Você não pode desativar o seu próprio usuário.");

        var usuario = await _usuarios.ObterPorIdAsync(idUsuario)
            ?? throw new DominioException(
                "Usuário não encontrado.");

        usuario.Ativo = ativo;

        await _usuarios.AtualizarAsync(usuario);
    }

    private static void ExigirAdmin(Usuario solicitante)
    {
        if (solicitante is not Administrador)
            throw new DominioException(
                "Somente o administrador pode gerenciar usuários.");
    }

    private static string RotuloPerfil(PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Aluno => "Aluno",
        PerfilUsuario.Secretaria => "Secretaria",
        PerfilUsuario.Admin => "Administrador",
        _ => perfil.ToString()
    };
}
