@echo off
setlocal EnableExtensions
title PDF Tool Portable - Startup diagnostics
cd /d "%~dp0"
set "APP=%~dp0PDF-Tool.exe"
set "REPORT=%TEMP%\PDF-Tool-startup-check.txt"
echo PDF Tool startup check
echo ======================
echo This diagnostic runs locally. It does not upload any PDF.
echo No administrator privileges or Windows settings changes are needed.
echo.
if not exist "%APP%" (
 echo ERROR: PDF-Tool.exe is not in the same folder as this script.
 echo Extract the entire portable ZIP before running it.
 goto END
)
echo Testing GUI startup without opening any PDF...
if exist "%REPORT%" del /q "%REPORT%" >nul 2>nul
"%APP%" --self-test "%REPORT%"
set "RESULT=%ERRORLEVEL%"
echo.
if exist "%REPORT%" (
 type "%REPORT%"
) else (
 echo No diagnostic report produced.
 echo This may mean Windows or endpoint protection stopped the EXE
 echo before its .NET entry point could start.
)
echo.
echo Process exit code: %RESULT%
echo.
if not "%RESULT%"=="0" (
 echo If this is a company-managed PC, ask IT to review the event.
 echo Do NOT disable antivirus, SmartScreen, or company policies.
) else (
 echo Startup check passed. Try opening PDF-Tool.exe normally.
)
:END
echo.
pause
endlocal
