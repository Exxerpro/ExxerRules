#!/usr/bin/env pwsh

param(
    [switch]$DryRun,
    [switch]$SkipBackup,
    [string]$SolutionPath = "Src"
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Complete Microsoft Testing Platform Migration" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host "This script will migrate ALL test projects to Microsoft Testing Platform" -ForegroundColor Yellow
Write-Host ""

# Step 1: Discover all test projects
Write-Host "🔍 Step 1: Discovering test projects..." -ForegroundColor Yellow
$testProjects = @()

# Find test projects by name pattern
$testProjects += Get-ChildItem -Path $SolutionPath -Recurse -Filter "*.csproj" |
    Where-Object {
        $_.Name -like "*Test*.csproj" -or
        $_.Name -like "*Tests*.csproj" -or
        $_.Directory.Name -like "*Test*" -or
        $_.Directory.Name -like "*Tests*"
    }

# Remove duplicates and sort
$testProjects = $testProjects | Sort-Object FullName -Unique

Write-Host "Found $($testProjects.Count) test projects:" -ForegroundColor Green
foreach ($project in $testProjects) {
    Write-Host "  📁 $($project.FullName)" -ForegroundColor Cyan
}

if ($DryRun) {
    Write-Host "`n🔍 DRY RUN MODE - No changes will be made" -ForegroundColor Yellow
    Write-Host "The following actions would be performed:" -ForegroundColor Yellow
}

# Step 2: Create backup branch
if (-not $SkipBackup -and -not $DryRun) {
    Write-Host "`n📦 Step 2: Creating backup branch..." -ForegroundColor Yellow
    try {
        $branchName = "backup/pre-microsoft-testing-platform-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        git checkout -b $branchName
        Write-Host "✅ Created backup branch: $branchName" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not create backup branch: $($_.Exception.Message)"
        Write-Host "Continuing without backup..." -ForegroundColor Yellow
    }
}

# Step 3: Migrate each project
Write-Host "`n🔄 Step 3: Migrating test projects..." -ForegroundColor Yellow
$migratedCount = 0
$failedCount = 0
$skippedCount = 0

foreach ($project in $testProjects) {
    Write-Host "`n📋 Processing: $($project.Name)" -ForegroundColor Yellow

    try {
        $projectContent = Get-Content $project.FullName -Raw

        # Check if already migrated
        if ($projectContent -match "Microsoft\.Testing\.Platform") {
            Write-Host "  ⏭️  Already migrated, skipping..." -ForegroundColor Gray
            $skippedCount++
            continue
        }

        if ($DryRun) {
            Write-Host "  [DRY RUN] Would migrate: $($project.Name)" -ForegroundColor Gray
            continue
        }

        # Create backup of original file
        $backupPath = "$($project.FullName).backup"
        Copy-Item $project.FullName $backupPath
        Write-Host "  💾 Created backup: $backupPath" -ForegroundColor Gray

        # Parse XML
        $projectXml = [xml]$projectContent

        # Update PropertyGroup
        $propertyGroup = $projectXml.Project.PropertyGroup[0]
        if (-not $propertyGroup) {
            $propertyGroup = $projectXml.Project.CreateElement("PropertyGroup")
            $projectXml.Project.InsertBefore($propertyGroup, $projectXml.Project.ItemGroup[0])
        }

        # Add Microsoft Testing Platform properties
        $testingPlatformProps = @{
            "UseMicrosoftTestingPlatformRunner" = "true"
            "TestingPlatformDotnetTestSupport" = "true"
            "TestingPlatformServer" = "true"
        }

        foreach ($prop in $testingPlatformProps.GetEnumerator()) {
            if (-not $propertyGroup.SelectSingleNode($prop.Key)) {
                $element = $projectXml.CreateElement($prop.Key)
                $element.InnerText = $prop.Value
                $propertyGroup.AppendChild($element)
                Write-Host "  ➕ Added property: $($prop.Key)" -ForegroundColor Gray
            }
        }

        # Find or create ItemGroup for packages
        $itemGroup = $projectXml.Project.ItemGroup | Where-Object { $_.PackageReference } | Select-Object -First 1
        if (-not $itemGroup) {
            $itemGroup = $projectXml.Project.CreateElement("ItemGroup")
            $projectXml.Project.AppendChild($itemGroup)
        }

        # Remove old xUnit packages
        $oldPackages = @("xunit", "xunit.runner.visualstudio")
        foreach ($oldPackage in $oldPackages) {
            $oldRef = $itemGroup.PackageReference | Where-Object { $_.Include -eq $oldPackage }
            if ($oldRef) {
                $itemGroup.RemoveChild($oldRef)
                Write-Host "  ➖ Removed old package: $oldPackage" -ForegroundColor Gray
            }
        }

        # Add Microsoft Testing Platform packages
        $newPackages = @(
            @{ Include = "Microsoft.Testing.Platform" },
            @{ Include = "Microsoft.Testing.Extensions.TrxReport" },
            @{ Include = "xunit.v3" },
            @{ Include = "xunit.v3.core" },
            @{ Include = "xunit.v3.runner.visualstudio"; PrivateAssets = "all"; IncludeAssets = "runtime; build; native; contentfiles; analyzers; buildtransitive" }
        )

        foreach ($package in $newPackages) {
            $packageRef = $projectXml.CreateElement("PackageReference")
            $packageRef.SetAttribute("Include", $package.Include)

            if ($package.PrivateAssets) {
                $packageRef.SetAttribute("PrivateAssets", $package.PrivateAssets)
            }
            if ($package.IncludeAssets) {
                $packageRef.SetAttribute("IncludeAssets", $package.IncludeAssets)
            }

            $itemGroup.AppendChild($packageRef)
            Write-Host "  ➕ Added package: $($package.Include)" -ForegroundColor Gray
        }

        # Add project capabilities
        $capabilitiesGroup = $projectXml.Project.CreateElement("ItemGroup")
        $capabilities = @("DiagnoseCapabilities", "TestingPlatformServer", "TestContainer")

        foreach ($capability in $capabilities) {
            $capabilityElement = $projectXml.CreateElement("ProjectCapability")
            $capabilityElement.SetAttribute("Include", $capability)
            $capabilitiesGroup.AppendChild($capabilityElement)
        }

        $projectXml.Project.AppendChild($capabilitiesGroup)
        Write-Host "  ➕ Added project capabilities" -ForegroundColor Gray

        # Save updated project
        $projectXml.Save($project.FullName)

        # Create testing platform config
        $configPath = Join-Path $project.Directory.FullName "$($project.BaseName).testingplatformconfig.json"
        $configContent = @"
{
  "$schema": "https://aka.ms/testingplatform/v1/configuration.schema.json",
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
        if (Test-Path $backupPath) {
            Copy-Item $backupPath $project.FullName
            Write-Host "     Restored from backup" -ForegroundColor Yellow
        }

        $failedCount++
    }
}

# Step 4: Validation
Write-Host "`n🔍 Step 4: Validation..." -ForegroundColor Yellow

if (-not $DryRun -and $migratedCount -gt 0) {
    Write-Host "Building solution to validate migration..." -ForegroundColor Yellow
    try {
        $buildResult = dotnet build $SolutionPath --verbosity quiet
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Build successful - migration validated!" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Build failed - please check for issues" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "⚠️  Could not validate build: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Summary
Write-Host "`n📊 Migration Summary" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green
Write-Host "Total projects found: $($testProjects.Count)" -ForegroundColor White
Write-Host "Successfully migrated: $migratedCount" -ForegroundColor Green
Write-Host "Skipped (already migrated): $skippedCount" -ForegroundColor Gray
Write-Host "Failed migrations: $failedCount" -ForegroundColor Red

if ($DryRun) {
    Write-Host "`n🔍 This was a dry run. No changes were made." -ForegroundColor Yellow
    Write-Host "Run without -DryRun to perform the actual migration." -ForegroundColor Yellow
}
else {
    if ($migratedCount -gt 0) {
        Write-Host "`n✅ Migration completed successfully!" -ForegroundColor Green
        Write-Host "`n🎯 Next steps:" -ForegroundColor Yellow
        Write-Host "1. Test the migration: dotnet test $SolutionPath/Tests --logger trx" -ForegroundColor White
        Write-Host "2. Verify performance improvements" -ForegroundColor White
        Write-Host "3. Update CI/CD pipelines if needed" -ForegroundColor White
        Write-Host "4. Commit changes: git add . && git commit -m 'Migrate to Microsoft Testing Platform'" -ForegroundColor White
    }
    else {
        Write-Host "`n⚠️  No projects were migrated." -ForegroundColor Yellow
    }
}

Write-Host "`n🎯 Complete migration script finished!" -ForegroundColor Green
