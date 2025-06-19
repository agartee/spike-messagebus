[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Checks for applications dependencies needed to contribute to this application.

Usage: bootstrap.ps1

"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

. "$rootDir\scripts\support\check-dotnet.ps1" -minVer "9.0.0"
. "$rootDir\scripts\support\restore-dotnet-tools.ps1"
