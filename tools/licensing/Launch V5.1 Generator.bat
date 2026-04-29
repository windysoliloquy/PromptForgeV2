@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "GENERATOR=%SCRIPT_DIR%Generate-UnlockFile-V5.1.ps1"

if not exist "%GENERATOR%" (
    echo Could not find:
    echo %GENERATOR%
    echo.
    pause
    exit /b 1
)

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%GENERATOR%"
set "EXIT_CODE=%ERRORLEVEL%"

if not "%EXIT_CODE%"=="0" (
    echo.
    echo Prompt Forge V5.1 generator exited with code %EXIT_CODE%.
    echo.
    pause
)

exit /b %EXIT_CODE%
