using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Dominio.Entidades;

public class Nota
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public string? Feedback { get; set; }
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;
    public int IdResposta { get; set; }
    public int IdSecretaria { get; set; }

    public void Validar(decimal notaMaximaDaAtividade)
    {
        if (Valor < 0)
            throw new DominioException("A nota não pode ser negativa.");
        if (Valor > notaMaximaDaAtividade)
            throw new DominioException("A nota não pode ser maior que a nota máxima da atividade.");
    }
}
