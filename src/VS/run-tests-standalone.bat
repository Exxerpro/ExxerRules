@echo off
echo Running tests with standalone timeout runner...
echo.

REM Set timeout to 10 minutes (600 seconds)
set TIMEOUT=600

REM Run the test command with timeout using the standalone runner
cd TestRunnerStandalone
dotnet run -- "dotnet test --filter \"ResultComprehensiveTests\" --verbosity minimal" %TIMEOUT%

set EXIT_CODE=%ERRORLEVEL%

cd ..

echo.
if %EXIT_CODE% NEQ 0 (
    echo Test run failed or timed out!
    exit /b 1
) else (
    echo Test run completed successfully!
    exit /b 0
)
