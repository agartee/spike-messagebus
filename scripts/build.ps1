[CmdletBinding(DefaultParameterSetName = 'local')]
param(
  [Parameter(Mandatory = $false, ParameterSetName = 'help')]
  [Alias("h")]
  [switch]$help,

  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [Alias("l")]
  [switch]$local,

  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [Alias("d")]
  [switch]$docker,

  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false, HelpMessage = "Assembly version (e.g., 1.2.3.4)")]
  [Alias("v")]
  [string]$version = "1.0.0"
)

if ($help) {
  Write-Output @"

Initiates a build of the project.

Usage: build.ps1 [[-local] | [docker]] [-configuration <value>] 
  [-version <value>]

Options:
-local|-l           Builds the application locally. This is the default script 
                    behavior.
-docker|-d          Builds the application in a Docker image.
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Debug".
-version|-v         Specifies the version for the build outputs. If not 
                    specified, the default is "1.0.0".
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

if ($PSCmdlet.ParameterSetName -eq "local") {
  & "$rootDir\scripts\support\build-local.ps1" `
    -configuration "$configuration" `
    -version "$version" `
}

if ($docker) {
  & "$rootDir\scripts\support\build-docker.ps1" `
    -configuration "$configuration" `
    -version "$version" `
}
