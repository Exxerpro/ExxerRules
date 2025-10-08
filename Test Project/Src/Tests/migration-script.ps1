#!/usr/bin/env pwsh

param([switch]$DryRun)

Write-Host "🚀 Microsoft Testing Platform Migration" -ForegroundColor Green

# Find test projects - look in the parent Src directory
$testProjects = Get-ChildItem -Path ".." -Recurse -Filter "*.csproj" |
    Where-Object {
        $_.Name -like "*Test*.csproj" -or
        $_.Name -like "*Tests*.csproj" -or
        $_.Directory.Name -like "*Test*" -or
        $_.Directory.Name -like "*Tests*"
    } | Sort-Object FullName

Write-Host "Found $($testProjects.Count) test projects:" -ForegroundColor Green
foreach ($project in $testProjects) {
    Write-Host "  📁 $($project.FullName)" -ForegroundColor Cyan
}

if ($DryRun) {
    Write-Host "`n🔍 DRY RUN MODE - No changes will be made" -ForegroundColor Yellow
    Write-Host "The following actions would be performed:" -ForegroundColor Yellow
}

$migratedCount = 0
$failedCount = 0

foreach ($project in $testProjects) {
    Write-Host "`n🔄 Processing: $($project.Name)" -ForegroundColor Yellow

    try {
        $projectContent = Get-Content $project.FullName -Raw

        # Check if already migrated
        if ($projectContent -match "Microsoft\.Testing\.Platform") {
            Write-Host "  ⏭️  Already migrated, skipping..." -ForegroundColor Gray
            continue
        }

        if ($DryRun) {
            Write-Host "  [DRY RUN] Would migrate: $($project.Name)" -ForegroundColor Gray
            continue
        }

        # Create backup
        Copy-Item $project.FullName "$($project.FullName).backup"
        Write-Host "  💾 Created backup" -ForegroundColor Gray

        # Add Microsoft Testing Platform properties to PropertyGroup
        if ($projectContent -match '<PropertyGroup>') {
            $projectContent = $projectContent -replace
                '<PropertyGroup>',
                '<PropertyGroup>
    <!-- Microsoft Testing Platform Configuration -->
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <TestingPlatformServer>true</TestingPlatformServer>'
            Write-Host "  ➕ Added Microsoft Testing Platform properties" -ForegroundColor Gray
        }

        # Remove old xUnit packages
        $projectContent = $projectContent -replace
            '\s*<PackageReference Include="xunit" />\s*', ''
        $projectContent = $projectContent -replace
            '\s*<PackageReference Include="xunit\.runner\.visualstudio">.*?</PackageReference>\s*', ''
        Write-Host "  ➖ Removed old xUnit packages" -ForegroundColor Gray

        # Add new packages before closing Project tag
        $newPackages = @'
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
'@

        $projectContent = $projectContent -replace '</Project>', "$newPackages`n</Project>"
        Write-Host "  ➕ Added Microsoft Testing Platform packages and capabilities" -ForegroundColor Gray

        # Save updated project
        Set-Content -Path $project.FullName -Value $projectContent -NoNewline

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
      "internalDiagnosticMessages": false,
      "failSkips": false,
      "stopOnFail": false,
      "preEnumerateTheories": true
    }
  },
  "logging": {
    "logLevel": {
      "default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
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
  ],
  "telemetry": {
    "enabled": false
  },
  "diagnostics": {
    "enabled": false
  }
}
"@

        Set-Content -Path $configPath -Value $configContent
        Write-Host "  ➕ Created config: $configPath" -ForegroundColor Gray

        Write-Host "  ✅ Successfully migrated: $($project.Name)" -ForegroundColor Green
        $migratedCount++

    }
    catch {
        Write-Host "  ❌ Failed to migrate: $($project.Name)" -ForegroundColor Red
        Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red

        # Restore backup if available
        $backupPath = "$($project.FullName).backup"
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
Write-Host "Total projects found: $($testProjects.Count)" -ForegroundColor White
Write-Host "Successfully migrated: $migratedCount" -ForegroundColor Green
Write-Host "Failed migrations: $failedCount" -ForegroundColor Red

if ($DryRun) {
    Write-Host "`n🔍 This was a dry run. No changes were made." -ForegroundColor Yellow
    Write-Host "Run without -DryRun to perform the actual migration." -ForegroundColor Yellow
}
elseif ($migratedCount -gt 0) {
    Write-Host "`n✅ Migration completed successfully!" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Build: dotnet build .." -ForegroundColor White
    Write-Host "2. Test: dotnet test .. --logger trx" -ForegroundColor White
}

Write-Host "`n🎯 Migration script completed!" -ForegroundColor Green
