namespace AprenderMais.Aplicacao.Interfaces;

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}
