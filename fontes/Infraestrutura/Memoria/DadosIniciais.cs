using AprenderMais.Aplicacao.Interfaces;
using AprenderMais.Dominio.Entidades;
using AprenderMais.Dominio.Enums;

namespace AprenderMais.Infraestrutura.Memoria;

/// <summary>
/// Dados de demonstração (os mesmos do seed do PIM III). Todos os usuários usam a senha de demonstração.
/// </summary>
public static class DadosIniciais
{
    public const string SenhaDemonstracao = "123456";

    private static DateTime Dt(int ano, int mes, int dia, int hora = 9, int minuto = 0) =>
        new(ano, mes, dia, hora, minuto, 0, DateTimeKind.Utc);

    public static void Popular(BancoEmMemoria banco, IPasswordHasher hasher)
    {
        var hash = hasher.Hash(SenhaDemonstracao);

        // Usuários (a ordem de inserção reproduz os ids do script SQL).
        banco.Usuarios.Add(new Aluno
        {
            Id = banco.ProximoId("usuario"), IdAluno = banco.ProximoId("aluno"),
            Cpf = "123.456.789-10", Nome = "Marina Costa", Email = "aluno@aprendermais.com",
            SenhaHash = hash, Ativo = true, DataCadastro = Dt(2026, 5, 1)
        });
        banco.Usuarios.Add(new Secretaria
        {
            Id = banco.ProximoId("usuario"), IdSecretaria = banco.ProximoId("secretaria"),
            Cpf = "222.333.444-55", Nome = "Rafael Martins", Email = "secretaria@aprendermais.com",
            SenhaHash = hash, Ativo = true, DataCadastro = Dt(2026, 4, 20)
        });
        banco.Usuarios.Add(new Administrador
        {
            Id = banco.ProximoId("usuario"), IdAdmin = banco.ProximoId("admin"),
            Cpf = "999.888.777-66", Nome = "Camila Rocha", Email = "admin@aprendermais.com",
            SenhaHash = hash, Ativo = true, DataCadastro = Dt(2026, 4, 2)
        });
        banco.Usuarios.Add(new Aluno
        {
            Id = banco.ProximoId("usuario"), IdAluno = banco.ProximoId("aluno"),
            Cpf = "555.123.987-00", Nome = "Bruno Lima", Email = "bruno@aprendermais.com",
            SenhaHash = hash, Ativo = true, DataCadastro = Dt(2026, 5, 8)
        });
        banco.Usuarios.Add(new Aluno
        {
            Id = banco.ProximoId("usuario"), IdAluno = banco.ProximoId("aluno"),
            Cpf = "444.321.789-99", Nome = "Ana Souza", Email = "ana@aprendermais.com",
            SenhaHash = hash, Ativo = false, DataCadastro = Dt(2026, 3, 22)
        });

        // Conteúdos (todos cadastrados pela secretaria 1).
        banco.Conteudos.Add(new Conteudo
        {
            Id = banco.ProximoId("conteudo"), Titulo = "Engenharia de Software",
            Descricao = "Requisitos, modelagem, DER, casos de uso, prototipação e qualidade de software.",
            Categoria = "Engenharia", Nivel = "Iniciante", Duracao = "18h", Aulas = 28,
            Ativo = true, DataCriacao = Dt(2026, 5, 4), IdSecretaria = 1
        });
        banco.Conteudos.Add(new Conteudo
        {
            Id = banco.ProximoId("conteudo"), Titulo = "UX/UI Design",
            Descricao = "Arquitetura de informação, usabilidade, acessibilidade e interfaces responsivas.",
            Categoria = "Design", Nivel = "Intermediário", Duracao = "14h", Aulas = 24,
            Ativo = true, DataCriacao = Dt(2026, 5, 9), IdSecretaria = 1
        });
        banco.Conteudos.Add(new Conteudo
        {
            Id = banco.ProximoId("conteudo"), Titulo = "Microsoft 365",
            Descricao = "Produtividade, colaboração, organização de documentos e boas práticas profissionais.",
            Categoria = "Produtividade", Nivel = "Iniciante", Duracao = "10h", Aulas = 18,
            Ativo = true, DataCriacao = Dt(2026, 5, 12), IdSecretaria = 1
        });
        banco.Conteudos.Add(new Conteudo
        {
            Id = banco.ProximoId("conteudo"), Titulo = "Fundamentos de T.I.",
            Descricao = "Base técnica para estudantes iniciantes em tecnologia da informação.",
            Categoria = "Base técnica", Nivel = "Iniciante", Duracao = "12h", Aulas = 21,
            Ativo = false, DataCriacao = Dt(2026, 3, 18), IdSecretaria = 1
        });

        // Atividades (ainda sem tela; entram no próximo passo).
        banco.Atividades.Add(new Atividade
        {
            Id = banco.ProximoId("atividade"), Titulo = "Modelagem de requisitos",
            Descricao = "Relacionar caso de uso, DER e entidades principais do sistema.",
            DataEntrega = new DateTime(2026, 5, 27, 23, 59, 0), NotaMaxima = 10m, IdConteudo = 1
        });
        banco.Atividades.Add(new Atividade
        {
            Id = banco.ProximoId("atividade"), Titulo = "Análise de usabilidade",
            Descricao = "Avaliar navegação, acessibilidade e clareza visual de uma tela.",
            DataEntrega = new DateTime(2026, 5, 29, 23, 59, 0), NotaMaxima = 10m, IdConteudo = 2
        });
        banco.Atividades.Add(new Atividade
        {
            Id = banco.ProximoId("atividade"), Titulo = "Organização com Microsoft 365",
            Descricao = "Estruturar documentos, pastas e materiais de estudo.",
            DataEntrega = new DateTime(2026, 6, 2, 23, 59, 0), NotaMaxima = 10m, IdConteudo = 3
        });

        // Matrículas.
        banco.Matriculas.Add(new Matricula
        {
            Id = banco.ProximoId("matricula"), IdAluno = 1, IdConteudo = 1,
            Status = StatusMatricula.Concluido, Progresso = 100m,
            DataMatricula = Dt(2026, 5, 1, 10), DataConclusao = Dt(2026, 5, 23, 18)
        });
        banco.Matriculas.Add(new Matricula
        {
            Id = banco.ProximoId("matricula"), IdAluno = 1, IdConteudo = 2,
            Status = StatusMatricula.EmAndamento, Progresso = 68m, DataMatricula = Dt(2026, 5, 1, 10)
        });
        banco.Matriculas.Add(new Matricula
        {
            Id = banco.ProximoId("matricula"), IdAluno = 1, IdConteudo = 3,
            Status = StatusMatricula.EmAndamento, Progresso = 42m, DataMatricula = Dt(2026, 5, 1, 10)
        });
        banco.Matriculas.Add(new Matricula
        {
            Id = banco.ProximoId("matricula"), IdAluno = 2, IdConteudo = 1,
            Status = StatusMatricula.EmAndamento, Progresso = 35m, DataMatricula = Dt(2026, 5, 8, 10)
        });
    }
}
