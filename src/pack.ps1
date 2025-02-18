[CmdletBinding()]
param (
    [Parameter()]
    [string] $output = "",

    [Parameter()]
    [switch] $clearOutput = $false,

    [Parameter()]
    [string] $version = ""
)

if ($clearOutput)
{
    $dir = Resolve-Path -Path ($output -ne "" ? $output : "../artifacts/src/package/release")
    $items = Get-ChildItem -Path $dir -Filter "*nupkg"
    if ($items.Length -gt 0)
    {
        Write-Host "Removing $($items.Length) items (.*nupkg) at '$dir'"
        $items | Remove-Item
    }
}

$projects = @(
    "$PSScriptRoot\Blackwing.Contracts\"
    "$PSScriptRoot\Blackwing.Extensions.DependencyInjection\"
    "$PSScriptRoot\Blackwing.Generator\"
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
