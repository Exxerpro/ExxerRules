@echo off
echo Running Stryker mutation testing...
echo.

REM Set timeout to 15 minutes (900 seconds)
set TIMEOUT=900

echo Starting Stryker at %TIME%
echo Timeout set to %TIMEOUT% seconds

REM Run Stryker with basic timeout
timeout /t %TIMEOUT% /nobreak > nul 2>&1 & dotnet stryker --reporters Progress,Html,Json --project ExxerRules.Analyzers --test-project ExxerRules.Tests

set EXIT_CODE=%ERRORLEVEL%

echo.
echo Stryker completed at %TIME%
echo Exit code: %EXIT_CODE%

if %EXIT_CODE% NEQ 0 (
    echo Stryker run failed or timed out!
    exit /b 1
) else (
    echo Stryker run completed successfully!
    exit /b 0
)
