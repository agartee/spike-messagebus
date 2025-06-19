param(
  [Parameter(Mandatory = $true, HelpMessage = "Container name or ID")]
  [Alias("c")]
  [string]$containerNameOrId
)

$timeoutSeconds = 10
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

try {
  while ($true) {
    Invoke-Expression "docker inspect --format='{{.State.Health.Status}}' $($containerNameOrId)" `
      -ErrorVariable errOut -OutVariable succOut 2>&1 | Out-Null

    if (-not $succOut) {
      throw [System.InvalidOperationException] ("$($errOut)")
    }
  
    if ($succOut -eq "healthy") {
      Write-Host "Container is healthy."
      break
    }
  
    if ($stopwatch.Elapsed.TotalSeconds -gt $timeoutSeconds) {
      throw [System.Exception] `
      ("Timeout exceeded waiting for container to become healthy.")
    }
  
    Start-Sleep -Seconds 2
  }  
}
catch {
  throw
}
finally {
  $stopwatch.Stop()
}
