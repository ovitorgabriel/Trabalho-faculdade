using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Infraestrutura.Memoria;
using AprenderMais.Infraestrutura.Relogio;
using AprenderMais.Infraestrutura.Seguranca;
using AprenderMais.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Raiz de composicao da versao web. Os dados ficam em memoria no navegador
// (reiniciam ao recarregar a pagina), igual a versao desktop.
IPasswordHasher hasher = new BCryptPasswordHasher();
var banco = new BancoEmMemoria();
DadosIniciais.Popular(banco, hasher);

var usuarios = new UsuarioRepositorioMemoria(banco);
var conteudos = new ConteudoRepositorioMemoria(banco);
var matriculas = new MatriculaRepositorioMemoria(banco);
var sessao = new SessaoUsuario();

builder.Services.AddSingleton(sessao);
builder.Services.AddSingleton(new AutenticacaoService(usuarios, hasher, sessao));
builder.Services.AddSingleton(new ConteudoService(conteudos));
builder.Services.AddSingleton(new MatriculaService(matriculas, conteudos, new RelogioSistema()));
builder.Services.AddSingleton(new UsuarioService(usuarios, hasher));

await builder.Build().RunAsync();
