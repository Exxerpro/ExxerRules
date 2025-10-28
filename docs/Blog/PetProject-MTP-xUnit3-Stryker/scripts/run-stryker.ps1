# Stryker.NET mutation testing script for Pet Project
# Runs mutation testing and generates reports

Write-Host "=== Running Stryker.NET Mutation Testing ===" -ForegroundColor Green

# Create results directory
$resultsDir = "results/stryker"
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

# Change to test project directory
Push-Location "src/Calculator.Tests"

try {
    # Run Stryker.NET
    Write-Host "Running Stryker.NET mutation testing..." -ForegroundColor Yellow
    Write-Host "This may take several minutes..." -ForegroundColor Cyan
    
    dotnet stryker `
        --config-file-path "../../tools/stryker-config.json" `
        --output-path "../../$resultsDir" `
        --verbosity info

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Stryker.NET execution failed"
        exit 1
    }

    Write-Host "=== Stryker.NET Execution Complete! ===" -ForegroundColor Green
    Write-Host "Results available in: $resultsDir" -ForegroundColor Cyan
    Write-Host "Open: $resultsDir/mutation-report.html" -ForegroundColor White

} finally {
    Pop-Location
}

# Display summary
$reportFile = Get-ChildItem -Path $resultsDir -Filter "mutation-report.html" -Recurse | Select-Object -First 1
if ($reportFile) {
    Write-Host "Mutation testing report: $($reportFile.FullName)" -ForegroundColor Cyan
} else {
    Write-Warning "No mutation report found"
}
