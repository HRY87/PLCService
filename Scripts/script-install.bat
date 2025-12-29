@echo off
echo ========================================
echo Instalador del Servicio ControlplastPLC
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

echo [1/5] Verificando prerequisitos...
where dotnet >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: .NET 6.0 SDK no encontrado
    echo Por favor instala .NET 6.0 SDK desde: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo    .NET SDK encontrado

echo.
echo [2/5] Compilando el servicio...
cd ..
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if %errorLevel% neq 0 (
    echo ERROR: Falló la compilación
    pause
    exit /b 1
)

echo.
echo [3/5] Creando directorio de instalación...
set INSTALL_DIR=C:\Services\ControlplastPLC
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
if not exist "%INSTALL_DIR%\logs" mkdir "%INSTALL_DIR%\logs"

echo.
echo [4/5] Copiando archivos...
xcopy /Y /E bin\Release\net6.0\win-x64\publish\* "%INSTALL_DIR%\"

if not exist "%INSTALL_DIR%\ControlplastPLCService.exe" (
    echo ERROR: No se encontró el ejecutable compilado
    pause
    exit /b 1
)

echo.
echo [5/5] Registrando servicio de Windows...
sc query ControlplastPLC >nul 2>&1
if %errorLevel% equ 0 (
    echo El servicio ya existe, eliminando versión anterior...
    sc stop ControlplastPLC
    timeout /t 2 >nul
    sc delete ControlplastPLC
    timeout /t 2 >nul
)

sc create ControlplastPLC binPath= "%INSTALL_DIR%\ControlplastPLCService.exe" start= auto DisplayName= "Controlplast PLC Monitor"
sc description ControlplastPLC "Servicio de monitoreo de PLCs Controlplast con almacenamiento en base de datos"
sc failure ControlplastPLC reset= 86400 actions= restart/5000/restart/10000/restart/30000

echo.
echo ========================================
echo Instalación completada exitosamente!
echo ========================================
echo.
echo El servicio ha sido instalado en: %INSTALL_DIR%
echo.
echo IMPORTANTE: Antes de iniciar el servicio:
echo    1. Edita %INSTALL_DIR%\appsettings.json
echo    2. Configura las IPs de los PLCs
echo    3. Configura las credenciales de la base de datos
echo    4. Ejecuta el script Schema.sql en SQL Server
echo.
echo Para iniciar el servicio:
echo    sc start ControlplastPLC
echo    o ejecuta: Scripts\start-service.bat
echo.
echo Para ver el estado:
echo    sc query ControlplastPLC
echo    o ejecuta: Scripts\status-service.bat
echo.
echo Los logs se encuentran en: %INSTALL_DIR%\logs
echo.
pause