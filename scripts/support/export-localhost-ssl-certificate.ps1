$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$pfxFile = $config.docker.pfxFile
$pfxPath = "$env:APPDATA\ASP.NET\Https\$pfxFile"
$pfxPassword = $config.docker.pfxPassword
$certName = "localhost"

if (Test-Path $pfxPath) {
    return
}

$cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {
    $_.Subject -eq "CN=$certName" -and $_.HasPrivateKey
} | Sort-Object NotBefore -Descending | Select-Object -First 1

if (-not $cert) {
    Write-Warning "No matching 'localhost' certificate with a private key was found in the CurrentUser\My store."
    return
}

$securePassword = ConvertTo-SecureString -String $pfxPassword -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword > $null 2>&1
