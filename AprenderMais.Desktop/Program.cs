
using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Aplicacao.Servicos;
using AprenderMais.Aplicacao.Sessao;
using AprenderMais.Desktop.Formularios;
using AprenderMais.Infraestrutura.Memoria;
using AprenderMais.Infraestrutura.Relogio;
using AprenderMais.Infraestrutura.Seguranca;
using System;
using System.Windows.Forms;

namespace AprenderMais.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            ExecutarAplicacao();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ocorreu um erro inesperado ao executar o AprenderMais.\n\n" +
                $"Detalhes: {ex.Message}",
                "Erro na aplicação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static void ExecutarAplicacao()
    {
        // Raiz de composição: centraliza a criação das dependências.
        // Ao migrar para o SQL Server, os repositórios poderão ser substituídos aqui.

        IPasswordHasher hasher = new BCryptPasswordHasher();
        IRelogio relogio = new RelogioSistema();

        var banco = new BancoEmMemoria();
        DadosIniciais.Popular(banco, hasher);

        var usuarios = new UsuarioRepositorioMemoria(banco);

        var conteudos = new ConteudoRepositorioMemoria(banco);
        var matriculas = new MatriculaRepositorioMemoria(banco);

        var sessao = new SessaoUsuario();

        var autenticacao = new AutenticacaoService(
            usuarios,
            hasher,
            sessao);

        var conteudoService = new ConteudoService(conteudos);

        var matriculaService = new MatriculaService(
            matriculas,
            conteudos,
            relogio);

        var usuarioService = new UsuarioService(
            usuarios,
            hasher);

        // Permite voltar ao login após sair da conta.
        // O aplicativo encerra quando o usuário fecha o login
        // ou escolhe sair da conta na janela principal.
        while (true)
        {
            using (var login = new LoginForm(autenticacao))
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    break;
                }
            }

            bool sairDaConta;

            using (var principal = new PrincipalForm(
                sessao,
                conteudoService,
                matriculaService,
                usuarioService))
            {
                Application.Run(principal);
                sairDaConta = principal.SairDaConta;
            }

            autenticacao.Sair();

            if (!sairDaConta)
            {
                break;
            }
        }
    }
}
