# Test execution script for Pet Project
# Runs all tests with detailed output and coverage

Write-Host "=== Running Tests: MTP 2.0 + xUnit v3 ===" -ForegroundColor Green

# Create results directory
$resultsDir = "results"
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir | Out-Null
}

# Run tests with coverage
Write-Host "Running tests with code coverage..." -ForegroundColor Yellow
dotnet test `
    --collect:"XPlat Code Coverage" `
    --results-directory "$resultsDir/test-results" `
    --logger "trx;LogFileName=test-results.trx" `
    --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed"
    exit 1
}

# Generate coverage report
Write-Host "Generating coverage report..." -ForegroundColor Yellow
$coverageFile = Get-ChildItem -Path "$resultsDir/test-results" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1
if ($coverageFile) {
    reportgenerator `
        -reports:"$($coverageFile.FullName)" `
        -targetdir:"$resultsDir/coverage" `
        -reporttypes:"Html;TextSummary"
    
    Write-Host "Coverage report generated in: $resultsDir/coverage/index.html" -ForegroundColor Cyan
} else {
    Write-Warning "No coverage file found"
}

Write-Host "=== Test Execution Complete! ===" -ForegroundColor Green
