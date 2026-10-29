using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Aplicacao.Servicos;

public class MatriculaService
{
    private readonly IMatriculaRepositorio _matriculas;
    private readonly IConteudoRepositorio _conteudos;
    private readonly IRelogio _relogio;

    public MatriculaService(IMatriculaRepositorio matriculas, IConteudoRepositorio conteudos, IRelogio relogio)
    {
        _matriculas = matriculas;
        _conteudos = conteudos;
        _relogio = relogio;
    }

    public async Task<MatriculaDto> MatricularAsync(int idConteudo, Usuario solicitante)
    {
        var aluno = ExigirAluno(solicitante);

        var conteudo = await _conteudos.ObterPorIdAsync(idConteudo);
        if (conteudo is null || !conteudo.Ativo)
            throw new DominioException("Conteúdo inexistente ou inativo.");

        var existente = await _matriculas.ObterPorAlunoEConteudoAsync(aluno.IdAluno, idConteudo);
        if (existente is not null)
        {
            if (existente.Status != StatusMatricula.Cancelado)
                throw new DominioException("Você já está matriculado neste conteúdo.");

            existente.Reativar(_relogio.AgoraUtc);
            await _matriculas.AtualizarAsync(existente);
            return ParaDto(existente, conteudo.Titulo);
        }

        var nova = await _matriculas.AdicionarAsync(new Matricula
        {
            IdAluno = aluno.IdAluno,
            IdConteudo = idConteudo,
            DataMatricula = _relogio.AgoraUtc
        });
        return ParaDto(nova, conteudo.Titulo);
    }

    public async Task<IReadOnlyList<MatriculaDto>> ListarDoAlunoAsync(Usuario solicitante)
    {
        var aluno = ExigirAluno(solicitante);

        var matriculas = await _matriculas.ListarPorAlunoAsync(aluno.IdAluno);
        var resultado = new List<MatriculaDto>();

        foreach (var matricula in matriculas)
        {
            var conteudo = await _conteudos.ObterPorIdAsync(matricula.IdConteudo);
            resultado.Add(ParaDto(matricula, conteudo?.Titulo ?? "(conteúdo removido)"));
        }

        return resultado;
    }

    public async Task CancelarAsync(int idMatricula, Usuario solicitante)
    {
        var aluno = ExigirAluno(solicitante);

        var matricula = await _matriculas.ObterPorIdAsync(idMatricula);
        if (matricula is null || matricula.IdAluno != aluno.IdAluno)
            throw new DominioException("Matrícula não encontrada.");

        matricula.Cancelar();
        await _matriculas.AtualizarAsync(matricula);
    }

    private static Aluno ExigirAluno(Usuario solicitante)
    {
        if (solicitante is not Aluno aluno)
            throw new DominioException("Esta operação é exclusiva de alunos.");
        return aluno;
    }

    private MatriculaDto ParaDto(Matricula m, string tituloConteudo) =>
        new(
            m.Id,
            tituloConteudo,
            RotuloStatus(m.Status),
            m.Progresso,
            _relogio.ParaBrasilia(m.DataMatricula).ToString("dd/MM/yyyy"));

    private static string RotuloStatus(StatusMatricula status) => status switch
    {
        StatusMatricula.EmAndamento => "Em andamento",
        StatusMatricula.Concluido => "Concluída",
        StatusMatricula.Cancelado => "Cancelada",
        _ => status.ToString()
    };
}
