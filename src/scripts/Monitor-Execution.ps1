# Monitor-Execution.ps1
# Monitors execution for off-rails indicators

param(
    [Parameter(Mandatory=$true)]
    [string]$History,
    
    [Parameter(Mandatory=$true)]
    [string]$Phase,
    
    [Parameter(Mandatory=$false)]
    [string]$LogDirectory = "docs/execution"
)

$ErrorActionPreference = "Continue"

$logFile = "$LogDirectory/History$History-$Phase.log"
$startTime = Get-Date

Write-Host "Starting $Phase monitoring for History $History..." -ForegroundColor Green
Write-Host "Logging to: $logFile" -ForegroundColor Yellow

# Ensure log directory exists
if (-not (Test-Path $LogDirectory)) {
    New-Item -ItemType Directory -Path $LogDirectory -Force | Out-Null
}

# Initialize log file
@"
Execution Log - History $History - Phase $Phase
Started: $startTime
User: $env:USERNAME
Working Directory: $(Get-Location)
Computer: $env:COMPUTERNAME

"@ | Out-File -FilePath $logFile -Encoding UTF8

# Define off-rails indicators
$offRailsIndicators = @(
    @{
        Pattern = "Creating new patterns without analyzing existing ones"
        Regex = "new.*pattern|create.*pattern"
        Severity = "High"
    },
    @{
        Pattern = "Implementing without running existing tests"
        Regex = "implement.*without.*test|code.*without.*test"
        Severity = "High"
    },
    @{
        Pattern = "Adding dependencies without checking existing ones"
        Regex = "add.*dependency|new.*dependency"
        Severity = "Medium"
    },
    @{
        Pattern = "Creating new abstractions without understanding existing ones"
        Regex = "new.*abstraction|create.*abstraction"
        Severity = "High"
    },
    @{
        Pattern = "Skipping verification checkpoints"
        Regex = "skip.*checkpoint|ignore.*validation"
        Severity = "Critical"
    },
    @{
        Pattern = "Not following existing naming conventions"
        Regex = "naming.*convention|convention.*naming"
        Severity = "Medium"
    },
    @{
        Pattern = "Breaking existing interfaces without migration plan"
        Regex = "break.*interface|change.*interface"
        Severity = "Critical"
    }
)

# Function to check for off-rails indicators
function Test-OffRailsIndicators {
    param(
        [string]$Content,
        [string]$Source
    )
    
    $detected = @()
    
    foreach ($indicator in $offRailsIndicators) {
        if ($Content -match $indicator.Regex) {
            $detected += @{
                Pattern = $indicator.Pattern
                Severity = $indicator.Severity
                Source = $Source
                Match = $matches[0]
            }
        }
    }
    
    return $detected
}

# Function to monitor file changes
function Start-FileMonitoring {
    param(
        [string]$Path,
        [string]$LogFile
    )
    
    $watcher = New-Object System.IO.FileSystemWatcher
    $watcher.Path = $Path
    $watcher.Filter = "*.cs"
    $watcher.IncludeSubdirectories = $true
    $watcher.EnableRaisingEvents = $true
    
    $action = {
        $path = $Event.SourceEventArgs.FullPath
        $changeType = $Event.SourceEventArgs.ChangeType
        $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        
        try {
            $content = Get-Content $path -Raw -ErrorAction SilentlyContinue
            if ($content) {
                $indicators = Test-OffRailsIndicators -Content $content -Source $path
                
                if ($indicators.Count -gt 0) {
                    foreach ($indicator in $indicators) {
                        $message = "[$timestamp] $($indicator.Severity) - $($indicator.Pattern) in $($indicator.Source)"
                        Write-Warning $message
                        $message | Out-File -FilePath $LogFile -Append -Encoding UTF8
                    }
                }
            }
        } catch {
            # Ignore errors in monitoring
        }
    }
    
    Register-ObjectEvent -InputObject $watcher -EventName "Changed" -Action $action
    Register-ObjectEvent -InputObject $watcher -EventName "Created" -Action $action
    Register-ObjectEvent -InputObject $watcher -EventName "Renamed" -Action $action
    
    return $watcher
}

# Function to monitor console output
function Start-ConsoleMonitoring {
    param(
        [string]$LogFile
    )
    
    $job = Start-Job -ScriptBlock {
        param($LogFile)
        
        while ($true) {
            # Monitor for common off-rails patterns in recent activity
            $recentFiles = Get-ChildItem -Path "src/" -Recurse -Filter "*.cs" | 
                Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-5) }
            
            foreach ($file in $recentFiles) {
                try {
                    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
                    if ($content) {
                        $indicators = Test-OffRailsIndicators -Content $content -Source $file.FullName
                        
                        if ($indicators.Count -gt 0) {
                            foreach ($indicator in $indicators) {
                                $message = "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] $($indicator.Severity) - $($indicator.Pattern) in $($indicator.Source)"
                                $message | Out-File -FilePath $LogFile -Append -Encoding UTF8
                            }
                        }
                    }
                } catch {
                    # Ignore errors in monitoring
                }
            }
            
            Start-Sleep 30
        }
    } -ArgumentList $LogFile
    
    return $job
}

# Start monitoring
Write-Host "Starting file monitoring..." -ForegroundColor Yellow
$fileWatcher = Start-FileMonitoring -Path "src/" -LogFile $logFile

Write-Host "Starting console monitoring..." -ForegroundColor Yellow
$consoleJob = Start-ConsoleMonitoring -LogFile $logFile

Write-Host "Monitoring active. Press Ctrl+C to stop." -ForegroundColor Green
Write-Host "Log file: $logFile" -ForegroundColor Yellow

# Keep the script running
try {
    while ($true) {
        Start-Sleep 1
        
        # Check if monitoring jobs are still running
        if ($consoleJob.State -ne "Running") {
            Write-Warning "Console monitoring job stopped unexpectedly"
            $consoleJob = Start-ConsoleMonitoring -LogFile $logFile
        }
    }
} finally {
    # Cleanup
    Write-Host "Stopping monitoring..." -ForegroundColor Yellow
    
    if ($fileWatcher) {
        $fileWatcher.EnableRaisingEvents = $false
        $fileWatcher.Dispose()
    }
    
    if ($consoleJob) {
        Stop-Job $consoleJob
        Remove-Job $consoleJob
    }
    
    # Log completion
    $endTime = Get-Date
    $duration = $endTime - $startTime
    "Monitoring stopped at $endTime (Duration: $($duration.TotalMinutes) minutes)" | Out-File -FilePath $logFile -Append -Encoding UTF8
    
    Write-Host "Monitoring stopped." -ForegroundColor Green
}

