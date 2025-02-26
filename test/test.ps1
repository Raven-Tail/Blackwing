function Execute ($argumentList) {
    $process = Start-Process -FilePath "dotnet" -ArgumentList $argumentList -NoNewWindow -Wait
    if ($null -ne $process.ExitCode || $process.ExitCode -ne 0) {
        exit $process.ExitCode
    }
}

$projects = @(
    "$PSScriptRoot\Blackwing.Test\"
    "$PSScriptRoot\Blackwing.Test.Integration\"
)

$cmd = @("test")
$arguments = @("-c", "Release")

foreach ($project in $projects) {
    $expression = ($cmd + $project + $arguments + $args) -join " ";
    Write-Host "$ $expression"
    Execute $expression
}
