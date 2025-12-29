@echo off
echo ========================================
echo Desinstalador del Servicio ControlplastPLC
echo ========================================
echo.

REM Verificar permisos de administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Este script requiere permisos de Administrador
    echo Por favor, ejecuta como Administrador
    pause
    exit /b 1
)

echo [1/3] Verificando si el servicio existe...
sc query ControlplastPLC >nul 2>&1
if %errorLevel% neq 0 (
    echo El servicio no está instalado
    goto :REMOVE_FILES
)

echo [2/3] Deteniendo el servicio...
sc query ControlplastPLC | find "RUNNING" >nul
if %errorLevel% equ 0 (
    sc stop ControlplastPLC
    echo Esperando a que el servicio se detenga...
    timeout /t 5 >nul
) else (
    echo El servicio ya está detenido
)

echo.
echo [3/3] Eliminando el servicio...
sc delete ControlplastPLC

if %errorLevel% equ 0 (
    echo Servicio eliminado correctamente
) else (
    echo ERROR: No se pudo eliminar el servicio
    pause
    exit /b 1
)

:REMOVE_FILES
echo.
echo ========================================
set INSTALL_DIR=C:\Services\ControlplastPLC

if not exist "%INSTALL_DIR%" (
    echo El directorio de instalación no existe
    goto :END
)

echo ¿Deseas eliminar los archivos del servicio?
echo Ruta: %INSTALL_DIR%
echo.
echo [S] Si - Eliminar todos los archivos (incluyendo logs)
echo [N] No - Conservar archivos
echo.
set /p REMOVE_FILES="Tu elección (S/N): "

if /i "%REMOVE_FILES%"=="S" (
    echo.
    echo Eliminando archivos...
    rmdir /S /Q "%INSTALL_DIR%"
    if %errorLevel% equ 0 (
        echo Archivos eliminados correctamente
    ) else (
        echo ADVERTENCIA: No se pudieron eliminar algunos archivos
        echo Puedes eliminarlos manualmente: %INSTALL_DIR%
    )
) else (
    echo.
    echo Archivos conservados en: %INSTALL_DIR%
    echo Puedes eliminarlos manualmente cuando lo desees
)

:END
echo.
echo ========================================
echo Desinstalación completada!
echo ========================================
echo.
pause