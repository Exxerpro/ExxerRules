@echo off
echo ========================================
echo IndTrace Unit Test Generator
echo ========================================
echo.
echo This script will help identify classes that need unit tests
echo and provide a systematic approach to generate them.
echo.

REM Set the base directory
set BASE_DIR=%~dp0
set APPLICATION_DIR=%BASE_DIR%..\..\Core\Application
set DOMAIN_DIR=%BASE_DIR%..\..\Core\Domain

echo Scanning for public classes in Application layer...
echo.

REM Find all public classes in Application layer
for /r "%APPLICATION_DIR%" %%f in (*.cs) do (
    findstr /C:"public class" "%%f" >nul 2>&1
    if not errorlevel 1 (
        echo Found: %%~nxf
        findstr /C:"public class" "%%f"
        echo.
    )
)

echo.
echo Scanning for public classes in Domain layer...
echo.

REM Find all public classes in Domain layer
for /r "%DOMAIN_DIR%" %%f in (*.cs) do (
    findstr /C:"public class" "%%f" >nul 2>&1
    if not errorlevel 1 (
        echo Found: %%~nxf
        findstr /C:"public class" "%%f"
        echo.
    )
)

echo.
echo ========================================
echo Test Generation Checklist
echo ========================================
echo.
echo Application Layer Classes to Test:
echo 1. Command Handlers (Create, Update, Delete)
echo 2. Query Handlers
echo 3. Validators
echo 4. Services
echo 5. Specifications
echo.
echo Domain Layer Classes to Test:
echo 1. Entities
echo 2. Value Objects
echo 3. Domain Services
echo 4. Specifications
echo.
echo Test Style Guidelines:
echo - Use xUnit + NSubstitute + Shouldly
echo - Follow Arrange-Act-Assert pattern
echo - Test constructors, properties, and methods
echo - Include edge cases and validation
echo - Use descriptive test names
echo.
echo Run this script periodically to track progress.
echo.
pause
