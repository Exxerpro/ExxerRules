<#
.SYNOPSIS
    Runs a single test with timeout and detailed output.

.DESCRIPTION
    This script runs a single test method with a configurable timeout.
    If the test exceeds the timeout, it will be forcefully terminated.
    Useful for debugging hanging tests.

.PARAMETER TestFullyQualifiedName
    The fully qualified name of the test method to run (e.g., "IndFusion.SemanticRag.Tests.Unit.Implementations.QdrantVectorDatabaseAdapterTests.SearchAsync_WithNullQueryVector_ShouldReturnFailure").

.PARAMETER TimeoutSeconds
    The maximum time in seconds to wait for the test to complete. Default is 60 seconds.

.PARAMETER ProjectPath
    The path to the test project. Default is "src/IndFusion.SemanticRag.Tests.Unit/IndFusion.SemanticRag.Tests.Unit.csproj".

.EXAMPLE
    .\RunSingleTest.ps1 -TestFullyQualifiedName "IndFusion.SemanticRag.Tests.Unit.Implementations.QdrantVectorDatabaseAdapterTests.SearchAsync_WithNullQueryVector_ShouldReturnFailure"

.EXAMPLE
    .\RunSingleTest.ps1 -TestFullyQualifiedName "MyNamespace.MyTestClass.MyTestMethod" -TimeoutSeconds 30
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$TestFullyQualifiedName,
    
    [Parameter(Mandatory=$false)]
    [int]$TimeoutSeconds = 60,
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/IndFusion.SemanticRag.Tests.Unit/IndFusion.SemanticRag.Tests.Unit.csproj"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Running Single Test with Timeout ===" -ForegroundColor Cyan
Write-Host "Test: $TestFullyQualifiedName" -ForegroundColor Yellow
Write-Host "Timeout: ${TimeoutSeconds}s" -ForegroundColor Yellow
Write-Host "Project: $ProjectPath" -ForegroundColor Yellow
Write-Host ""

# Build the test command
$testCommand = "dotnet test `"$ProjectPath`" --filter `"FullyQualifiedName=$TestFullyQualifiedName`" --verbosity normal --logger `"console;verbosity=detailed`""

Write-Host "Command: $testCommand" -ForegroundColor Gray
Write-Host ""

# Execute the test with timeout
$startTime = Get-Date
$process = Start-Process -FilePath "dotnet" -ArgumentList @(
    "test",
    "`"$ProjectPath`"",
    "--filter",
    "`"FullyQualifiedName=$TestFullyQualifiedName`"",
    "--verbosity",
    "normal",
    "--logger",
    "`"console;verbosity=detailed`""
) -NoNewWindow -PassThru -RedirectStandardOutput "test-output.txt" -RedirectStandardError "test-error.txt"

try
{
    # Wait for process to complete or timeout
    $completed = $process.WaitForExit($TimeoutSeconds * 1000)
    
    if ($completed)
    {
        $endTime = Get-Date
        $duration = ($endTime - $startTime).TotalSeconds
        
        Write-Host "=== Test Completed ===" -ForegroundColor Green
        Write-Host "Duration: ${duration}s" -ForegroundColor Green
        Write-Host ""
        
        # Display output
        if (Test-Path "test-output.txt")
        {
            Write-Host "=== Test Output ===" -ForegroundColor Cyan
            Get-Content "test-output.txt" | Write-Host
            Remove-Item "test-output.txt" -ErrorAction SilentlyContinue
        }
        
        if (Test-Path "test-error.txt")
        {
            $errorContent = Get-Content "test-error.txt" -Raw
            if (![string]::IsNullOrWhiteSpace($errorContent))
            {
                Write-Host "=== Test Errors ===" -ForegroundColor Red
                Write-Host $errorContent -ForegroundColor Red
            }
            Remove-Item "test-error.txt" -ErrorAction SilentlyContinue
        }
        
        exit $process.ExitCode
    }
    else
    {
        # Test timed out - kill the process
        Write-Host "=== Test Timed Out ===" -ForegroundColor Red
        Write-Host "Test exceeded timeout of ${TimeoutSeconds}s" -ForegroundColor Red
        Write-Host "Terminating test process..." -ForegroundColor Yellow
        
        $process.Kill()
        $process.WaitForExit(5000)  # Wait up to 5 seconds for graceful shutdown
        
        if (!$process.HasExited)
        {
            Write-Host "Process did not terminate gracefully. Force killing..." -ForegroundColor Red
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        }
        
        # Display partial output if available
        if (Test-Path "test-output.txt")
        {
            Write-Host "=== Partial Test Output ===" -ForegroundColor Yellow
            Get-Content "test-output.txt" -Tail 50 | Write-Host
            Remove-Item "test-output.txt" -ErrorAction SilentlyContinue
        }
        
        exit 1
    }
}
catch
{
    Write-Host "=== Error Running Test ===" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
finally
{
    # Cleanup
    if (Test-Path "test-output.txt")
    {
        Remove-Item "test-output.txt" -ErrorAction SilentlyContinue
    }
    if (Test-Path "test-error.txt")
    {
        Remove-Item "test-error.txt" -ErrorAction SilentlyContinue
    }
}

