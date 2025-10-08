<#
.SYNOPSIS
Starts the CI/CD watcher for IndTraceV2025
.DESCRIPTION
This script starts the CI/CD watcher with proper configuration
.EXAMPLE
.\start-ci-cd.ps1
.EXAMPLE
.\start-ci-cd.ps1 -SkipInitialBuild
#>

param(
    [Parameter(Mandatory=$false)]
    [switch]$SkipInitialBuild
)

$ErrorActionPreference = "Stop"

# Configuration
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionDir = Split-Path -Parent $scriptDir
$solutionFile = "$solutionDir\Src\IndTrace.sln"
$watcherScript = "$scriptDir\ci-cd-watcher.ps1"
$logFile = "$scriptDir\ci-cd-log.txt"

# Make sure we're in the right directory
Set-Location $scriptDir

# Create log file if it doesn't exist
if (-not (Test-Path $logFile)) {
    "" | Out-File -FilePath $logFile
}

# Display startup banner
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "        IndTraceV2025 CI/CD Watcher Startup         " -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Solution:     $solutionFile" -ForegroundColor White
Write-Host "Log File:     $logFile" -ForegroundColor White
Write-Host "Script Path:  $watcherScript" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop monitoring" -ForegroundColor Yellow
Write-Host ""

# Validate environment
Write-Host "Validating environment..." -ForegroundColor Cyan

# Check if solution exists
if (-not (Test-Path $solutionFile)) {
    Write-Host "ERROR: Solution file not found at: $solutionFile" -ForegroundColor Red
    Write-Host "Please check your directory structure and update the script if necessary." -ForegroundColor Red
    exit 1
}

# Check for .NET SDK
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET SDK Version: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: .NET SDK not found or not in path" -ForegroundColor Red
    Write-Host "Please install the .NET SDK and try again." -ForegroundColor Red
    exit 1
}

# Check for Git
try {
    $gitVersion = git --version
    Write-Host "Git Version: $gitVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Git not found or not in path" -ForegroundColor Red
    Write-Host "Please install Git and try again." -ForegroundColor Red
    exit 1
}

# Perform initial build to validate solution compilation
if (-not $SkipInitialBuild) {
    Write-Host "Performing initial build to validate solution..." -ForegroundColor Cyan

    try {
        dotnet build $solutionFile --configuration Debug --verbosity minimal

        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: Initial build failed with exit code $LASTEXITCODE" -ForegroundColor Red
            Write-Host "Please fix build errors before starting the CI/CD watcher." -ForegroundColor Red

            $continue = Read-Host "Continue anyway? (y/n)"
            if ($continue -ne "y") {
                exit 1
            }
        } else {
            Write-Host "Initial build successful!" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "ERROR: Failed to build solution: $_" -ForegroundColor Red

        $continue = Read-Host "Continue anyway? (y/n)"
        if ($continue -ne "y") {
            exit 1
        }
    }
}

# Run the watcher script
try {
    Write-Host "Starting CI/CD Watcher..." -ForegroundColor Green
    & "$scriptDir\ci-cd-watcher.ps1"
}
catch {
    Write-Host "Error running CI/CD watcher: $_" -ForegroundColor Red
}
