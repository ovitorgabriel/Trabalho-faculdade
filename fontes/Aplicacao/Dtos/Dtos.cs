namespace AprenderMais.Aplicacao.Dtos;

// Objetos de transferência: o que a interface enxerga.
// A tela nunca manipula as entidades do domínio diretamente.

public record ConteudoDto(
    int Id,
    string Titulo,
    string Categoria,
    string Nivel,
    string Duracao,
    int Aulas,
    bool Ativo)
{
    public string Situacao => Ativo ? "Ativo" : "Inativo";
}

public record NovoConteudoDto(
    string Titulo,
    string? Descricao,
    string Categoria,
    string Nivel,
    string Duracao,
    int Aulas);

public record MatriculaDto(
    int IdMatricula,
    string Conteudo,
    string Status,
    decimal Progresso,
    string DataMatricula)
{
    public string ProgressoTexto => $"{Progresso:0.##}%";
}

public record UsuarioDto(
    int Id,
    string Nome,
    string Email,
    string Perfil,
    bool Ativo)
{
    public string Situacao => Ativo ? "Ativo" : "Inativo";
}
