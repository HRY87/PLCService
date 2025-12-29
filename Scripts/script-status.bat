@echo off
setlocal enabledelayedexpansion

echo ========================================
echo Estado del Servicio ControlplastPLC
echo ========================================
echo.

REM Verificar si el servicio existe
sc query ControlplastPLC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: El servicio no está instalado
    echo.
    echo Para instalarlo, ejecuta: install-service.bat
    pause
    exit /b 1
)

echo === ESTADO DEL SERVICIO ===
echo.
sc query ControlplastPLC
echo.

echo === CONFIGURACIÓN DEL SERVICIO ===
echo.
sc qc ControlplastPLC
echo.

echo ========================================
echo.

set LOG_DIR=C:\Services\ControlplastPLC\logs

if not exist "%LOG_DIR%" (
    echo ADVERTENCIA: Directorio de logs no existe: %LOG_DIR%
    goto :END
)

echo === ARCHIVOS DE LOG DISPONIBLES ===
echo.
dir /B /O-D "%LOG_DIR%\service-*.log" 2>nul

if %errorLevel% neq 0 (
    echo No se encontraron archivos de log
    goto :END
)

echo.
echo ========================================
echo.
set /p SHOW_LOG="¿Deseas ver el log más reciente? (S/N): "

if /i "%SHOW_LOG%"=="S" (
    echo.
    echo === ÚLTIMAS 50 LÍNEAS DEL LOG ===
    echo.
    
    REM Encontrar el archivo más reciente
    for /f "delims=" %%f in ('dir /B /O-D "%LOG_DIR%\service-*.log"') do (
        set LATEST_LOG=%%f
        goto :FOUND_LOG
    )
    
    :FOUND_LOG
    echo Archivo: !LATEST_LOG!
    echo.
    
    powershell -Command "Get-Content '%LOG_DIR%\!LATEST_LOG!' -Tail 50"
    
    echo.
    echo ========================================
    echo.
    echo Para ver el log completo:
    echo    type %LOG_DIR%\!LATEST_LOG!
    echo.
    echo Para ver en tiempo real:
    echo    powershell -Command "Get-Content '%LOG_DIR%\!LATEST_LOG!' -Wait -Tail 50"
    echo.
)

:END
pause