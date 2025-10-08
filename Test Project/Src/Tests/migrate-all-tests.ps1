#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Migrates all test projects to Microsoft Testing Platform with xUnit v3.
#>

param(
    [string]$SolutionPath = "Src",
    [switch]$DryRun,
    [switch]$SkipBackup
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Microsoft Testing Platform Migration" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Find test projects
$testProjects = Get-ChildItem -Path $SolutionPath -Recurse -Filter "*.csproj" |
    Where-Object {
        $_.Name -like "*Test*.csproj" -or
        $_.Name -like "*Tests*.csproj" -or
        $_.Directory.Name -like "*Test*" -or
        $_.Directory.Name -like "*Tests*"
    } | Sort-Object FullName

Write-Host "Found $($testProjects.Count) test projects:" -ForegroundColor Green
foreach ($project in $testProjects) {
    Write-Host "  - $($project.FullName)" -ForegroundColor Cyan
}

if ($DryRun) {
    Write-Host "`n🔍 DRY RUN MODE - No changes will be made" -ForegroundColor Yellow
}

# Create backup branch
if (-not $SkipBackup -and -not $DryRun) {
    Write-Host "`n📦 Creating backup branch..." -ForegroundColor Yellow
    try {
        git checkout -b "backup/pre-microsoft-testing-platform-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Write-Host "✅ Backup branch created" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not create backup branch: $($_.Exception.Message)"
    }
}

$migratedCount = 0
$failedCount = 0

foreach ($project in $testProjects) {
    Write-Host "`n🔄 Processing: $($project.Name)" -ForegroundColor Yellow

    try {
        $content = Get-Content $project.FullName -Raw

        if ($DryRun) {
            Write-Host "  [DRY RUN] Would migrate: $($project.Name)" -ForegroundColor Gray
            continue
        }

        # Check if already migrated
        if ($content -match "Microsoft\.Testing\.Platform") {
            Write-Host "  ⏭️  Already migrated, skipping..." -ForegroundColor Gray
            continue
        }

        # Create backup
        $backupPath = "$($project.FullName).backup"
        Copy-Item $project.FullName $backupPath

        # Update project file
        $updatedContent = $content -replace
            '<PropertyGroup>',
            '<PropertyGroup>
    <!-- Microsoft Testing Platform Configuration -->
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <TestingPlatformServer>true</TestingPlatformServer>'

        # Remove old xUnit packages
        $updatedContent = $updatedContent -replace
            '<PackageReference Include="xunit" />\s*', ''
        $updatedContent = $updatedContent -replace
            '<PackageReference Include="xunit\.runner\.visualstudio">.*?</PackageReference>\s*', ''

        # Add new packages
        $packageSection = @"

  <!-- Microsoft Testing Platform -->
  <PackageReference Include="Microsoft.Testing.Platform" />
  <PackageReference Include="Microsoft.Testing.Extensions.TrxReport" />

  <!-- xUnit v3 -->
  <PackageReference Include="xunit.v3" />
  <PackageReference Include="xunit.v3.core" />
  <PackageReference Include="xunit.v3.runner.visualstudio">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>

  <!-- Microsoft Testing Platform Configuration -->
  <ItemGroup>
    <ProjectCapability Include="DiagnoseCapabilities" />
    <ProjectCapability Include="TestingPlatformServer" />
    <ProjectCapability Include="TestContainer" />
  </ItemGroup>
"@

        $updatedContent = $updatedContent -replace
            '</Project>',
            "$packageSection
</Project>"

        # Save updated project
        Set-Content -Path $project.FullName -Value $updatedContent

        # Create testing platform config
        $configPath = Join-Path $project.Directory.FullName "$($project.BaseName).testingplatformconfig.json"
        $configContent = @"
{
  "`$schema": "https://aka.ms/testingplatform/v1/configuration.schema.json",
  "version": "1",
  "runSettings": {
    "dotnetTestSupport": {
      "enabled": true
    },
    "xunit": {
      "enabled": true,
      "parallelizeTestCollections": true,
      "maxParallelThreads": 0,
      "diagnosticMessages": false,
      "internalDiagnosticMessages": false
    }
  },
  "extensions": [
    {
      "extensionId": "Microsoft.Testing.Extensions.TrxReport",
      "configuration": {
        "outputPath": "TestResults",
        "fileName": "test-results.trx"
      }
    }
  ]
}
"@

        Set-Content -Path $configPath -Value $configContent

        Write-Host "  ✅ Successfully migrated: $($project.Name)" -ForegroundColor Green
        $migratedCount++

    }
    catch {
        Write-Host "  ❌ Failed to migrate: $($project.Name)" -ForegroundColor Red
        Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red

        if (Test-Path $backupPath) {
            Copy-Item $backupPath $project.FullName
            Write-Host "     Restored from backup" -ForegroundColor Yellow
        }

        $failedCount++
    }
}

# Summary
Write-Host "`n📊 Migration Summary" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green
Write-Host "Total projects: $($testProjects.Count)" -ForegroundColor White
Write-Host "Migrated: $migratedCount" -ForegroundColor Green
Write-Host "Failed: $failedCount" -ForegroundColor Red

if ($DryRun) {
    Write-Host "`n🔍 This was a dry run. Run without -DryRun to perform migration." -ForegroundColor Yellow
}
elseif ($migratedCount -gt 0) {
    Write-Host "`n✅ Migration completed!" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Build: dotnet build $SolutionPath" -ForegroundColor White
    Write-Host "2. Test: dotnet test $SolutionPath/Tests --logger trx" -ForegroundColor White
}

Write-Host "`n🎯 Migration script completed!" -ForegroundColor Green
