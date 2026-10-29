using AprenderMais.Aplicacao.Interfaces;

namespace AprenderMais.Infraestrutura.Seguranca;

/// <summary>
/// Hash de senha com BCrypt (pacote BCrypt.Net-Next). A senha nunca é guardada em texto puro.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch (Exception)
        {
            // Hash em formato inválido: trata como senha incorreta.
            return false;
        }
    }
}
