@echo off
echo Running tests with improved timeout wrapper...
echo.

REM Set timeout to 10 minutes (600 seconds)
set TIMEOUT=600

REM Run the test command with timeout
powershell -ExecutionPolicy Bypass -File "run-with-timeout-v2.ps1" -Command "dotnet test --filter \"ResultComprehensiveTests\" --verbosity minimal" -TimeoutSeconds %TIMEOUT%

set EXIT_CODE=%ERRORLEVEL%

echo.
if %EXIT_CODE% NEQ 0 (
    echo Test run failed or timed out!
    exit /b 1
) else (
    echo Test run completed successfully!
    exit /b 0
)
