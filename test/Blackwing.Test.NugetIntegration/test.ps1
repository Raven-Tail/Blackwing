function Execute ($command, $argumentList) {

    $process = Start-Process -FilePath $command -ArgumentList $argumentList -NoNewWindow -Wait
    Write-Host $process
    if ($null -ne $process.ExitCode || $process.ExitCode -ne 0) {
        exit $process.ExitCode
    }
}

$pack = "-c ..\..\src\pack.ps1 -output .\packages -clearOutput -version 1.0.0-test"

# Restore the project using the custom config file, restoring packages to a local folder
$restore = "restore --packages .\packages --configfile nuget.integration.config"

# Build the project (no restore), using the packages restored to the local folder
$build = "build -c Release --packages .\packages --no-restore"

# Test the project (no build or restore)
$test = "test -c Release --no-build --no-restore "

New-Item -Name "packages" -ItemType Directory -ErrorAction SilentlyContinue
Get-ChildItem -Path ".\packages\" -Filter "Blackwing*" -Recurse -Directory | Remove-Item -Recurse

Execute "pwsh" $pack
Execute "dotnet" $restore
Execute "dotnet" $build
Execute "dotnet" $test
