namespace AprenderMais.Dominio.Entidades;

public class Resposta
{
    public int Id { get; set; }
    public string TextoResposta { get; set; } = string.Empty;
    public string? ArquivoUrl { get; set; }
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    public int IdAtividade { get; set; }
    public int IdAluno { get; set; }
}
