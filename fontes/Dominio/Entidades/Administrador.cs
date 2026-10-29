using AprenderMais.Dominio.Enums;

namespace AprenderMais.Dominio.Entidades;

public class Administrador : Usuario
{
    /// <summary>Chave da tabela admin (id_admin).</summary>
    public int IdAdmin { get; set; }

    public override PerfilUsuario Perfil => PerfilUsuario.Admin;
}
