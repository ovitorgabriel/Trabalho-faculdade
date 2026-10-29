using AprenderMais.Dominio.Enums;

namespace AprenderMais.Dominio.Entidades;

/// <summary>
/// Classe base dos três perfis do sistema (tabela usuario).
/// </summary>
public abstract class Usuario
{
    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public abstract PerfilUsuario Perfil { get; }
}
