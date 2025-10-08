# PowerShell script to remove xUnit v2 package references
# This script will remove all xUnit v2 packages that are causing conflicts

$testProjects = @(
    "../Presentation/MetricsTests/MetricsTests.csproj",
    "../Presentation/IndTrace.Oee.Tests/IndTrace.Oee.Tests/IndTrace.Oee.Tests.csproj",
    "../Presentation/IndTrace.Monitor.Tests/IndTrace.Monitor.Tests.csproj",
    "../Integration/Integration.UnitTests/Integration.UnitTests.csproj",
    "../Integration/Indtrace.Mutable.Tests/Indtrace.Mutable.Tests.csproj",
    "../Integration/IndTrace.IntegrationProgram/IndTrace.IntegrationProgram.csproj",
    "../Integration/Indtrace.Integration.Tests/IndTrace.Integration.Tests.csproj",
    "../Integration/IndTrace.Gateway.Tests/GateWay.Tests.csproj",
    "../Integration/Gateway.UnitTests/Gateway.UnitTests.csproj",
    "../HelperMethodsTests/HelperMethodsTests.csproj",
    "../E2E/E2E.csproj",
    "../Core/Models.UnitTests/Models.UnitTests.csproj",
    "../Core/HubConnection.Tests/HubConnection.Tests.csproj",
    "../Core/Filters.Tests/Filters.Tests/Filters.Tests.csproj",
    "../Core/Domain.UnitTests/Domain.UnitTests.csproj",
    "../Core/Application.UnitTests/Application.UnitTests.csproj",
    "../Acceptance/Intrace.BDDTests/IndTrace.BDDTests.csproj",
    "../Architecture/Architecture.Tests.csproj",
    "../MetricsTests/MetricsTests.csproj"
)

Write-Host "Removing xUnit v2 package references from test projects..." -ForegroundColor Yellow

foreach ($project in $testProjects) {
    if (Test-Path $project) {
        Write-Host "Processing: $project" -ForegroundColor Cyan

        $content = Get-Content $project -Raw

        # Remove xunit.runner.visualstudio references
        $content = $content -replace '<PackageReference Include="xunit\.runner\.visualstudio"[^>]*>', ''
        $content = $content -replace '<PackageReference Include="xunit\.v3\.runner\.visualstudio"[^>]*>', ''

        # Remove problematic packages that pull in xUnit v2
        $content = $content -replace '<PackageReference Include="Microsoft\.Playwright\.Xunit"[^>]*>', ''
        $content = $content -replace '<PackageReference Include="Xunit\.DependencyInjection"[^>]*>', ''
        $content = $content -replace '<PackageReference Include="LightBDD\.XUnit2"[^>]*>', ''
        $content = $content -replace '<PackageReference Include="SignalR\.UnitTestingSupport\.xUnit"[^>]*>', ''

        # Clean up empty lines
        $content = $content -replace '(?m)^\s*$\r?\n', ''

        Set-Content $project $content -NoNewline
        Write-Host "  ✓ Updated $project" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Project not found: $project" -ForegroundColor Yellow
    }
}

Write-Host "`nRemoving xUnit v2 packages from Directory.Packages.props..." -ForegroundColor Yellow

$propsPath = "../Directory.Packages.props"
if (Test-Path $propsPath) {
    $content = Get-Content $propsPath -Raw

    # Remove xUnit v2 package versions
    $content = $content -replace '\s*<PackageVersion Include="xunit\.runner\.visualstudio"[^>]*>', ''
    $content = $content -replace '\s*<PackageVersion Include="LightBDD\.XUnit2"[^>]*>', ''
    $content = $content -replace '\s*<PackageVersion Include="SignalR\.UnitTestingSupport\.xUnit"[^>]*>', ''
    $content = $content -replace '\s*<PackageVersion Include="Microsoft\.Playwright\.Xunit"[^>]*>', ''
    $content = $content -replace '\s*<PackageVersion Include="Xunit\.DependencyInjection"[^>]*>', ''

    # Clean up empty lines
    $content = $content -replace '(?m)^\s*$\r?\n', ''

    Set-Content $propsPath $content -NoNewline
    Write-Host "  ✓ Updated Directory.Packages.props" -ForegroundColor Green
}

Write-Host "`nDone! Please run 'dotnet restore' and 'dotnet build' to verify the changes." -ForegroundColor Green
