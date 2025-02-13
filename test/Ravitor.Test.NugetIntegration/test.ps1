$pack = "..\..\src\pack.ps1 -output .\packages -clearOutput -version 1.0.0-test"

# Restore the project using the custom config file, restoring packages to a local folder
$restore = "dotnet restore --packages .\packages --configfile nuget.integration.config"

# Build the project (no restore), using the packages restored to the local folder
$build = "dotnet build -c Release --packages .\packages --no-restore"

# Test the project (no build or restore)
$test = "dotnet test -c Release --no-build --no-restore "

New-Item -Name "packages" -ItemType Directory -ErrorAction SilentlyContinue
Invoke-Expression $pack
Invoke-Expression $restore
Invoke-Expression $build
Invoke-Expression $test
