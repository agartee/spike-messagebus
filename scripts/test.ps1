[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help,

  [Parameter(Mandatory = $false, HelpMessage = 'Open coverage report')]
  [Alias("o")]
  [switch]$openCoverageReport,

  [Parameter(Mandatory = $false, HelpMessage = 'Do not perform build on projects')]
  [Alias("nb")]
  [switch]$noBuild
)

if ($help) {
  Write-Output @"
  
Runs tests and generates code coverage reports (Cobertura XML and HTML) across
all test projects.

Usage: test.ps1 [-openCoverageReport] [-noBuild]

Options:
  --openCoverageReport|-o   Open the generated HTML coverage report in the 
                            default browser.
  --noBuild|-nb             Skips the build step during test execution.

"@
  exit
}

$rootDir = (Get-Item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$solutionFile = Join-Path "$rootDir" $config.solutionFile
$reportOutputDir = Join-Path "$rootDir" "coverage-report"

. "$rootDir\scripts\support\delete-coverage-files.ps1"

Write-Host "Running tests with code coverage collection..."

$noBuildArg = if ($noBuild) { "--no-build" } else { "" }

dotnet test $solutionFile --settings "$rootDir\.config\coverlet.runsettings" $noBuildArg

if ($LASTEXITCODE -ne 0) {
  Write-Error "Tests failed. Aborting code coverage report generation."
  exit 1
}

$reportGlob = Join-Path "$rootDir" "**\coverage.cobertura.xml"

dotnet tool run reportgenerator `
  "-reports:$reportGlob" `
  "-targetdir:$reportOutputDir" `
  "-reporttypes:Html;Cobertura"

if ($LASTEXITCODE -eq 0) {
  $reportPath = Join-Path $reportOutputDir "index.html"
  Write-Host "Coverage report generated at: $reportPath"

  if ($openCoverageReport) {
    if (Test-Path $reportPath) {
      Start-Process $reportPath
    } else {
      Write-Warning "Report file not found at expected location: $reportPath"
    }
  }
} else {
  Write-Error "Failed to generate the coverage report."
  exit 1
}
