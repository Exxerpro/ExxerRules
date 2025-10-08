# PowerShell script to add global using statements to all test projects
# This script will add the necessary global using statements for xUnit v3

$testProjects = @(
    "Tests/Core/Application.UnitTests",
    "Tests/Core/Domain.UnitTests",
    "Tests/Core/Filters.Tests/Filters.Tests",
    "Tests/Core/HubConnection.Tests",
    "Tests/Core/Models.UnitTests",
    "Tests/Integration/IndTrace.Gateway.Tests",
    "Tests/Integration/Indtrace.Integration.Tests",
    "Tests/Integration/Indtrace.Mutable.Tests",
    "Tests/Presentation/IndTrace.Oee.Tests/IndTrace.Oee.Tests"
)

$globalUsingsContent = @"
// Global using directives for xUnit v3

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Xunit;
"@

foreach ($project in $testProjects) {
    $globalUsingsPath = "Src/$project/GlobalUsings.cs"

    Write-Host "Processing: $globalUsingsPath"

    if (Test-Path $globalUsingsPath) {
        Write-Host "  - GlobalUsings.cs already exists, checking content..."
        $content = Get-Content $globalUsingsPath -Raw
        if ($content -notmatch "global using Xunit;") {
            Write-Host "  - Adding global using Xunit; to existing file..."
            $newContent = $content + "`nglobal using Xunit;`n"
            Set-Content $globalUsingsPath $newContent
        } else {
            Write-Host "  - global using Xunit; already exists"
        }
    } else {
        Write-Host "  - Creating new GlobalUsings.cs file..."
        Set-Content $globalUsingsPath $globalUsingsContent
    }
}

Write-Host "`nGlobal using statements have been added to all test projects."
Write-Host "Please rebuild your solution to verify the changes."
