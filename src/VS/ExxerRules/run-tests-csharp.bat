@echo off
echo Building and running C# test runner...
echo.

REM Build the test runner
dotnet build TestRunner.csproj --verbosity minimal

if %ERRORLEVEL% NEQ 0 (
    echo Failed to build test runner!
    exit /b 1
)

REM Run the test runner
dotnet run --project TestRunner.csproj

set EXIT_CODE=%ERRORLEVEL%

echo.
if %EXIT_CODE% NEQ 0 (
    echo Test run failed or timed out!
    exit /b 1
) else (
    echo Test run completed successfully!
    exit /b 0
)
