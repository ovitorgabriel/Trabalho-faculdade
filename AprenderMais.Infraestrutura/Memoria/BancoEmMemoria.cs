using AprenderMais.Dominio.Entidades;

namespace AprenderMais.Infraestrutura.Memoria;

/// <summary>
/// "Banco de dados" temporário: guarda tudo em listas enquanto o programa está aberto.
/// Será substituído pelos repositórios de SQL Server sem mudar o resto do sistema.
/// </summary>
public class BancoEmMemoria
{
    private readonly Dictionary<string, int> _contadores = new();

    public List<Usuario> Usuarios { get; } = new();
    public List<Conteudo> Conteudos { get; } = new();
    public List<Atividade> Atividades { get; } = new();
    public List<Matricula> Matriculas { get; } = new();

    /// <summary>Simula o IDENTITY do banco: devolve o próximo id da "tabela".</summary>
    public int ProximoId(string tabela)
    {
        _contadores.TryGetValue(tabela, out var atual);
        atual++;
        _contadores[tabela] = atual;
        return atual;
    }
}
