using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Desktop.Formularios;
using AprenderMais.Infraestrutura.Memoria;
using AprenderMais.Infraestrutura.Relogio;
using AprenderMais.Infraestrutura.Seguranca;

namespace AprenderMais.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // ---- Raiz de composição: único lugar que conhece as implementações concretas. ----
        // Quando o SQL Server entrar, só as linhas dos repositórios abaixo mudam.
        IPasswordHasher hasher = new BCryptPasswordHasher();
        IRelogio relogio = new RelogioSistema();

        var banco = new BancoEmMemoria();
        DadosIniciais.Popular(banco, hasher);

        var usuarios = new UsuarioRepositorioMemoria(banco);
        var conteudos = new ConteudoRepositorioMemoria(banco);
        var matriculas = new MatriculaRepositorioMemoria(banco);

        var sessao = new SessaoUsuario();
        var autenticacao = new AutenticacaoService(usuarios, hasher, sessao);
        var conteudoService = new ConteudoService(conteudos);
        var matriculaService = new MatriculaService(matriculas, conteudos, relogio);
        var usuarioService = new UsuarioService(usuarios);

        // Login -> janela principal -> (Sair) -> login de novo, até o usuário fechar.
        while (true)
        {
            using (var login = new LoginForm(autenticacao))
            {
                if (login.ShowDialog() != DialogResult.OK)
                    break;
            }

            using var principal = new PrincipalForm(sessao, conteudoService, matriculaService, usuarioService);
            Application.Run(principal);

            autenticacao.Sair();

            if (!principal.SairDaConta)
                break;
        }
    }
}
