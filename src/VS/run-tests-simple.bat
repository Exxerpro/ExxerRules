@echo off
echo Running tests with simple timeout wrapper...
echo.

REM Set timeout to 10 minutes (600 seconds)
set TIMEOUT=600

echo Starting test run at %TIME%
echo Timeout set to %TIMEOUT% seconds

REM Run the test command with timeout
timeout /t %TIMEOUT% /nobreak > nul 2>&1 & dotnet test --filter "ResultComprehensiveTests" --verbosity minimal

set EXIT_CODE=%ERRORLEVEL%

echo.
echo Test run completed at %TIME%
echo Exit code: %EXIT_CODE%

if %EXIT_CODE% NEQ 0 (
    echo Test run failed!
    exit /b 1
) else (
    echo Test run completed successfully!
    exit /b 0
)
