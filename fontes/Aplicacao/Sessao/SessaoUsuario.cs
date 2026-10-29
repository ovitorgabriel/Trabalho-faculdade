using AprenderMais.Dominio.Entidades;

namespace AprenderMais.Aplicacao.Sessao;

/// <summary>
/// Guarda o usuário autenticado enquanto o programa está aberto.
/// </summary>
public class SessaoUsuario
{
    public Usuario? Atual { get; private set; }

    public bool EstaAutenticado => Atual is not null;

    public void Entrar(Usuario usuario) => Atual = usuario;

    public void Sair() => Atual = null;
}
