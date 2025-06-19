$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName

Push-Location $rootDir
dotnet tool restore
Pop-Location
