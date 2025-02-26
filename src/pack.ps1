[CmdletBinding(PositionalBinding = $false)]
param (
    [Parameter()]
    [string] $output = "",

    [Parameter()]
    [switch] $clearOutput = $false,

    [Parameter()]
    [string] $version = "",

    [Parameter(ValueFromRemainingArguments)]
    [string[]] $remainingArgs
)

function Execute ($argumentList) {
    $process = Start-Process -FilePath "dotnet" -ArgumentList $argumentList -NoNewWindow -Wait
    if ($null -ne $process.ExitCode || $process.ExitCode -ne 0) {
        exit $process.ExitCode
    }
}

if ($clearOutput)
{
    $dir = Resolve-Path -Path ($output -ne "" ? $output : "../artifacts/src/package/release")
    $items = Get-ChildItem -Path $dir -Filter "Blackwing*nupkg"
    if ($items.Length -gt 0)
    {
        Write-Host "Removing $($items.Length) items (.*nupkg) at '$dir'"
        $items | ForEach-Object { Remove-Item -Path $_}
    }
}

$projects = @(
    "$PSScriptRoot\Blackwing.Contracts\"
    "$PSScriptRoot\Blackwing.Extensions.DependencyInjection\"
    "$PSScriptRoot\Blackwing.Generator\"
)

$cmd = @("pack")
$arguments = @("-c", "Release")

foreach ($project in $projects) {
    $expression = $cmd + $project + $arguments;
    if ($output -ne "") {
        $expression += "-o", $output
    }
    if ($version -ne "") {
        $expression += "-p:Version=$version", "-p:DisableGitVersionTask=true"
    }
    $expression = ($expression + $remainingArgs) -join " "
    Write-Host "$ $expression"
    Execute $expression
}
