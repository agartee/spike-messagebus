[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help,

  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false, HelpMessage = "The target migration to apply. If not specified, all pending migrations will be applied.")]
  [Alias("m")]
  [string]$migrationName,

  [Parameter(Mandatory = $false, HelpMessage = "Name of the database connection string to apply migrations (from the .NET project's user secrets)")]
  [Alias("n")]
  [string]$connectionStringName = "database"
)

if ($help) {
  Write-Output @"

Runs the Entity Framework migrations for the application database.

Usage: migrate-database.ps1 [-configuration <value>] [-connectionStringName <value>] [-migrationName <value>]

Options:
-configuration|-c         Specifies the configuration name for the build. 
                          Common values are "Release" or "Debug". If not 
                          specified, the default is "Debug".
-connectionStringName|-n  Specifies the connection string name from the
                          .NET project's user secrets. If not specified, 
                          the default is "database".
-migrationName|-m         Specifies the name of the migration to deploy to
                          the target database. If not specified, the latest
                          migration will be used. To rollback all migrations,
                          pass "0".
                          
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$webAppProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile
$migrationsProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.dbMigrations.projectFile

if (-not (Test-Path -Path $webAppProjectFile -PathType Leaf)) {
  Write-Error "Web application project file not found at path: $webAppProjectFile"
  exit 1
}

if (-not (Test-Path -Path $migrationsProjectFile -PathType Leaf)) {
  Write-Error "Migrations project file not found at path: $migrationsProjectFile"
  exit 1
}

function GetConnectionStringFromUserSecrets() {
  $secretsList = dotnet user-secrets list --project $webAppProjectFile
  if (-not $secretsList) {
      return $null;
  }
  
  $secrets = @{}
  $secretsList -split "`n" | ForEach-Object {
      $parts = $_ -split '=', 2
      if ($parts.Count -eq 2) {
          $key = $parts[0].Trim()
          $value = $parts[1].Trim()
          $secrets[$key] = $value
      }
  }

  $result = $secrets["connectionStrings:$($connectionStringName)"];
  
  return $result;
}

function GetConnectionStringFromEnvironment() {
  $envVars = [System.Environment]::GetEnvironmentVariables()
  $envDict = @{}
  foreach ($key in $envVars.Keys) {
      $upperKey = $key.ToUpper()
      $envDict[$upperKey] = $envVars[$key]
  }

  $result = $envDict["CONNECTIONSTRINGS:$($connectionStringName.ToUpper())"]

  if(-not $result) {
    $result = $envDict["CONNECTIONSTRINGS__$($connectionStringName.ToUpper())"];
  }

  return $result;
}

$connectionString = GetConnectionStringFromUserSecrets;
if(-not $connectionString) {
  $connectionString = GetConnectionStringFromEnvironment;
}
if (-not $connectionString) {
  Write-Error "Database connection string not found in user secrets or environment variables."
  exit 1
}

dotnet ef database update `
    --configuration $configuration `
    --connection $connectionString `
    --startup-project $webAppProjectFile `
    --project $migrationsProjectFile `
    $migrationName
