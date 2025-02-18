$consoleProject = "../Console/Console.csproj"
$consoleProjectAot = "./ConsoleAOT.csproj"

$publishConsole = "dotnet publish $consoleProject"
$publishConsoleAot = "dotnet publish $consoleProjectAot"

Write-Host "$ $publishConsole"
Invoke-Expression $publishConsole

Write-Host "$ $publishConsoleAot"
Invoke-Expression $publishConsoleAot

$consoleExe = Resolve-Path "../../artifacts/sample/publish/Console/release/Console.exe"
$consoleExeAot = Resolve-Path "../../artifacts/sample/publish/ConsoleAOT/release/ConsoleAOT.exe"
$hyperfine = "hyperfine $consoleExe $consoleExeAot --warmup 5"

Write-Host "$ $hyperfine"
Invoke-Expression $hyperfine
