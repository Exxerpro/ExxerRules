param(
    [Parameter(Mandatory=$true)]
    [string]$Command,

    [Parameter(Mandatory=$false)]
    [int]$TimeoutSeconds = 300
)

Write-Host "Running command: $Command" -ForegroundColor Green
Write-Host "Timeout: $TimeoutSeconds seconds" -ForegroundColor Yellow
Write-Host "Starting at: $(Get-Date)" -ForegroundColor Cyan

try {
    # Create process start info
    $processInfo = New-Object System.Diagnostics.ProcessStartInfo
    $processInfo.FileName = "cmd.exe"
    $processInfo.Arguments = "/c $Command"
    $processInfo.UseShellExecute = $false
    $processInfo.RedirectStandardOutput = $true
    $processInfo.RedirectStandardError = $true
    $processInfo.CreateNoWindow = $false

    # Start the process
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $processInfo
    $process.Start() | Out-Null

    # Wait for completion with timeout
    $completed = $process.WaitForExit($TimeoutSeconds * 1000)

    if (-not $completed) {
        Write-Host "Command timed out after $TimeoutSeconds seconds" -ForegroundColor Red
        $process.Kill()
        exit 1
    }

    # Get output
    $output = $process.StandardOutput.ReadToEnd()
    $errorOutput = $process.StandardError.ReadToEnd()
    $exitCode = $process.ExitCode

    # Display results
    Write-Host "Command completed at: $(Get-Date)" -ForegroundColor Cyan
    Write-Host "Exit code: $exitCode" -ForegroundColor $(if ($exitCode -eq 0) { "Green" } else { "Red" })

    if ($output) {
        Write-Host "Output:" -ForegroundColor White
        Write-Host $output
    }

    if ($errorOutput) {
        Write-Host "Error output:" -ForegroundColor Red
        Write-Host $errorOutput
    }

    exit $exitCode

} catch {
    Write-Host "Error running command: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
