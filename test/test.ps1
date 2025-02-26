$projects = @(
    "$PSScriptRoot\Blackwing.Test\"
    "$PSScriptRoot\Blackwing.Test.Integration\"
)

$cmd = @("dotnet", "test")
$arguments = @("-c", "Release")

foreach ($project in $projects) {
    $expression = ($cmd + $project + $arguments + $args) -join " ";
    Write-Host "$ $expression"
    Invoke-Expression -Command $expression
}
