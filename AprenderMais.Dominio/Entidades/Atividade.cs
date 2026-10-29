using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Dominio.Entidades;

public class Atividade
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    /// <summary>Prazo de entrega, em horário de Brasília.</summary>
    public DateTime DataEntrega { get; set; }

    public decimal NotaMaxima { get; set; } = 10m;
    public int IdConteudo { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <param name="agoraBrasilia">Data e hora atuais em horário de Brasília.</param>
    public bool PrazoEncerrado(DateTime agoraBrasilia) => agoraBrasilia > DataEntrega;

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new DominioException("O título da atividade é obrigatório.");
        if (NotaMaxima <= 0)
            throw new DominioException("A nota máxima deve ser maior que zero.");
    }
}
