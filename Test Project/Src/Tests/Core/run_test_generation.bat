@echo off
setlocal enabledelayedexpansion

:menu
cls
echo ========================================
echo IndTrace Unit Test Generation Tool
echo ========================================
echo.
echo 1. Scan for classes that need tests
echo 2. Generate all test files
echo 3. Generate test for specific class
echo 4. Run existing tests
echo 5. Show test coverage report
echo 6. Exit
echo.
set /p choice="Select an option (1-6): "

if "%choice%"=="1" goto scan
if "%choice%"=="2" goto generate_all
if "%choice%"=="3" goto generate_specific
if "%choice%"=="4" goto run_tests
if "%choice%"=="5" goto coverage
if "%choice%"=="6" goto exit
goto menu

:scan
echo.
echo Scanning for classes that need unit tests...
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0generate_unit_tests.ps1"
echo.
pause
goto menu

:generate_all
echo.
echo Generating all unit test files...
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0generate_unit_tests.ps1" -GenerateAll
echo.
pause
goto menu

:generate_specific
echo.
set /p class_name="Enter class name: "
if "%class_name%"=="" goto menu
echo.
echo Generating test for class: %class_name%
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0generate_unit_tests.ps1" -SpecificClass "%class_name%"
echo.
pause
goto menu

:run_tests
echo.
echo Running existing unit tests...
echo.
cd /d "%~dp0"
dotnet test --verbosity normal
echo.
pause
goto menu

:coverage
echo.
echo Generating test coverage report...
echo.
cd /d "%~dp0"
dotnet test --collect:"XPlat Code Coverage" --results-directory coverage
echo.
echo Coverage report generated in coverage directory
echo.
pause
goto menu

:exit
echo.
echo Thank you for using IndTrace Unit Test Generation Tool!
echo.
exit /b 0
