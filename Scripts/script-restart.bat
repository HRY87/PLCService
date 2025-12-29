@echo off
echo ========================================
echo Reiniciando Servicio ControlplastPLC
echo ========================================
echo.

REM Verificar si el servicio existe
sc query ControlplastPLC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: El servicio no está instalado
    pause
    exit /b 1
)

echo [1/2] Deteniendo servicio...
sc query ControlplastPLC | find "RUNNING" >nul
if %errorLevel% equ 0 (
    net stop ControlplastPLC
    if %errorLevel% neq 0 (
        echo ERROR: No se pudo detener el servicio
        pause
        exit /b 1
    )
    echo Servicio detenido
) else (
    echo El servicio ya estaba detenido
)

echo Esperando 3 segundos...
timeout /t 3 >nul

echo.
echo [2/2] Iniciando servicio...
net start ControlplastPLC

if %errorLevel% equ 0 (
    echo.
    echo ========================================
    echo Servicio reiniciado correctamente
    echo ========================================
    echo.
    echo El servicio está ejecutándose nuevamente
    echo.
    echo Para verificar el estado:
    echo    sc query ControlplastPLC
    echo.
    echo Para ver los logs más recientes:
    echo    powershell -Command "Get-Content C:\Services\ControlplastPLC\logs\service-$(Get-Date -Format 'yyyyMMdd').log -Tail 50"
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: No se pudo iniciar el servicio
    echo ========================================
    echo.
    echo Revisa los logs en: C:\Services\ControlplastPLC\logs
    echo.
)

pause