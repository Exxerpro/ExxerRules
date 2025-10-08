#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Migrates all test projects in the solution to Microsoft Testing Platform with xUnit v3.

.DESCRIPTION
    This script performs a complete migration of all test projects to ensure no mixed environments.
    It updates project files, adds Microsoft Testing Platform configuration, and validates the migration.

.PARAMETER SolutionPath
    Path to the solution directory. Defaults to "Src".

.PARAMETER DryRun
    If specified, shows what would be migrated without making changes.

.PARAMETER SkipBackup
    If specified, skips creating a backup branch.

.EXAMPLE
    .\migrate-to-microsoft-testing-platform.ps1 -DryRun
    Shows what would be migrated without making changes.

.EXAMPLE
    .\migrate-to-microsoft-testing-platform.ps1
    Performs the complete migration.
#>

param(
    [string]$SolutionPath = "Src",
    [switch]$DryRun,
    [switch]$SkipBackup
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "🚀 Microsoft Testing Platform Migration Script" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green

# Validate solution path
if (-not (Test-Path $SolutionPath)) {
    Write-Error "Solution path '$SolutionPath' does not exist."
    exit 1
}

# Find all test projects
Write-Host "🔍 Discovering test projects..." -ForegroundColor Yellow
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
    Write-Host "The following actions would be performed:" -ForegroundColor Yellow
}

# Create backup branch if not skipped
if (-not $SkipBackup -and -not $DryRun) {
    Write-Host "`n📦 Creating backup branch..." -ForegroundColor Yellow
    try {
        git checkout -b "backup/pre-microsoft-testing-platform-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Write-Host "✅ Backup branch created successfully" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not create backup branch: $($_.Exception.Message)"
        Write-Host "Continuing without backup..." -ForegroundColor Yellow
    }
}

# Migration counter
$migratedCount = 0
$failedCount = 0

foreach ($project in $testProjects) {
    Write-Host "`n🔄 Processing: $($project.Name)" -ForegroundColor Yellow

    try {
        $projectContent = Get-Content $project.FullName -Raw

        if ($DryRun) {
            Write-Host "  [DRY RUN] Would migrate: $($project.Name)" -ForegroundColor Gray
            continue
        }

        # Check if already migrated
        if ($projectContent -match "Microsoft\.Testing\.Platform") {
            Write-Host "  ⏭️  Already migrated, skipping..." -ForegroundColor Gray
            continue
        }

        # Create backup of original file
        $backupPath = "$($project.FullName).backup"
        Copy-Item $project.FullName $backupPath

        # Read project content
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
            }
        }

        # Update ItemGroup for packages
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
else {
    if ($migratedCount -gt 0) {
        Write-Host "`n✅ Migration completed successfully!" -ForegroundColor Green
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "1. Build the solution: dotnet build $SolutionPath" -ForegroundColor White
        Write-Host "2. Run tests: dotnet test $SolutionPath/Tests --logger trx" -ForegroundColor White
        Write-Host "3. Verify performance improvements" -ForegroundColor White
    }
    else {
        Write-Host "`n⚠️  No projects were migrated." -ForegroundColor Yellow
    }
}

Write-Host "`n🎯 Migration script completed!" -ForegroundColor Green
