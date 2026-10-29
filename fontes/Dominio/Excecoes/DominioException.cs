namespace AprenderMais.Dominio.Excecoes;

/// <summary>
/// Violação de uma regra de negócio. A mensagem é escrita para ser exibida ao usuário.
/// </summary>
public class DominioException : Exception
{
    public DominioException(string mensagem) : base(mensagem)
    {
    }
}
