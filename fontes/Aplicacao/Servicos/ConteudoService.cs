using AprenderMais.Aplicacao.Dtos;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Excecoes;
using AprenderMais.Dominio.Repositorios;

namespace AprenderMais.Aplicacao.Servicos;

public class ConteudoService
{
    private readonly IConteudoRepositorio _conteudos;

    public ConteudoService(IConteudoRepositorio conteudos)
    {
        _conteudos = conteudos;
    }

    /// <summary>Catálogo visível para todos os perfis.</summary>
    public async Task<IReadOnlyList<ConteudoDto>> ListarAtivosAsync()
    {
        var lista = await _conteudos.ListarAsync(somenteAtivos: true);
        return lista.Select(ParaDto).ToList();
    }

    /// <summary>Todos os conteúdos, inclusive inativos (gestão da secretaria).</summary>
    public async Task<IReadOnlyList<ConteudoDto>> ListarTodosAsync(Usuario solicitante)
    {
        ExigirSecretaria(solicitante);
        var lista = await _conteudos.ListarAsync(somenteAtivos: false);
        return lista.Select(ParaDto).ToList();
    }

    public async Task<ConteudoDto> CriarAsync(NovoConteudoDto dto, Usuario solicitante)
    {
        var secretaria = ExigirSecretaria(solicitante);

        var conteudo = new Conteudo
        {
            Titulo = (dto.Titulo ?? string.Empty).Trim(),
            Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao.Trim(),
            Categoria = (dto.Categoria ?? string.Empty).Trim(),
            Nivel = string.IsNullOrWhiteSpace(dto.Nivel) ? "Iniciante" : dto.Nivel.Trim(),
            Duracao = string.IsNullOrWhiteSpace(dto.Duracao) ? "0h" : dto.Duracao.Trim(),
            Aulas = dto.Aulas,
            IdSecretaria = secretaria.IdSecretaria
        };
        conteudo.Validar();

        var salvo = await _conteudos.AdicionarAsync(conteudo);
        return ParaDto(salvo);
    }

    public async Task AlterarAtivoAsync(int idConteudo, bool ativo, Usuario solicitante)
    {
        ExigirSecretaria(solicitante);

        var conteudo = await _conteudos.ObterPorIdAsync(idConteudo)
            ?? throw new DominioException("Conteúdo não encontrado.");

        conteudo.Ativo = ativo;
        await _conteudos.AtualizarAsync(conteudo);
    }

    private static Secretaria ExigirSecretaria(Usuario solicitante)
    {
        if (solicitante is not Secretaria secretaria)
            throw new DominioException("Somente a secretaria pode gerenciar conteúdos.");
        return secretaria;
    }

    private static ConteudoDto ParaDto(Conteudo c) =>
        new(c.Id, c.Titulo, c.Categoria, c.Nivel, c.Duracao, c.Aulas, c.Ativo);
}
