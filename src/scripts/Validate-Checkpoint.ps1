# Validate-Checkpoint.ps1
# Validates specific checkpoints during implementation

param(
    [Parameter(Mandatory=$true)]
    [string]$History,
    
    [Parameter(Mandatory=$true)]
    [string]$Checkpoint,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet("PatternCompliance", "AbstractionCompliance", "IntegrationValidation", "BuildValidation", "TestValidation")]
    [string]$ValidationType,
    
    [Parameter(Mandatory=$false)]
    [string]$Path = "src/",
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

$checkpointFile = "docs/execution/History$History-Checkpoint$Checkpoint.md"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

Write-Host "Validating History $History - Checkpoint $Checkpoint - Type: $ValidationType" -ForegroundColor Green

# Ensure execution directory exists
$executionDir = "docs/execution"
if (-not (Test-Path $executionDir)) {
    New-Item -ItemType Directory -Path $executionDir -Force | Out-Null
}

# Initialize checkpoint file if it doesn't exist
if (-not (Test-Path $checkpointFile)) {
    @"
# History $History - Checkpoint $Checkpoint

## Validation Log

"@ | Out-File -FilePath $checkpointFile -Encoding UTF8
}

switch ($ValidationType) {
    "PatternCompliance" {
        Write-Host "Validating pattern compliance..." -ForegroundColor Yellow
        
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
        
        if ($violations.Count -eq 0) {
            Write-Host "✅ Pattern compliance validated" -ForegroundColor Green
            "✅ Pattern compliance validated at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
        } else {
            Write-Error "❌ Pattern violations detected: $($violations.Count)"
            "❌ Pattern violations detected: $($violations.Count) at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            $violations | ForEach-Object { 
                Write-Host "  - $_" -ForegroundColor Red
                "  - $_" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            }
            exit 1
        }
    }
    
    "AbstractionCompliance" {
        Write-Host "Validating abstraction compliance..." -ForegroundColor Yellow
        
        $violations = @()
        
        # Check for concrete dependencies in application layer
        $appFiles = Get-ChildItem -Path "$Path/code/IndFusion.Mcp.Core" -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue
        foreach ($file in $appFiles) {
            $content = Get-Content $file.FullName -Raw
            if ($content -match "using.*Infrastructure" -and $content -notmatch "// TODO: Move to infrastructure") {
                $violations += "Application layer file $($file.Name) has infrastructure dependency"
            }
        }
        
        # Check for missing interface implementations
        $serviceFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Service\.cs$" }
        foreach ($file in $serviceFiles) {
            $content = Get-Content $file.FullName -Raw
            if ($content -match "public class \w+Service" -and $content -notmatch "implements|:" -and $content -notmatch "// TODO: Add interface") {
                $violations += "Service class $($file.Name) doesn't implement an interface"
            }
        }
        
        if ($violations.Count -eq 0) {
            Write-Host "✅ Abstraction compliance validated" -ForegroundColor Green
            "✅ Abstraction compliance validated at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
        } else {
            Write-Error "❌ Abstraction violations detected: $($violations.Count)"
            "❌ Abstraction violations detected: $($violations.Count) at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            $violations | ForEach-Object { 
                Write-Host "  - $_" -ForegroundColor Red
                "  - $_" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            }
            exit 1
        }
    }
    
    "IntegrationValidation" {
        Write-Host "Validating integration points..." -ForegroundColor Yellow
        
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
        
        # Check for missing configuration sections
        $configFiles = Get-ChildItem -Path $Path -Recurse -Filter "*.cs" | Where-Object { $_.Name -match "Options\.cs$" }
        $appSettingsFiles = Get-ChildItem -Path $Path -Recurse -Filter "appsettings*.json" -ErrorAction SilentlyContinue
        
        foreach ($configFile in $configFiles) {
            $configName = $configFile.BaseName
            $found = $false
            foreach ($appFile in $appSettingsFiles) {
                $appContent = Get-Content $appFile.FullName -Raw
                if ($appContent -match $configName) {
                    $found = $true
                    break
                }
            }
            if (-not $found) {
                $issues += "Configuration $configName not found in appsettings"
            }
        }
        
        if ($issues.Count -eq 0) {
            Write-Host "✅ Integration validation passed" -ForegroundColor Green
            "✅ Integration validation passed at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
        } else {
            Write-Error "❌ Integration issues detected: $($issues.Count)"
            "❌ Integration issues detected: $($issues.Count) at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            $issues | ForEach-Object { 
                Write-Host "  - $_" -ForegroundColor Red
                "  - $_" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            }
            exit 1
        }
    }
    
    "BuildValidation" {
        Write-Host "Validating build..." -ForegroundColor Yellow
        
        try {
            $buildResult = dotnet build IndFusion.sln -c Release --no-restore
            if ($LASTEXITCODE -ne 0) {
                Write-Error "❌ Build failed"
                "❌ Build failed at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
                exit 1
            }
            Write-Host "✅ Build successful" -ForegroundColor Green
            "✅ Build successful at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
        } catch {
            Write-Error "❌ Build validation failed: $_"
            "❌ Build validation failed: $_ at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            exit 1
        }
    }
    
    "TestValidation" {
        Write-Host "Validating tests..." -ForegroundColor Yellow
        
        try {
            $testResult = dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release --no-build --verbosity minimal
            if ($LASTEXITCODE -ne 0) {
                Write-Error "❌ Tests failed"
                "❌ Tests failed at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
                exit 1
            }
            Write-Host "✅ Tests passed" -ForegroundColor Green
            "✅ Tests passed at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
        } catch {
            Write-Error "❌ Test validation failed: $_"
            "❌ Test validation failed: $_ at $timestamp" | Out-File -FilePath $checkpointFile -Append -Encoding UTF8
            exit 1
        }
    }
}

Write-Host "Checkpoint validation completed successfully!" -ForegroundColor Green

