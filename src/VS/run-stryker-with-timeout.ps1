param(
    [int]$TimeoutSeconds = 900  # 15 minutes default
)

Write-Host "Running Stryker with timeout of $TimeoutSeconds seconds..." -ForegroundColor Green
Write-Host "Starting at: $(Get-Date)" -ForegroundColor Cyan

try {
    # Create process start info
    $processInfo = New-Object System.Diagnostics.ProcessStartInfo
    $processInfo.FileName = "dotnet"
    $processInfo.Arguments = "stryker --reporters Progress,Html,Json --test-project ../ExxerRules.Tests"
    $processInfo.UseShellExecute = $false
    $processInfo.RedirectStandardOutput = $true
    $processInfo.RedirectStandardError = $true
    $processInfo.CreateNoWindow = $false
    $processInfo.WorkingDirectory = Get-Location

    # Start the process
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $processInfo
    $process.Start() | Out-Null

    Write-Host "Stryker process started with PID: $($process.Id)" -ForegroundColor Yellow

    # Wait for completion with timeout
    $completed = $process.WaitForExit($TimeoutSeconds * 1000)

    if (-not $completed) {
        Write-Host "Stryker timed out after $TimeoutSeconds seconds - killing process..." -ForegroundColor Red
        $process.Kill()
        exit 1
    }

    # Get output
    $output = $process.StandardOutput.ReadToEnd()
    $error = $process.StandardError.ReadToEnd()
    $exitCode = $process.ExitCode

    # Display results
    Write-Host "Stryker completed at: $(Get-Date)" -ForegroundColor Cyan
    Write-Host "Exit code: $exitCode" -ForegroundColor $(if ($exitCode -eq 0) { "Green" } else { "Red" })

    if ($output) {
        Write-Host "Output:" -ForegroundColor White
        Write-Host $output
    }

    if ($error) {
        Write-Host "Error output:" -ForegroundColor Red
        Write-Host $error
    }

    exit $exitCode

} catch {
    Write-Host "Error running Stryker: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
