$consoleProject = "../Console/Console.csproj"
$consoleProjectAot = "./ConsoleAOT.csproj"

Write-Host "$ dotnet publish $consoleProject"
dotnet publish $consoleProject

Write-Host "$ dotnet publish $consoleProjectAot"
dotnet publish $consoleProjectAot

$consoleExe = Resolve-Path "../../artifacts/sample/publish/Console/release/Console.exe"
$consoleExeAot = Resolve-Path "../../artifacts/sample/publish/ConsoleAOT/release/ConsoleAOT.exe"

Write-Host "$ hyperfine $consoleExe $consoleExeAot"
hyperfine $consoleExe $consoleExeAot