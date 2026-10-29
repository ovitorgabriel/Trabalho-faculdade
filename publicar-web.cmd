@echo off
rem Gera o site estatico (Blazor WebAssembly) na pasta dist\, que a Vercel publica.
rem Rode depois de qualquer alteracao no codigo e faca commit da pasta dist.
if exist dist rmdir /s /q dist
dotnet publish AprenderMais.Web\AprenderMais.Web.csproj -c Release -o publish-tmp || exit /b 1
xcopy /E /I /Y publish-tmp\wwwroot dist >nul
rmdir /s /q publish-tmp
echo Pronto: dist\ atualizado.
