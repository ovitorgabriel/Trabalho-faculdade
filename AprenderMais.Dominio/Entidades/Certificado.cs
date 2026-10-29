namespace AprenderMais.Dominio.Entidades;

public class Certificado
{
    public int Id { get; set; }
    public int IdAluno { get; set; }
    public int IdConteudo { get; set; }
    public string CodigoValidacao { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
}
