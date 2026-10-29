@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
  echo [ERRO] .NET SDK nao encontrado. Instale o .NET 10 SDK e rode de novo.
  exit /b 1
)
echo SDK em uso:
dotnet --version

if exist AprenderMais.Dominio (
  echo [ERRO] As pastas da solucao ja existem. Rode este script em uma pasta limpa.
  exit /b 1
)

echo.
echo [1/6] Criando a solucao e os projetos...
dotnet new sln -n AprenderMais || goto erro
dotnet new classlib -n AprenderMais.Dominio -f net10.0 || goto erro
dotnet new classlib -n AprenderMais.Aplicacao -f net10.0 || goto erro
dotnet new classlib -n AprenderMais.Infraestrutura -f net10.0 || goto erro
dotnet new winforms -n AprenderMais.Desktop -f net10.0 || goto erro
dotnet new xunit -n AprenderMais.Testes -f net10.0 || goto erro

echo.
echo [2/6] Copiando o codigo-fonte...
del /q AprenderMais.Dominio\Class1.cs 2>nul
del /q AprenderMais.Aplicacao\Class1.cs 2>nul
del /q AprenderMais.Infraestrutura\Class1.cs 2>nul
del /q AprenderMais.Desktop\Form1.cs 2>nul
del /q AprenderMais.Desktop\Form1.Designer.cs 2>nul
del /q AprenderMais.Desktop\Form1.resx 2>nul
del /q AprenderMais.Desktop\Program.cs 2>nul
del /q AprenderMais.Testes\UnitTest1.cs 2>nul

xcopy /e /i /y "fontes\Dominio" "AprenderMais.Dominio" >nul || goto erro
xcopy /e /i /y "fontes\Aplicacao" "AprenderMais.Aplicacao" >nul || goto erro
xcopy /e /i /y "fontes\Infraestrutura" "AprenderMais.Infraestrutura" >nul || goto erro
xcopy /e /i /y "fontes\Desktop" "AprenderMais.Desktop" >nul || goto erro
xcopy /e /i /y "fontes\Testes" "AprenderMais.Testes" >nul || goto erro

echo.
echo [3/6] Adicionando os projetos a solucao...
dotnet sln add AprenderMais.Dominio\AprenderMais.Dominio.csproj AprenderMais.Aplicacao\AprenderMais.Aplicacao.csproj AprenderMais.Infraestrutura\AprenderMais.Infraestrutura.csproj AprenderMais.Desktop\AprenderMais.Desktop.csproj AprenderMais.Testes\AprenderMais.Testes.csproj || goto erro

echo.
echo [4/6] Ligando as camadas (referencias)...
dotnet add AprenderMais.Aplicacao\AprenderMais.Aplicacao.csproj reference AprenderMais.Dominio\AprenderMais.Dominio.csproj || goto erro
dotnet add AprenderMais.Infraestrutura\AprenderMais.Infraestrutura.csproj reference AprenderMais.Dominio\AprenderMais.Dominio.csproj AprenderMais.Aplicacao\AprenderMais.Aplicacao.csproj || goto erro
dotnet add AprenderMais.Desktop\AprenderMais.Desktop.csproj reference AprenderMais.Dominio\AprenderMais.Dominio.csproj AprenderMais.Aplicacao\AprenderMais.Aplicacao.csproj AprenderMais.Infraestrutura\AprenderMais.Infraestrutura.csproj || goto erro
dotnet add AprenderMais.Testes\AprenderMais.Testes.csproj reference AprenderMais.Dominio\AprenderMais.Dominio.csproj AprenderMais.Aplicacao\AprenderMais.Aplicacao.csproj AprenderMais.Infraestrutura\AprenderMais.Infraestrutura.csproj || goto erro

echo.
echo [5/6] Instalando o pacote de hash de senha (BCrypt)...
dotnet add AprenderMais.Infraestrutura\AprenderMais.Infraestrutura.csproj package BCrypt.Net-Next || goto erro

echo.
echo [6/6] Compilando e rodando os testes...
dotnet build || goto erro
dotnet test AprenderMais.Testes\AprenderMais.Testes.csproj || goto erro

echo.
echo Tudo certo! Para abrir o sistema:
echo   dotnet run --project AprenderMais.Desktop\AprenderMais.Desktop.csproj
echo ou abra a solucao no Visual Studio e execute o projeto AprenderMais.Desktop.
exit /b 0

:erro
echo.
echo [ERRO] Um passo falhou. Copie as mensagens acima e envie para a gente corrigir.
exit /b 1
