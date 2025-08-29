@echo off
echo Running tests with timeout wrapper...
echo.

REM Set timeout to 10 minutes (600 seconds)
set TIMEOUT=600

REM Run the test command with timeout
powershell -ExecutionPolicy Bypass -File "run-with-timeout.ps1" -Command "dotnet test --filter \"ResultComprehensiveTests\" --verbosity minimal" -TimeoutSeconds %TIMEOUT%

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Test run failed or timed out!
    exit /b 1
) else (
    echo.
    echo Test run completed successfully!
    exit /b 0
)
