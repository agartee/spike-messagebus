param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false, HelpMessage = "Docker network to connect the container to")]
  [Alias("n")]
  [string]$network
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$imageName = $config.docker.imageName
$containerName = $config.docker.containerName
$tagName = $config.docker.tagName
$userSecretsId = $config.userSecretsId
$webAppName = $config.webApp.name;

docker container rm "$containerName" --force 2>&1 | Out-Null

$pfxFile = $config.docker.pfxFile
$pfxPath = "$env:APPDATA\ASP.NET\Https\$pfxFile"
$pfxDir = Split-Path -Path "$pfxPath" -Parent
$pfxPassword = $config.docker.pfxPassword
$secretsDir = "$env:APPDATA\Microsoft\UserSecrets\$userSecretsId"

if ($network) {
  $exists = docker network ls --format '{{.Name}}' | Where-Object { $_ -eq $network }
  if (-not $exists) {
    Write-Error "Docker network '$network' does not exist."
    return 1
  }
}

$dockerArgs = @()

if ($network) {
  $dockerArgs += "--network"
  $dockerArgs += $network
}

$dockerArgs += @(
  "--name", $containerName,
  "--publish", "5000:8080",
  "--publish", "5001:8081"
  "--env", "ASPNETCORE_ENVIRONMENT=Development",
  "--env", "ASPNETCORE_URLS=https://+:8081;http://+:8080",
  "--env", "HTTPS_PORT=5001",
  "--env", "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/$($pfxFile)",
  "--env", "ASPNETCORE_Kestrel__Certificates__Default__Password=$($pfxPassword)",
  "--volume", "$($secretsDir):/home/app/.microsoft/usersecrets/$($userSecretsId):ro",
  "--volume", "$($pfxDir):/https:ro",
  "--entrypoint", "dotnet",
  "--detach",
  "$($imageName):$($tagName)",
  "/app/$($webAppName).dll"
)

docker container run @dockerArgs

if ($LASTEXITCODE -eq 0) {
  & "$rootDir\scripts\support\wait-for-healthy-container.ps1" -c $containerName
}
