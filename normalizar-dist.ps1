# Converte os .js de dist\_framework para LF e atualiza os hashes de integridade em dist\index.html.
# Necessario porque a Vercel serve os arquivos de texto sem os CR, o que invalidaria o hash gerado pelo .NET.
$dist = Join-Path $PSScriptRoot 'dist'
$indexPath = Join-Path $dist 'index.html'
$utf8 = New-Object Text.UTF8Encoding($false)
$index = [IO.File]::ReadAllText($indexPath)
$sha = [Security.Cryptography.SHA256]::Create()

foreach ($arquivo in Get-ChildItem (Join-Path $dist '_framework') -Filter *.js) {
    $bytes = [IO.File]::ReadAllBytes($arquivo.FullName)
    if ($bytes -notcontains 13) { continue }

    $antigo = 'sha256-' + [Convert]::ToBase64String($sha.ComputeHash($bytes))
    $texto = [IO.File]::ReadAllText($arquivo.FullName).Replace("`r`n", "`n").Replace("`r", "`n")
    [IO.File]::WriteAllText($arquivo.FullName, $texto, $utf8)
    $novo = 'sha256-' + [Convert]::ToBase64String($sha.ComputeHash([IO.File]::ReadAllBytes($arquivo.FullName)))

    $index = $index.Replace($antigo, $novo)
    Write-Host "Normalizado: $($arquivo.Name)"
}

[IO.File]::WriteAllText($indexPath, $index, $utf8)
