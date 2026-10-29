# Aprender Mais - PIM IV (versão desktop em C# / .NET)

Plataforma educacional com três perfis: **Aluno**, **Secretaria** e **Administrador**.
Esta é a primeira entrega do PIM IV: o sistema desktop funcionando, em camadas,
pronto para receber o banco SQL Server e as APIs depois.

## Como montar e rodar

Requisitos: Windows, .NET 10 SDK e (opcional) Visual Studio com a carga
"Desenvolvimento para desktop com .NET".

1. Extraia esta pasta e abra um terminal dentro dela.
2. Rode `criar-solucao.cmd`. Ele cria a solução, copia o código, liga as camadas,
   instala o BCrypt, compila e executa os testes.
3. Abra o sistema: `dotnet run --project AprenderMais.Desktop\AprenderMais.Desktop.csproj`
   (ou abra `AprenderMais.slnx` / `AprenderMais.sln` no Visual Studio e execute o projeto Desktop).

## Contas de demonstração (senha `123456`)

| Perfil | E-mail |
|---|---|
| Aluno | aluno@aprendermais.com (também bruno@aprendermais.com) |
| Secretaria | secretaria@aprendermais.com |
| Administrador | admin@aprendermais.com |
| Aluno inativo (não consegue entrar) | ana@aprendermais.com |

## Arquitetura

```
AprenderMais.Dominio          entidades, regras de negócio e contratos de repositório
AprenderMais.Aplicacao        serviços (casos de uso), DTOs, sessão
AprenderMais.Infraestrutura   repositórios em memória, BCrypt, relógio
AprenderMais.Desktop          Windows Forms (login e telas por perfil)
AprenderMais.Testes           xUnit
```

Dependências apontam sempre para dentro: Desktop -> Aplicação -> Domínio.
A Infraestrutura implementa as interfaces definidas no Domínio.

Por enquanto os dados ficam **em memória** (reiniciam a cada execução, com os dados
de demonstração do PIM III). Quando o SQL Server entrar, basta criar repositórios
`...RepositorioSql` que implementem as mesmas interfaces e trocar as linhas
correspondentes em `Program.cs`. Telas, serviços e regras não mudam.

## O que já funciona

- Login com senha em hash (BCrypt), bloqueio de usuário inativo.
- Todos: catálogo de conteúdos ativos.
- Aluno: matricular-se, ver e cancelar matrículas (reativa se matricular de novo).
- Secretaria: cadastrar conteúdo, ativar/desativar conteúdo.
- Administrador: listar usuários, ativar/desativar (não desativa a si mesmo).

## Próximos passos

Atividades, entrega de respostas, avaliação com nota, progresso, certificados e
relatórios; cadastro de usuários; depois SQL Server (Azure) e a API ASP.NET Core.
