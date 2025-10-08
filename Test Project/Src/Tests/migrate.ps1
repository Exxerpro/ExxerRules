#!/usr/bin/env pwsh

param([switch]$DryRun)

Write-Host "🚀 Microsoft Testing Platform Migration" -ForegroundColor Green

# Find test projects
$testProjects = Get-ChildItem -Path "Src" -Recurse -Filter "*.csproj" |
    Where-Object { $_.Name -like "*Test*.csproj" -or $_.Name -like "*Tests*.csproj" }

Write-Host "Found $($testProjects.Count) test projects" -ForegroundColor Green

foreach ($project in $testProjects) {
    Write-Host "Processing: $($project.Name)" -ForegroundColor Yellow

    if ($DryRun) {
        Write-Host "  [DRY RUN] Would migrate: $($project.Name)" -ForegroundColor Gray
        continue
    }

    try {
        # Create backup
        Copy-Item $project.FullName "$($project.FullName).backup"

        # Read and update project file
        $content = Get-Content $project.FullName -Raw

        # Add Microsoft Testing Platform properties
        $content = $content -replace
            '<PropertyGroup>',
            '<PropertyGroup>
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <TestingPlatformServer>true</TestingPlatformServer>'

        # Add new packages
        $content = $content -replace
            '</Project>',
            '  <!-- Microsoft Testing Platform -->
  <PackageReference Include="Microsoft.Testing.Platform" />
  <PackageReference Include="Microsoft.Testing.Extensions.TrxReport" />
  <PackageReference Include="xunit.v3" />
  <PackageReference Include="xunit.v3.core" />
  <PackageReference Include="xunit.v3.runner.visualstudio" />
</Project>'

        Set-Content -Path $project.FullName -Value $content

        Write-Host "  ✅ Migrated: $($project.Name)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ❌ Failed: $($project.Name)" -ForegroundColor Red
    }
}

Write-Host "`n🎯 Migration completed!" -ForegroundColor Green
