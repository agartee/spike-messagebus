[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'Help')]
  [Alias("h")]
  [switch]$help,
  
  [Parameter(Mandatory = $true, HelpMessage = "Migration name")]
  [Alias("m")]
  [string]$migrationName
)

if ($help) {
  Write-Output @"

Creates a new Entity Framework migration for the application database.

Usage: migrate-database.ps1 [-migrationName <value>]

Options:
-migrationName|-m   Specifies the name of the migration to create.

"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$webAppProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile
$migrationsProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.dbMigrations.projectFile
$dbContextName = $config.dbMigrations.dbContextName

if (-not (Test-Path -Path $webAppProjectFile -PathType Leaf)) {
  Write-Error "Web application project file not found at path: $webAppProjectFile"
  exit 1
}

if (-not (Test-Path -Path $migrationsProjectFile -PathType Leaf)) {
  Write-Error "Migrations project file not found at path: $migrationsProjectFile"
  exit 1
}

dotnet ef migrations add $migrationName `
  --startup-project $webAppProjectFile `
  --project $migrationsProjectFile `
  --context $dbContextName
