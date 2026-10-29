using AprenderMais.Dominio.Entidades;

namespace AprenderMais.Dominio.Repositorios;

// Contratos de acesso a dados. O Domínio define o que precisa;
// a Infraestrutura decide como (memória agora, SQL Server depois).

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<Usuario>> ListarAsync();
    Task<Usuario> AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
}

public interface IConteudoRepositorio
{
    Task<IReadOnlyList<Conteudo>> ListarAsync(bool somenteAtivos);
    Task<Conteudo?> ObterPorIdAsync(int id);
    Task<Conteudo> AdicionarAsync(Conteudo conteudo);
    Task AtualizarAsync(Conteudo conteudo);
}

public interface IMatriculaRepositorio
{
    Task<Matricula?> ObterPorIdAsync(int id);
    Task<Matricula?> ObterPorAlunoEConteudoAsync(int idAluno, int idConteudo);
    Task<IReadOnlyList<Matricula>> ListarPorAlunoAsync(int idAluno);
    Task<Matricula> AdicionarAsync(Matricula matricula);
    Task AtualizarAsync(Matricula matricula);
}
