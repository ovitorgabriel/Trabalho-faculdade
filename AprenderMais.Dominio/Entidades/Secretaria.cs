using AprenderMais.Dominio.Enums;

namespace AprenderMais.Dominio.Entidades;

public class Secretaria : Usuario
{
    /// <summary>Chave da tabela secretaria (id_secretaria).</summary>
    public int IdSecretaria { get; set; }

    public override PerfilUsuario Perfil => PerfilUsuario.Secretaria;
}
