@echo off
echo Running Stryker with standalone timeout runner...
echo.

REM Set timeout to 15 minutes (900 seconds)
set TIMEOUT=900

REM Run Stryker with timeout using the standalone runner
cd TestRunnerStandalone
dotnet run --project TestRunnerStandalone.csproj -- "dotnet stryker --reporters Progress,Html,Json --project ExxerRules.Analyzers --test-project ExxerRules.Tests" %TIMEOUT%

set EXIT_CODE=%ERRORLEVEL%

cd ..

echo.
if %EXIT_CODE% NEQ 0 (
    echo Stryker run failed or timed out!
    exit /b 1
) else (
    echo Stryker run completed successfully!
    exit /b 0
)
