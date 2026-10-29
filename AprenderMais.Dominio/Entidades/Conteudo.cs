using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Dominio.Entidades;

public class Conteudo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Nivel { get; set; } = "Iniciante";
    public string Duracao { get; set; } = "0h";
    public int Aulas { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public int IdSecretaria { get; set; }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new DominioException("O título do conteúdo é obrigatório.");
        if (Titulo.Trim().Length > 150)
            throw new DominioException("O título pode ter no máximo 150 caracteres.");
        if (string.IsNullOrWhiteSpace(Categoria))
            throw new DominioException("A categoria do conteúdo é obrigatória.");
        if (Aulas < 0)
            throw new DominioException("A quantidade de aulas não pode ser negativa.");
    }
}
