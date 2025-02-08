[CmdletBinding()]
param (
    [Parameter()]
    [string] $output = "",

    [Parameter()]
    [string] $version = ""
)

if ($output -ne ""){
    Resolve-Path $output | Remove-Item -Filter "*nupkg" -Recurse -ErrorAction SilentlyContinue
}
else {
    Remove-Item -Path "../artifacts/src/package/release" -Filter "*nupkg" -Recurse -ErrorAction SilentlyContinue
}

$projects = @(
    "$PSScriptRoot\Ravitor.Contracts\"
    "$PSScriptRoot\Ravitor.Extensions.DependencyInjection\"
    "$PSScriptRoot\Ravitor.Generator\"
)

$cmd = "dotnet", "pack", "-c", "Release"

foreach ($project in $projects) {
    $expression = $cmd;

    if ($output -ne "") {
        $output = Resolve-Path $output
        $expression += "-o", $output
    }
    if ($version -ne "") {
        $expression += "-p:Version=$version", "-p:DisableGitVersionTask=true"
    }
    $expression += $project
    $expression = $expression -join " "
    Write-Host "$ $expression"
    Invoke-Expression -Command $expression
}

# dotnet pack -c Release .\Ravitor.Contracts\
# dotnet pack -c Release .\Ravitor.Extensions.DependencyInjection\
# dotnet pack -c Release .\Ravitor.Generator\
