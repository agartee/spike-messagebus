$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName

$coverageGlob = Get-ChildItem -Path $rootDir `
  -Recurse `
  -Filter coverage.cobertura.xml `
  -ErrorAction SilentlyContinue |
  Select-Object -ExpandProperty Directory |
  Get-Unique |
  Where-Object { Test-Path $_ }

foreach ($dir in $coverageGlob) {
  Write-Host "Deleting: $dir"
  Remove-Item -Path $dir -Recurse -Force
}
