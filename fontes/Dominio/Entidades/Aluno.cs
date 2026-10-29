using AprenderMais.Dominio.Enums;

namespace AprenderMais.Dominio.Entidades;

public class Aluno : Usuario
{
    /// <summary>Chave da tabela aluno (id_aluno), diferente do Id do usuário.</summary>
    public int IdAluno { get; set; }

    public override PerfilUsuario Perfil => PerfilUsuario.Aluno;
}
