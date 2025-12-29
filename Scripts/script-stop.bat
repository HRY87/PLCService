@echo off
echo ========================================
echo Deteniendo Servicio ControlplastPLC
echo ========================================
echo.

REM Verificar si el servicio existe
sc query ControlplastPLC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: El servicio no está instalado
    pause
    exit /b 1
)

echo Deteniendo servicio...
net stop ControlplastPLC

if %errorLevel% equ 0 (
    echo.
    echo ========================================
    echo Servicio detenido correctamente
    echo ========================================
    echo.
    echo El servicio ha sido detenido
    echo.
    echo Para volver a iniciarlo:
    echo    net start ControlplastPLC
    echo    o ejecuta: start-service.bat
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: No se pudo detener el servicio
    echo ========================================
    echo.
    echo Posibles causas:
    echo    1. El servicio ya está detenido
    echo    2. El servicio está bloqueado por otro proceso
    echo.
    echo Intenta forzar la detención con:
    echo    sc stop ControlplastPLC
    echo.
)

pause