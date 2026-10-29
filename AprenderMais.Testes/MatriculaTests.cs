using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;
using Xunit;

namespace AprenderMais.Testes;

public class MatriculaTests
{
    private static readonly DateTime Agora = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Progresso_parcial_mantem_matricula_em_andamento()
    {
        var matricula = new Matricula();

        matricula.AtualizarProgresso(1, 4, Agora);

        Assert.Equal(25m, matricula.Progresso);
        Assert.Equal(StatusMatricula.EmAndamento, matricula.Status);
        Assert.Null(matricula.DataConclusao);
    }

    [Fact]
    public void Todas_as_atividades_avaliadas_concluem_a_matricula()
    {
        var matricula = new Matricula();

        matricula.AtualizarProgresso(3, 3, Agora);

        Assert.Equal(100m, matricula.Progresso);
        Assert.Equal(StatusMatricula.Concluido, matricula.Status);
        Assert.Equal(Agora, matricula.DataConclusao);
    }

    [Fact]
    public void Matricula_cancelada_nao_tem_progresso_atualizado()
    {
        var matricula = new Matricula { Status = StatusMatricula.Cancelado };

        matricula.AtualizarProgresso(2, 2, Agora);

        Assert.Equal(0m, matricula.Progresso);
        Assert.Equal(StatusMatricula.Cancelado, matricula.Status);
    }

    [Fact]
    public void Matricula_concluida_nao_pode_ser_cancelada()
    {
        var matricula = new Matricula { Status = StatusMatricula.Concluido };

        Assert.Throws<DominioException>(() => matricula.Cancelar());
    }

    [Fact]
    public void Reativar_zera_o_progresso()
    {
        var matricula = new Matricula { Status = StatusMatricula.Cancelado, Progresso = 40m };

        matricula.Reativar(Agora);

        Assert.Equal(StatusMatricula.EmAndamento, matricula.Status);
        Assert.Equal(0m, matricula.Progresso);
    }
}
