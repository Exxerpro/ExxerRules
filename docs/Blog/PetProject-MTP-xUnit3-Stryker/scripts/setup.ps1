# Setup script for Pet Project: MTP 2.0 + xUnit v3 + Stryker.NET Testing
# This script sets up the development environment and installs required tools

Write-Host "=== Pet Project Setup: MTP 2.0 + xUnit v3 + Stryker.NET ===" -ForegroundColor Green

# Check .NET version
Write-Host "Checking .NET version..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "Current .NET version: $dotnetVersion" -ForegroundColor Cyan

if ($dotnetVersion -notlike "10.0.*") {
    Write-Warning "Expected .NET 10.x, but found $dotnetVersion"
    Write-Host "Please install .NET 10 Preview from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Red
    exit 1
}

# Restore tools
Write-Host "Restoring .NET tools..." -ForegroundColor Yellow
dotnet tool restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore .NET tools"
    exit 1
}

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --verbosity normal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed"
    exit 1
}

# Test Stryker.NET
Write-Host "Testing Stryker.NET installation..." -ForegroundColor Yellow
dotnet stryker --version
if ($LASTEXITCODE -ne 0) {
    Write-Error "Stryker.NET not properly installed"
    exit 1
}

Write-Host "=== Setup Complete! ===" -ForegroundColor Green
Write-Host "You can now:" -ForegroundColor Cyan
Write-Host "  1. Run tests: dotnet test" -ForegroundColor White
Write-Host "  2. Run mutation testing: dotnet stryker" -ForegroundColor White
Write-Host "  3. Run console app: dotnet run --project src/Calculator.Console" -ForegroundColor White
Write-Host "  4. View results in: results/stryker/" -ForegroundColor White
