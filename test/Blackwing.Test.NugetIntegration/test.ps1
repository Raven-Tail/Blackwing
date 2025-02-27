Import-Module $PSScriptRoot\..\..\Invoke-Process.psm1

$pack = "pwsh -c ..\..\src\pack.ps1 -output .\packages -clearOutput -version 1.0.0-test"
# $pack = "..\..\src\pack.ps1 -output .\packages -clearOutput -version 1.0.0-test"

# Restore the project using the custom config file, restoring packages to a local folder
$restore = "dotnet restore --packages .\packages --configfile nuget.integration.config"

# Build the project (no restore), using the packages restored to the local folder
$build = "dotnet build -c Release --packages .\packages --no-restore"

# Test the project (no build or restore)
$test = "dotnet test -c Release --no-build --no-restore "

New-Item -Name "packages" -ItemType Directory -ErrorAction SilentlyContinue
Get-ChildItem -Path ".\packages\" -Filter "Blackwing*" -Recurse -Directory | Remove-Item -Recurse

Invoke-Process $pack
Invoke-Process $restore
Invoke-Process $build
Invoke-Process $test
