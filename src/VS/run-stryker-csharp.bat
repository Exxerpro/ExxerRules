@echo off
echo Building and running C# Stryker runner...
echo.

REM Build the Stryker runner
dotnet build StrykerRunner.csproj --verbosity minimal

if %ERRORLEVEL% NEQ 0 (
    echo Failed to build Stryker runner!
    exit /b 1
)

REM Run the Stryker runner
dotnet run --project StrykerRunner.csproj

set EXIT_CODE=%ERRORLEVEL%

echo.
if %EXIT_CODE% NEQ 0 (
    echo Stryker run failed or timed out!
    exit /b 1
) else (
    echo Stryker run completed successfully!
    exit /b 0
)
