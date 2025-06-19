[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help,

  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Release",

  [Parameter(Mandatory = $false, HelpMessage = "Do not perform build on projects")]
  [Alias("nb")]
  [switch]$noBuild
)

if ($help) {
  Write-Output @"

Initiates a publish of the project to the ".publish/" directory.

Usage: publish.ps1 [-configuration <value>] [-noBuild]

Options:
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Release".
-noBuild|-nb        Skips the build step before publishing.
"@ 
  exit
}

$rootDir = (Get-Item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$projectFile = Join-Path -Path $rootDir -ChildPath $config.webApp.projectFile
$publishDir = "$rootDir\.publish"

if (Test-Path -Path $publishDir) {
  Remove-Item -Recurse -Force $publishDir
}

$noBuildArg = if ($noBuild) { "--no-build" } else { "" }

dotnet publish $projectFile --configuration "$configuration" `
  --no-restore $noBuildArg --output "$publishDir" /p:UseAppHost=false

Write-Host ""
Write-Host "Publish directory contents:" -ForegroundColor Blue
Get-ChildItem "$publishDir"
