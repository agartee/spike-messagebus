param(
  [Parameter(Mandatory = $true)]
  [Alias("v")]
  [string]$minVer
)

$minMajorVer = $minVer.Split('.')[0]
$maxMajorVer = [int]$minMajorVer + 1
$maxVer = "$($maxMajorVer).0.0"

Invoke-Expression "dotnet --list-sdks" -OutVariable succOut 2>&1 | Out-Null

try {
  if (-not $succOut) {
    throw [System.Exception] `
      ".NET CLI installation not found (target: $($minMajorVer).x; min: $($minVer))."
  }

  $hasTargetVer = $false
  foreach ($result in $succOut) {
    $currentVer = $result.Split(' ')[0]
    if ([System.Version] $currentVer -gt [System.Version] $minVer `
        -and [System.Version] $currentVer -lt [System.Version] $maxVer) {
      $hasTargetVer = $true
      break
    }
  }

  if (-not $hasTargetVer) {
    throw [System.Exception] `
    ("Target .NET version not found: $($minMajorVer).x (minimum: $($minVer)).")
  }

  Write-Host ".NET installation found: $($currentVer)" `
    "(target: $($minMajorVer).x; min: $($minVer))" -ForegroundColor Green
}
catch [System.Exception] {
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit 1
}
