@echo off
echo ========================================
echo Iniciando Servicio ControlplastPLC
echo ========================================
echo.

REM Verificar si el servicio existe
sc query ControlplastPLC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: El servicio no está instalado
    echo Ejecuta install-service.bat primero
    pause
    exit /b 1
)

echo Iniciando servicio...
net start ControlplastPLC

if %errorLevel% equ 0 (
    echo.
    echo ========================================
    echo Servicio iniciado correctamente
    echo ========================================
    echo.
    echo El servicio está ejecutándose en segundo plano
    echo.
    echo Para ver el estado:
    echo    sc query ControlplastPLC
    echo.
    echo Para ver los logs:
    echo    type C:\Services\ControlplastPLC\logs\service-*.log
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: No se pudo iniciar el servicio
    echo ========================================
    echo.
    echo Posibles causas:
    echo    1. El servicio ya está ejecutándose
    echo    2. Error en la configuración (revisa appsettings.json)
    echo    3. No se puede conectar a la base de datos
    echo    4. Problemas de permisos
    echo.
    echo Revisa los logs en: C:\Services\ControlplastPLC\logs
    echo O usa el Visor de Eventos de Windows (eventvwr.msc)
    echo.
)

pause