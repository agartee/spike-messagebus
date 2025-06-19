param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$projectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile

dotnet run --project $projectFile --launch-profile https --configuration $configuration --no-build
