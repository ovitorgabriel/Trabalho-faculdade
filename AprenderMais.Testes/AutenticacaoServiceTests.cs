using AprenderMais.Dominio.Excecoes;
using AprenderMais.Infraestrutura.Memoria;
using Xunit;

namespace AprenderMais.Testes;

public class AutenticacaoServiceTests
{
    [Fact]
    public async Task Login_correto_retorna_o_usuario()
    {
        var servico = new Ambiente().CriarAutenticacao();

        var usuario = await servico.EntrarAsync("aluno@aprendermais.com", DadosIniciais.SenhaDemonstracao);

        Assert.Equal("Marina Costa", usuario.Nome);
    }

    [Fact]
    public async Task Email_nao_diferencia_maiusculas_de_minusculas()
    {
        var servico = new Ambiente().CriarAutenticacao();

        var usuario = await servico.EntrarAsync("ADMIN@AprenderMais.com", DadosIniciais.SenhaDemonstracao);

        Assert.Equal("Camila Rocha", usuario.Nome);
    }

    [Fact]
    public async Task Senha_errada_e_rejeitada()
    {
        var servico = new Ambiente().CriarAutenticacao();

        await Assert.ThrowsAsync<DominioException>(() =>
            servico.EntrarAsync("aluno@aprendermais.com", "senha-errada"));
    }

    [Fact]
    public async Task Email_inexistente_e_rejeitado()
    {
        var servico = new Ambiente().CriarAutenticacao();

        await Assert.ThrowsAsync<DominioException>(() =>
            servico.EntrarAsync("ninguem@aprendermais.com", DadosIniciais.SenhaDemonstracao));
    }

    [Fact]
    public async Task Usuario_inativo_nao_entra()
    {
        var servico = new Ambiente().CriarAutenticacao();

        await Assert.ThrowsAsync<DominioException>(() =>
            servico.EntrarAsync("ana@aprendermais.com", DadosIniciais.SenhaDemonstracao));
    }
}
