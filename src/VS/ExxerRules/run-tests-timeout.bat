@echo off
echo Running tests with timeout protection...
echo.

REM Set timeout to 10 minutes (600 seconds)
set TIMEOUT=600

echo Starting test run at %TIME%
echo Timeout set to %TIMEOUT% seconds

REM Create a temporary batch file to run the command
echo @echo off > temp_test.bat
echo dotnet test --filter "ResultComprehensiveTests" --verbosity minimal >> temp_test.bat

REM Run the temporary batch file with timeout
start /wait cmd /c "timeout /t %TIMEOUT% /nobreak > nul 2>&1 & temp_test.bat"

set EXIT_CODE=%ERRORLEVEL%

REM Clean up
del temp_test.bat

echo.
echo Test run completed at %TIME%
echo Exit code: %EXIT_CODE%

if %EXIT_CODE% NEQ 0 (
    echo Test run failed or timed out!
    exit /b 1
) else (
    echo Test run completed successfully!
    exit /b 0
)
