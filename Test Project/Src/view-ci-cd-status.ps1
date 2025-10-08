<#
.SYNOPSIS
View the status and logs of the CI/CD watcher for IndTraceV2025
.DESCRIPTION
This script displays the latest status and logs from the CI/CD watcher
.EXAMPLE
.\view-ci-cd-status.ps1
.EXAMPLE
.\view-ci-cd-status.ps1 -Lines 50
#>

param(
    [Parameter(Mandatory=$false)]
    [int]$Lines = 30,

    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$logFile = "$scriptDir\ci-cd-log.txt"
$watcherScriptPath = "$scriptDir\ci-cd-watcher.ps1"
$solutionPath = "E:\Dynamic\IndTrace\IndTraceV2025\Src\IndTrace.sln" # Updated with correct solution path

# Check if log file exists
if (-not (Test-Path $logFile)) {
    Write-Host "CI/CD watcher has not been started yet or no log file exists." -ForegroundColor Yellow
    Write-Host "Run .\start-ci-cd.ps1 to start the watcher." -ForegroundColor Cyan
    exit
}

# Function to find PowerShell processes running our watcher script
function Find-WatcherProcess {
    # Look for PowerShell processes
    $psProcesses = Get-Process -Name powershell, pwsh -ErrorAction SilentlyContinue

    # Try to find our script name in the process list using WMI
    $watcherProcesses = @()
    foreach ($process in $psProcesses) {
        try {
            # Use WMI to get command line
            $processInfo = Get-WmiObject Win32_Process -Filter "ProcessId = $($process.Id)" -ErrorAction SilentlyContinue
            if ($processInfo -and $processInfo.CommandLine -like "*ci-cd-watcher.ps1*") {
                $watcherProcesses += $process
            }
        } catch {
            # Fallback if WMI access fails
            # No action needed, just continue with the next process
        }
    }

    # If we couldn't find it via WMI, check if our log file is recent (within last 5 minutes)
    if ($watcherProcesses.Count -eq 0) {
        $logFileInfo = Get-Item $logFile -ErrorAction SilentlyContinue
        if ($logFileInfo -and $logFileInfo.LastWriteTime -gt (Get-Date).AddMinutes(-5)) {
            Write-Host "CI/CD Watcher appears to be RUNNING (recent log activity)" -ForegroundColor Green
            return $true
        }
        return $false
    }

    return $watcherProcesses
}

# Get status of CI/CD watcher process
$watcherProcess = Find-WatcherProcess

if ($watcherProcess -eq $true) {
    Write-Host "CI/CD Watcher appears to be RUNNING (based on log activity)" -ForegroundColor Green

    # Display additional solution details if verbose
    if ($Verbose) {
        Write-Host "`nMonitoring solution: $solutionPath" -ForegroundColor Cyan
        $lastBuildTime = [DateTime]::MinValue

        # Try to detect the last build time from log
        $buildLines = Get-Content -Path $logFile -ErrorAction SilentlyContinue | Where-Object { $_ -match "Building solution" }
        if ($buildLines.Count -gt 0) {
            $lastBuildLine = $buildLines[-1]
            if ($lastBuildLine -match "\[(.*?)\]") {
                $dateStr = $Matches[1]
                $lastBuildTime = [DateTime]::Parse($dateStr)
                Write-Host "Last build attempt: $($lastBuildTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Cyan
            }
        }
    }
}
elseif ($watcherProcess -and $watcherProcess.Count -gt 0) {
    Write-Host "CI/CD Watcher is RUNNING (PID: $($watcherProcess[0].Id))" -ForegroundColor Green

    # Display additional process details if verbose
    if ($Verbose) {
        Write-Host "  Started: $($watcherProcess[0].StartTime)" -ForegroundColor Cyan
        Write-Host "  Runtime: $([math]::Round(((Get-Date) - $watcherProcess[0].StartTime).TotalMinutes, 1)) minutes" -ForegroundColor Cyan
        Write-Host "  Memory: $([math]::Round($watcherProcess[0].WorkingSet / 1MB, 2)) MB" -ForegroundColor Cyan
    }
}
else {
    Write-Host "CI/CD Watcher is NOT RUNNING" -ForegroundColor Red
    Write-Host "Run .\start-ci-cd.ps1 to start the watcher." -ForegroundColor Cyan
}

# Display last error if any
$errorLines = Get-Content -Path $logFile -ErrorAction SilentlyContinue | Where-Object { $_ -match "\[ERROR\]" }
if ($errorLines.Count -gt 0) {
    Write-Host "`nLast Error:" -ForegroundColor Red
    Write-Host $errorLines[-1] -ForegroundColor Red
}

# Display last build status if any
$buildLines = Get-Content -Path $logFile -ErrorAction SilentlyContinue | Where-Object { $_ -match "Build (successful|failed)" }
if ($buildLines.Count -gt 0) {
    $lastBuildLine = $buildLines[-1]
    if ($lastBuildLine -match "Build successful") {
        Write-Host "`nLast Build: SUCCESSFUL" -ForegroundColor Green
    } else {
        Write-Host "`nLast Build: FAILED" -ForegroundColor Red
    }
}

# Display the most recent log entries
Write-Host "`nRecent Log Entries (last $Lines lines):" -ForegroundColor Cyan
Write-Host "----------------------------------------------" -ForegroundColor Cyan

# Format and display log entries with colors
Get-Content -Path $logFile -Tail $Lines -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_ -match "\[INFO\]") {
        Write-Host $_ -ForegroundColor White
    }
    elseif ($_ -match "\[WARNING\]") {
        Write-Host $_ -ForegroundColor Yellow
    }
    elseif ($_ -match "\[ERROR\]") {
        Write-Host $_ -ForegroundColor Red
    }
    elseif ($_ -match "\[SUCCESS\]") {
        Write-Host $_ -ForegroundColor Green
    }
    else {
        Write-Host $_
    }
}

# Display a summary of monitored projects if verbose
if ($Verbose) {
    Write-Host "`nMonitored Project Types:" -ForegroundColor Cyan
    Write-Host "----------------------------------------------" -ForegroundColor Cyan
    Write-Host "✓ .NET 10 Projects" -ForegroundColor Green
    Write-Host "✓ .NET Standard 2.0 Projects" -ForegroundColor Green
    Write-Host "✓ Blazor Components" -ForegroundColor Green
    Write-Host "✓ Worker Services" -ForegroundColor Green

    Write-Host "`nFor more detailed information about CI/CD setup, see:" -ForegroundColor Cyan
    Write-Host "- $scriptDir\CI-CD-README.md" -ForegroundColor White
}
