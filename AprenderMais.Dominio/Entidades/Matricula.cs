using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;

namespace AprenderMais.Dominio.Entidades;

public class Matricula
{
    public int Id { get; set; }
    public int IdAluno { get; set; }
    public int IdConteudo { get; set; }
    public StatusMatricula Status { get; set; } = StatusMatricula.EmAndamento;
    public decimal Progresso { get; set; }
    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
    public DateTime? DataConclusao { get; set; }

    public void Cancelar()
    {
        if (Status == StatusMatricula.Concluido)
            throw new DominioException("Uma matrícula concluída não pode ser cancelada.");
        if (Status == StatusMatricula.Cancelado)
            throw new DominioException("Esta matrícula já está cancelada.");
        Status = StatusMatricula.Cancelado;
    }

    public void Reativar(DateTime agoraUtc)
    {
        if (Status != StatusMatricula.Cancelado)
            throw new DominioException("Somente matrículas canceladas podem ser reativadas.");
        Status = StatusMatricula.EmAndamento;
        Progresso = 0m;
        DataConclusao = null;
        DataMatricula = agoraUtc;
    }

    /// <summary>
    /// Mesma regra do trigger do banco: progresso = % de atividades avaliadas.
    /// Ao chegar em 100% a matrícula é concluída.
    /// </summary>
    public void AtualizarProgresso(int atividadesAvaliadas, int totalAtividades, DateTime agoraUtc)
    {
        if (Status == StatusMatricula.Cancelado || totalAtividades <= 0)
            return;

        Progresso = Math.Round(100m * atividadesAvaliadas / totalAtividades, 2);

        if (Progresso >= 100m)
        {
            Progresso = 100m;
            Status = StatusMatricula.Concluido;
            DataConclusao ??= agoraUtc;
        }
    }
}
