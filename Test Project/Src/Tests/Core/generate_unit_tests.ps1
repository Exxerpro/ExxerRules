# IndTrace Unit Test Generator
# This script automatically generates unit test files for public classes

param(
    [string]$ProjectPath = "E:\Dynamic\IndTrace\IndTraceV2025\Src\",
    [switch]$GenerateAll,
    [string]$SpecificClass
)

# Configuration
$ApplicationPath = Join-Path $ProjectPath "Core\Application"
$DomainPath = Join-Path $ProjectPath "Core\Domain"
$TestOutputPath = Join-Path $ProjectPath "Tests\Core"

# Test template for Application layer classes
function Get-ApplicationTestTemplate {
    param($ClassName, $Namespace)

    return @"
using Xunit;
using NSubstitute;
using Shouldly;
using $Namespace;

namespace IndTrace.Tests.Core.Application.UnitTests;

/// <summary>
/// Unit tests for $ClassName
/// </summary>
public class ${ClassName}Tests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        // TODO: Add constructor parameters

        // Act
        var instance = new $ClassName();

        // Assert
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithInvalidParameters_ShouldThrowException()
    {
        // Arrange
        // TODO: Add invalid parameters

        // Act & Assert
        // TODO: Add exception assertion
    }

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var instance = new $ClassName();

        // Act & Assert
        // TODO: Test property setters and getters
    }

    [Fact]
    public void Methods_WhenCalled_ShouldReturnExpectedResults()
    {
        // Arrange
        var instance = new $ClassName();

        // Act
        // TODO: Call methods

        // Assert
        // TODO: Verify results
    }
}
"@
}

# Test template for Domain layer classes
function Get-DomainTestTemplate {
    param($ClassName, $Namespace)

    return @"
using Xunit;
using NSubstitute;
using Shouldly;
using $Namespace;

namespace IndTrace.Tests.Core.Domain.UnitTests;

/// <summary>
/// Unit tests for $ClassName
/// </summary>
public class ${ClassName}Tests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        // TODO: Add constructor parameters

        // Act
        var instance = new $ClassName();

        // Assert
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithInvalidParameters_ShouldThrowException()
    {
        // Arrange
        // TODO: Add invalid parameters

        // Act & Assert
        // TODO: Add exception assertion
    }

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var instance = new $ClassName();

        // Act & Assert
        // TODO: Test property setters and getters
    }

    [Fact]
    public void Methods_WhenCalled_ShouldReturnExpectedResults()
    {
        // Arrange
        var instance = new $ClassName();

        // Act
        // TODO: Call methods

        // Assert
        // TODO: Verify results
    }

    [Fact]
    public void DomainLogic_WhenExecuted_ShouldFollowBusinessRules()
    {
        // Arrange
        var instance = new $ClassName();

        // Act
        // TODO: Execute domain logic

        // Assert
        // TODO: Verify business rules
    }
}
"@
}

# Find all public classes in a directory
function Find-PublicClasses {
    param($Path, $Namespace)

    $classes = @()

    Get-ChildItem -Path $Path -Filter "*.cs" -Recurse | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        $lines = $content -split "`n"

        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i].Trim()
            if ($line -match "public class (\w+)") {
                $className = $matches[1]
                $classes += @{
                    Name = $className
                    Namespace = $Namespace
                    FilePath = $_.FullName
                    RelativePath = $_.FullName.Replace($Path, "").TrimStart("\")
                }
            }
        }
    }

    return $classes
}

# Generate test file
function New-TestFile {
    param($ClassName, $Namespace, $TestType, $OutputPath)

    $testFileName = "${ClassName}Tests.cs"
    $testFilePath = Join-Path $OutputPath $testFileName

    if (Test-Path $testFilePath) {
        Write-Host "Test file already exists: $testFileName" -ForegroundColor Yellow
        return
    }

    $template = if ($TestType -eq "Application") {
        Get-ApplicationTestTemplate -ClassName $ClassName -Namespace $Namespace
    } else {
        Get-DomainTestTemplate -ClassName $ClassName -Namespace $Namespace
    }

    $template | Out-File -FilePath $testFilePath -Encoding UTF8
    Write-Host "Generated: $testFileName" -ForegroundColor Green
}

# Main execution
Write-Host "IndTrace Unit Test Generator" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

if ($SpecificClass) {
    Write-Host "Generating test for specific class: $SpecificClass" -ForegroundColor Yellow
    # TODO: Implement specific class generation
} else {
    Write-Host "Scanning for public classes..." -ForegroundColor Yellow

    # Find Application classes
    $appClasses = Find-PublicClasses -Path $ApplicationPath -Namespace "IndTrace.Application"
    Write-Host "Found $($appClasses.Count) classes in Application layer" -ForegroundColor Green

    # Find Domain classes
    $domainClasses = Find-PublicClasses -Path $DomainPath -Namespace "IndTrace.Domain"
    Write-Host "Found $($domainClasses.Count) classes in Domain layer" -ForegroundColor Green

    Write-Host ""
    Write-Host "Application Classes:" -ForegroundColor Cyan
    $appClasses | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }

    Write-Host ""
    Write-Host "Domain Classes:" -ForegroundColor Cyan
    $domainClasses | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }

    if ($GenerateAll) {
        Write-Host ""
        Write-Host "Generating all test files..." -ForegroundColor Yellow

        # Create output directories
        $appTestPath = Join-Path $TestOutputPath "Application.UnitTests"
        $domainTestPath = Join-Path $TestOutputPath "Domain.UnitTests"

        if (!(Test-Path $appTestPath)) { New-Item -ItemType Directory -Path $appTestPath -Force }
        if (!(Test-Path $domainTestPath)) { New-Item -ItemType Directory -Path $domainTestPath -Force }

        # Generate Application tests
        $appClasses | ForEach-Object {
            New-TestFile -ClassName $_.Name -Namespace $_.Namespace -TestType "Application" -OutputPath $appTestPath
        }

        # Generate Domain tests
        $domainClasses | ForEach-Object {
            New-TestFile -ClassName $_.Name -Namespace $_.Namespace -TestType "Domain" -OutputPath $domainTestPath
        }

        Write-Host ""
        Write-Host "Test generation completed!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "To generate all test files, run: .\generate_unit_tests.ps1 -GenerateAll" -ForegroundColor Yellow
        Write-Host "To generate test for specific class, run: .\generate_unit_tests.ps1 -SpecificClass 'ClassName'" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Script completed." -ForegroundColor Cyan
