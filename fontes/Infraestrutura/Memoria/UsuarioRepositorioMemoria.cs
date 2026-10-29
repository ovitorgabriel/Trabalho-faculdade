using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Infraestrutura.Memoria;

public class UsuarioRepositorioMemoria : IUsuarioRepositorio
{
    private readonly BancoEmMemoria _banco;

    public UsuarioRepositorioMemoria(BancoEmMemoria banco)
    {
        _banco = banco;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var usuario = _banco.Usuarios.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(usuario);
    }

    public Task<Usuario?> ObterPorIdAsync(int id) =>
        Task.FromResult(_banco.Usuarios.FirstOrDefault(u => u.Id == id));

    public Task<IReadOnlyList<Usuario>> ListarAsync() =>
        Task.FromResult<IReadOnlyList<Usuario>>(_banco.Usuarios.ToList());

    public Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        if (_banco.Usuarios.Any(u => string.Equals(u.Email, usuario.Email, StringComparison.OrdinalIgnoreCase)))
            throw new DominioException("Já existe um usuário com este e-mail.");
        if (_banco.Usuarios.Any(u => u.Cpf == usuario.Cpf))
            throw new DominioException("Já existe um usuário com este CPF.");

        usuario.Id = _banco.ProximoId("usuario");

        switch (usuario)
        {
            case Aluno aluno:
                aluno.IdAluno = _banco.ProximoId("aluno");
                break;
            case Secretaria secretaria:
                secretaria.IdSecretaria = _banco.ProximoId("secretaria");
                break;
            case Administrador admin:
                admin.IdAdmin = _banco.ProximoId("admin");
                break;
        }

        _banco.Usuarios.Add(usuario);
        return Task.FromResult(usuario);
    }

    // Os objetos já são os mesmos da lista; um banco real gravaria as alterações aqui.
    public Task AtualizarAsync(Usuario usuario) => Task.CompletedTask;
}
