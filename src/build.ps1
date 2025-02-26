function Execute ($argumentList) {
    $process = Start-Process -FilePath "dotnet" -ArgumentList $argumentList -NoNewWindow -Wait
    if ($null -ne $process.ExitCode || $process.ExitCode -ne 0) {
        exit $process.ExitCode
    }
}

$projects = @(
    "$PSScriptRoot\Blackwing.Contracts\"
    "$PSScriptRoot\Blackwing.Extensions.DependencyInjection\"
    "$PSScriptRoot\Blackwing.Generator\"
)

$cmd = @("build")
$arguments = @("-c", "Release")

foreach ($project in $projects) {
    $expression = ($cmd + $project + $arguments + $args) -join " ";
    Write-Host "$ $expression"
    Execute $expression
}
