# Validate-Implementation.ps1
# Validates that implementation follows existing patterns and doesn't introduce regressions

param(
    [Parameter(Mandatory=$true)]
    [string]$History,
    
    [Parameter(Mandatory=$false)]
    [string]$Path = "src/",
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

Write-Host "Validating History $History implementation..." -ForegroundColor Green
Write-Host "Path: $Path" -ForegroundColor Yellow

# 1. Check if implementation follows existing patterns
Write-Host "1. Checking pattern compliance..." -ForegroundColor Yellow
$patternViolations = Get-PatternViolations -Path $Path -History $History -Verbose:$Verbose
if ($patternViolations.Count -gt 0) {
    Write-Error "Pattern violations detected: $($patternViolations.Count)"
    $patternViolations | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}
Write-Host "✅ Pattern compliance validated" -ForegroundColor Green

# 2. Verify against existing abstractions
Write-Host "2. Verifying abstraction compliance..." -ForegroundColor Yellow
$abstractionViolations = Get-AbstractionViolations -Path $Path -History $History -Verbose:$Verbose
if ($abstractionViolations.Count -gt 0) {
    Write-Error "Abstraction violations detected: $($abstractionViolations.Count)"
    $abstractionViolations | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}
Write-Host "✅ Abstraction compliance validated" -ForegroundColor Green

# 3. Check for regressions
Write-Host "3. Checking for regressions..." -ForegroundColor Yellow
$regressions = Get-Regressions -Path $Path -History $History -Verbose:$Verbose
if ($regressions.Count -gt 0) {
    Write-Error "Regressions detected: $($regressions.Count)"
    $regressions | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}
Write-Host "✅ No regressions detected" -ForegroundColor Green

# 4. Validate integration points
Write-Host "4. Validating integration points..." -ForegroundColor Yellow
$integrationIssues = Get-IntegrationIssues -Path $Path -History $History -Verbose:$Verbose
if ($integrationIssues.Count -gt 0) {
    Write-Error "Integration issues detected: $($integrationIssues.Count)"
    $integrationIssues | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}
Write-Host "✅ Integration points validated" -ForegroundColor Green

# 5. Check build health
Write-Host "5. Checking build health..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build IndFusion.sln -c Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed"
        exit 1
    }
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Error "Build validation failed: $_"
    exit 1
}

# 6. Run tests
Write-Host "6. Running tests..." -ForegroundColor Yellow
try {
    $testResult = dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release --no-build --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit 1
    }
    Write-Host "✅ Tests passed" -ForegroundColor Green
} catch {
    Write-Error "Test validation failed: $_"
    exit 1
}

Write-Host "All validations passed!" -ForegroundColor Green

# Helper functions
function Get-PatternViolations {
    param(
        [string]$Path,
        [string]$History,
        [switch]$Verbose
    )
    
    $violations = @()
    
    # Check for new service classes that don't follow naming conventions
    $serviceFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Service\.cs$" }
    foreach ($file in $serviceFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -notmatch "public class \w+Service") {
            $violations += "Service class $($file.Name) doesn't follow naming convention"
        }
    }
    
    # Check for new interfaces that don't follow naming conventions
    $interfaceFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "I\w+\.cs$" }
    foreach ($file in $interfaceFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -notmatch "public interface I\w+") {
            $violations += "Interface $($file.Name) doesn't follow naming convention"
        }
    }
    
    # Check for new adapters that don't follow naming conventions
    $adapterFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Adapter\.cs$" }
    foreach ($file in $adapterFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -notmatch "public class \w+Adapter") {
            $violations += "Adapter class $($file.Name) doesn't follow naming convention"
        }
    }
    
    return $violations
}

function Get-AbstractionViolations {
    param(
        [string]$Path,
        [string]$History,
        [switch]$Verbose
    )
    
    $violations = @()
    
    # Check for concrete dependencies in application layer
    $appFiles = Get-ChildItem -Path "$Path/code/IndFusion.Mcp.Core" -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue
    foreach ($file in $appFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match "using.*Infrastructure" -and $content -notmatch "// TODO: Move to infrastructure") {
            $violations += "Application layer file $($file.Name) has infrastructure dependency"
        }
    }
    
    return $violations
}

function Get-Regressions {
    param(
        [string]$Path,
        [string]$History,
        [switch]$Verbose
    )
    
    $regressions = @()
    
    # Check for breaking changes in public APIs
    $publicFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Public|Interface" }
    foreach ($file in $publicFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match "// BREAKING CHANGE" -and $content -notmatch "// TODO: Update consumers") {
            $regressions += "Breaking change in $($file.Name) without consumer update plan"
        }
    }
    
    return $regressions
}

function Get-IntegrationIssues {
    param(
        [string]$Path,
        [string]$History,
        [switch]$Verbose
    )
    
    $issues = @()
    
    # Check for missing service registrations
    $serviceFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Service\.cs$" }
    $registrationFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "ServiceCollection|DependencyInjection" }
    
    foreach ($serviceFile in $serviceFiles) {
        $serviceName = $serviceFile.BaseName
        $found = $false
        foreach ($regFile in $registrationFiles) {
            $regContent = Get-Content $regFile.FullName -Raw
            if ($regContent -match $serviceName) {
                $found = $true
                break
            }
        }
        if (-not $found) {
            $issues += "Service $serviceName not registered in DI container"
        }
    }
    
    return $issues
}

