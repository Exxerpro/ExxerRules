param(
    [Parameter(Mandatory=$true)]
    [string]$Command,
    
    [Parameter(Mandatory=$false)]
    [int]$TimeoutSeconds = 300,  # 5 minutes default
    
    [Parameter(Mandatory=$false)]
    [string]$WorkingDirectory = "."
)

# Function to run command with timeout
function Invoke-CommandWithTimeout {
    param(
        [string]$Command,
        [int]$TimeoutSeconds,
        [string]$WorkingDirectory
    )
    
    Write-Host "Running command: $Command" -ForegroundColor Green
    Write-Host "Timeout: $TimeoutSeconds seconds" -ForegroundColor Yellow
    Write-Host "Working directory: $WorkingDirectory" -ForegroundColor Yellow
    Write-Host "Starting at: $(Get-Date)" -ForegroundColor Cyan
    
    try {
        # Create job to run the command
        $job = Start-Job -ScriptBlock {
            param($cmd, $wd)
            Set-Location $wd
            & cmd /c $cmd
        } -ArgumentList $Command, $WorkingDirectory
        
        # Wait for job with timeout
        $result = Wait-Job -Job $job -Timeout $TimeoutSeconds
        
        if ($result -eq $null) {
            # Timeout occurred
            Write-Host "Command timed out after $TimeoutSeconds seconds" -ForegroundColor Red
            Stop-Job -Job $job -ErrorAction SilentlyContinue
            Remove-Job -Job $job -ErrorAction SilentlyContinue
            exit 1
        }
        
        # Get job results
        $output = Receive-Job -Job $job
        $exitCode = $job.ExitCode
        
        # Clean up job
        Remove-Job -Job $job
        
        # Output results
        Write-Host "Command completed at: $(Get-Date)" -ForegroundColor Cyan
        Write-Host "Exit code: $exitCode" -ForegroundColor $(if ($exitCode -eq 0) { "Green" } else { "Red" })
        
        # Display output
        if ($output) {
            Write-Host "Output:" -ForegroundColor White
            $output | ForEach-Object { Write-Host $_ }
        }
        
        return $exitCode
        
    } catch {
        Write-Host "Error running command: $($_.Exception.Message)" -ForegroundColor Red
        return 1
    }
}

# Main execution
$exitCode = Invoke-CommandWithTimeout -Command $Command -TimeoutSeconds $TimeoutSeconds -WorkingDirectory $WorkingDirectory
exit $exitCode
