[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Cleans up temporary directories and files from the project.

Usage: clean.ps1

"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

. "$rootDir\scripts\support\delete-coverage-files.ps1"
