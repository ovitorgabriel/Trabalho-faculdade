using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Excecoes;
using Xunit;

namespace AprenderMais.Testes;

public class RegrasDeDominioTests
{
    [Fact]
    public void Nota_acima_da_maxima_e_rejeitada()
    {
        var nota = new Nota { Valor = 10.5m };

        Assert.Throws<DominioException>(() => nota.Validar(10m));
    }

    [Fact]
    public void Nota_dentro_do_limite_e_aceita()
    {
        var nota = new Nota { Valor = 8.7m };

        nota.Validar(10m);
    }

    [Fact]
    public void Prazo_da_atividade_encerra_depois_da_data_de_entrega()
    {
        var atividade = new Atividade { DataEntrega = new DateTime(2026, 5, 27, 23, 59, 0) };

        Assert.False(atividade.PrazoEncerrado(new DateTime(2026, 5, 27, 20, 0, 0)));
        Assert.True(atividade.PrazoEncerrado(new DateTime(2026, 5, 28, 0, 1, 0)));
    }

    [Fact]
    public void Conteudo_sem_titulo_e_invalido()
    {
        var conteudo = new Conteudo { Titulo = "  ", Categoria = "Design" };

        Assert.Throws<DominioException>(() => conteudo.Validar());
    }
}
