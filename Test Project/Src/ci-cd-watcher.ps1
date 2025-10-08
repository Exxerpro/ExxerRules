<#
.SYNOPSIS
CI/CD automation script for IndTraceV2025 solution
.DESCRIPTION
This script watches for changes in the solution, builds it, and commits if successful.
It provides a continuous integration workflow that:
1. Detects code changes
2. Builds the full solution
3. Only commits changes when the build is successful
4. Creates meaningful commit messages
5. Waits for subsequent changes
.EXAMPLE
.\ci-cd-watcher.ps1
#>

# Configuration
$solutionDir = Split-Path -Parent $PSScriptRoot
$solutionFile = "$solutionDir\Src\IndTrace.sln" # Updated with correct solution path
$srcDir = "$solutionDir\Src"
$buildDir = "$solutionDir\build"
$lastChangeTime = Get-Date
$checkInterval = 5 # Time in seconds between checking for changes
$fileExtensionsToWatch = @(".cs", ".razor", ".cshtml", ".csproj", ".json", ".config")
$logFile = "$PSScriptRoot\ci-cd-log.txt"

# Configure logging
function Write-Log {
    param (
        [Parameter(Mandatory=$true)]
        [string]$Message,

        [Parameter(Mandatory=$false)]
        [ValidateSet("INFO", "WARNING", "ERROR", "SUCCESS")]
        [string]$Level = "INFO"
    )

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"

    # Write to console with color based on level
    switch ($Level) {
        "INFO"    { Write-Host $logMessage -ForegroundColor White }
        "WARNING" { Write-Host $logMessage -ForegroundColor Yellow }
        "ERROR"   { Write-Host $logMessage -ForegroundColor Red }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
    }

    # Write to log file
    Add-Content -Path $logFile -Value $logMessage
}

# Function to check for file changes
function Get-FileChanges {
    $changedFiles = @()

    # Get all files with specified extensions
    Get-ChildItem -Path $srcDir -Recurse -File |
        Where-Object {
            $fileExtensionsToWatch -contains $_.Extension -and
            $_.LastWriteTime -gt $lastChangeTime -and
            # Exclude build artifacts and bin/obj directories
            $_.FullName -notmatch '[\\/]bin[\\/]' -and
            $_.FullName -notmatch '[\\/]obj[\\/]' -and
            $_.FullName -notmatch '[\\/]\.vs[\\/]'
        } |
        ForEach-Object {
            $changedFiles += $_.FullName
        }

    return $changedFiles
}

# Function to build solution
function Build-Solution {
    Write-Log "Building solution: $solutionFile" "INFO"

    try {
        # Verify solution file exists
        if (-not (Test-Path $solutionFile)) {
            Write-Log "Solution file not found at: $solutionFile" "ERROR"
            return $false
        }

        # Clean and restore first
        Write-Log "Cleaning solution..." "INFO"
        $cleanOutput = dotnet clean $solutionFile --verbosity quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Clean failed with exit code $LASTEXITCODE" "ERROR"
            return $false
        }

        Write-Log "Restoring packages..." "INFO"
        $restoreOutput = dotnet restore $solutionFile --verbosity quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Restore failed with exit code $LASTEXITCODE" "ERROR"
            Write-Log ($restoreOutput -join "`n") "ERROR"
            return $false
        }

        # Build the solution
        Write-Log "Building solution..." "INFO"
        $buildOutput = dotnet build $solutionFile --configuration Release --verbosity minimal
        $buildSuccess = $LASTEXITCODE -eq 0

        # Log build output
        if ($buildSuccess) {
            Write-Log "Build successful" "SUCCESS"

            # Extract warnings from build output
            $warningLines = $buildOutput | Where-Object { $_ -match "warning " }
            if ($warningLines.Count -gt 0) {
                Write-Log "Build completed with $($warningLines.Count) warnings" "WARNING"
                foreach ($warning in $warningLines | Select-Object -First 5) {
                    Write-Log "Warning: $warning" "WARNING"
                }

                if ($warningLines.Count -gt 5) {
                    Write-Log "... and $($warningLines.Count - 5) more warnings" "WARNING"
                }
            }
        } else {
            Write-Log "Build failed with exit code $LASTEXITCODE" "ERROR"

            # Extract and log errors from build output
            $errorLines = $buildOutput | Where-Object { $_ -match "error " }
            foreach ($error in $errorLines) {
                Write-Log $error "ERROR"
            }
        }

        return $buildSuccess
    }
    catch {
        Write-Log "Exception building solution: $_" "ERROR"
        return $false
    }
}

# Function to run tests
function Run-Tests {
    Write-Log "Running tests..." "INFO"

    try {
        $testOutput = dotnet test $solutionFile --no-build --verbosity minimal
        $testSuccess = $LASTEXITCODE -eq 0

        if ($testSuccess) {
            Write-Log "All tests passed" "SUCCESS"
        } else {
            Write-Log "Tests failed with exit code $LASTEXITCODE" "WARNING"

            # Extract and log test failures
            $failureLines = $testOutput | Where-Object { $_ -match "Failed" -or $_ -match "Error" }
            foreach ($failure in $failureLines) {
                Write-Log $failure "WARNING"
            }
        }

        return $testSuccess
    }
    catch {
        Write-Log "Exception running tests: $_" "ERROR"
        return $false
    }
}

# Function to generate a commit message for changes
function Get-CommitMessage {
    param (
        [Parameter(Mandatory=$true)]
        [string[]]$ChangedFiles
    )

    try {
        # First try using GitHub Copilot CLI if available
        Write-Log "Attempting to generate commit message using Copilot..." "INFO"
        $copilotAvailable = Get-Command "copilot" -ErrorAction SilentlyContinue

        if ($copilotAvailable) {
            # Use Copilot to generate a commit message
            return & copilot commit --no-commit
        }
    }
    catch {
        Write-Log "Copilot not available, generating conventional message" "WARNING"
    }

    # Default message generation logic based on changed files
    $mainChangeType = "feat"  # Default to feature
    $fileTypes = @{
        "feat" = @()
        "fix" = @()
        "refactor" = @()
        "test" = @()
        "docs" = @()
        "style" = @()
        "chore" = @()
        "blazor" = @() # Added Blazor-specific category
        "worker" = @() # Added Worker Service category
    }

    # Categorize changes by path patterns
    foreach ($file in $ChangedFiles) {
        $relativePath = $file.Replace("$solutionDir\", "")

        # Special handling for Blazor components
        if ($relativePath -match "\.razor$") {
            $fileTypes["blazor"] += $relativePath
            continue
        }

        # Special handling for Worker Services
        if ($relativePath -match "Worker\.cs$" -or (Get-Content $file -ErrorAction SilentlyContinue | Select-String -Pattern "BackgroundService" -SimpleMatch)) {
            $fileTypes["worker"] += $relativePath
            continue
        }

        # Standard categorization
        if ($relativePath -match "Tests") {
            $fileTypes["test"] += $relativePath
        }
        elseif ($relativePath -match "\.md$") {
            $fileTypes["docs"] += $relativePath
        }
        elseif ($relativePath -match "\.cs$" -and (Get-Content $file -ErrorAction SilentlyContinue | Select-String -Pattern "bug fix|fix|fixes|fixed" -SimpleMatch)) {
            $fileTypes["fix"] += $relativePath
        }
        elseif ($relativePath -match "\.cs$" -and (Get-Content $file -ErrorAction SilentlyContinue | Select-String -Pattern "refactor|rewrite|cleanup|clean-up" -SimpleMatch)) {
            $fileTypes["refactor"] += $relativePath
        }
        elseif ($relativePath -match "\.cs$") {
            $fileTypes["feat"] += $relativePath
        }
        else {
            $fileTypes["chore"] += $relativePath
        }
    }

    # Determine primary change type (prioritize certain types)
    $priorityTypes = @("fix", "blazor", "worker", "feat", "refactor", "test", "docs", "style", "chore")
    foreach ($type in $priorityTypes) {
        if ($fileTypes[$type].Count -gt 0) {
            $mainChangeType = $type
            break
        }
    }

    # Remap some types to conventional commit types
    if ($mainChangeType -eq "blazor") { $mainChangeType = "feat" }
    if ($mainChangeType -eq "worker") { $mainChangeType = "feat" }

    # Get the most common directory for changes
    $directories = $ChangedFiles | ForEach-Object {
        $parts = ($_.Replace("$solutionDir\", "")) -split "\\"
        if ($parts.Count -ge 2) { $parts[0..1] -join "\" }
        else { $parts[0] }
    } | Group-Object | Sort-Object -Property Count -Descending

    $scope = ""
    if ($directories.Count -gt 0) {
        $scope = "($($directories[0].Name))"
    }

    # Create summary based on the change type and special categories
    $summary = if ($fileTypes["blazor"].Count -gt 0) {
        "update Blazor components in "
    } elseif ($fileTypes["worker"].Count -gt 0) {
        "update worker services in "
    } else {
        switch ($mainChangeType) {
            "fix" { "fix bugs in " }
            "feat" { "add new functionality to " }
            "refactor" { "refactor code in " }
            "test" { "update tests for " }
            "docs" { "update documentation for " }
            "style" { "improve code style in " }
            "chore" { "update configuration for " }
            default { "update " }
        }
    }

    $summary += $directories[0].Name

    # Generate a conventional commit message
    $message = "$mainChangeType$scope: $summary`n`n"

    # Add details about changes
    if ($fileTypes["blazor"].Count -gt 0) {
        $message += "Blazor Components:`n"
        $message += $fileTypes["blazor"] | ForEach-Object { "- $_" } | Out-String
        $message += "`n"
    }

    if ($fileTypes["worker"].Count -gt 0) {
        $message += "Worker Services:`n"
        $message += $fileTypes["worker"] | ForEach-Object { "- $_" } | Out-String
        $message += "`n"
    }

    if ($fileTypes["feat"].Count -gt 0) {
        $message += "Features:`n"
        $message += $fileTypes["feat"] | ForEach-Object { "- $_" } | Out-String
        $message += "`n"
    }

    if ($fileTypes["fix"].Count -gt 0) {
        $message += "Fixes:`n"
        $message += $fileTypes["fix"] | ForEach-Object { "- $_" } | Out-String
        $message += "`n"
    }

    if ($fileTypes["refactor"].Count -gt 0) {
        $message += "Refactoring:`n"
        $message += $fileTypes["refactor"] | ForEach-Object { "- $_" } | Out-String
        $message += "`n"
    }

    if ($fileTypes["test"].Count -gt 0) {
        $message += "Tests:`n"
        $message += $fileTypes["test"] | ForEach-Object { "- $_" } | Out-String
    }

    return $message
}

# Function to commit changes
function Commit-Changes {
    param (
        [Parameter(Mandatory=$true)]
        [string[]]$ChangedFiles
    )

    try {
        Write-Log "Staging changes..." "INFO"
        git add .

        if ($LASTEXITCODE -ne 0) {
            Write-Log "Failed to stage changes: $LASTEXITCODE" "ERROR"
            return $false
        }

        # Generate commit message
        $commitMessage = Get-CommitMessage -ChangedFiles $ChangedFiles

        # Commit with the generated message
        Write-Log "Committing with message:`n$commitMessage" "INFO"
        git commit -m "$commitMessage"

        if ($LASTEXITCODE -ne 0) {
            Write-Log "Failed to commit changes: $LASTEXITCODE" "ERROR"
            return $false
        }

        # Push changes (optional - uncomment if you want to automatically push)
        # Write-Log "Pushing changes to remote..." "INFO"
        # git push
        # if ($LASTEXITCODE -ne 0) {
        #     Write-Log "Failed to push changes: $LASTEXITCODE" "ERROR"
        #     return $false
        # }

        Write-Log "Successfully committed changes" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Error committing changes: $_" "ERROR"
        return $false
    }
}

# Main execution loop
Write-Log "Starting CI/CD watcher for IndTraceV2025" "INFO"
Write-Log "Press Ctrl+C to stop the watcher" "INFO"
Write-Log "Monitoring for changes in: $srcDir" "INFO"
Write-Log "Solution path: $solutionFile" "INFO"

# Initial validation of solution file
if (-not (Test-Path $solutionFile)) {
    Write-Log "ERROR: Solution file not found at: $solutionFile" "ERROR"
    Write-Log "Please check the solution path and update the script if necessary" "ERROR"
    exit 1
}

try {
    while ($true) {
        $changedFiles = Get-FileChanges

        if ($changedFiles.Count -gt 0) {
            Write-Log "Detected $($changedFiles.Count) changed files" "INFO"

            # Report file types that were changed
            $extensions = $changedFiles | ForEach-Object { [System.IO.Path]::GetExtension($_) } | Group-Object | Sort-Object -Property Count -Descending
            $extensionSummary = $extensions | ForEach-Object { "$($_.Count) $($_.Name) files" }
            Write-Log "Changed file types: $($extensionSummary -join ', ')" "INFO"

            # Build the solution
            $buildSuccess = Build-Solution

            if ($buildSuccess) {
                # Run tests
                $testsSuccess = Run-Tests

                # Commit changes if build was successful (regardless of test results)
                $commitSuccess = Commit-Changes -ChangedFiles $changedFiles

                if ($commitSuccess) {
                    Write-Log "Changes committed successfully" "SUCCESS"
                } else {
                    Write-Log "Failed to commit changes" "ERROR"
                }
            } else {
                Write-Log "Build failed - changes not committed" "WARNING"
            }

            # Update last change time
            $lastChangeTime = Get-Date
        }

        # Wait before checking again
        Start-Sleep -Seconds $checkInterval
    }
}
catch {
    Write-Log "Error in CI/CD watcher: $_" "ERROR"
}
finally {
    Write-Log "CI/CD watcher stopped" "INFO"
}
