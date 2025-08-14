@echo off
echo Running Stryker mutation testing with timeout wrapper...
echo.

REM Set timeout to 15 minutes (900 seconds) for Stryker
set TIMEOUT=900

REM Run Stryker with timeout, excluding Dashboard reporter
powershell -ExecutionPolicy Bypass -File "run-with-timeout.ps1" -Command "dotnet stryker --reporters Progress,Html,Json --project ExxerRules.Analyzers --test-project ExxerRules.Tests" -TimeoutSeconds %TIMEOUT%

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Stryker run failed or timed out!
    exit /b 1
) else (
    echo.
    echo Stryker run completed successfully!
    exit /b 0
)
