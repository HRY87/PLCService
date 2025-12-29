# 🪟 Servicio de Windows - ControlplastPLC v3.0

Sistema completo de monitoreo multi-máquina para PLCs Controlplast como Servicio de Windows.

---

## 📁 Estructura Completa del Proyecto

```
ControlplastPLCService/
├── Models/
│   ├── model-config-maquina.cs         # Config por máquina y datos
│   ├── model-config-sistema.cs         # Config general del sistema
│   |── model-datos-produccion.cs       # Modelo de datos de producción
│   ├── model-eventargs.cs              # Eventos del sistema
│   ├── model-maquina.cs                # Definición de máquina
├── Scripts/
│   ├── script-install.bat              # Instalación automática
│   ├── script-uninstall.bat            # Desinstalación
│   ├── script-start.bat                # Iniciar servicio
│   ├── script-stop.bat                 # Detener servicio
│   ├── script-restart.bat              # Reiniciar servicio
│   └── script-status.bat               # Ver estado y logs
├── Services/
│   ├── service-encryption.cs           # Encriptación AES-256
│   ├── service-database.cs             # Acceso a BD SQL Server
│   └── service-manager.cs              # Gestor de múltiples máquinas
├── controlplast-plc.cs                 # Cliente PLC(protocolo TCP/IP)
├── program-service.cs                  # Configuración del servicio
├── plc-worker.cs                       # Worker principal del servicio
├── ControlplastPLCService.csproj       # Archivo de proyecto
├── appsetting.json                     # Configuración
└── readme.md                           # Este archivo
```

---

## 🚀 Instalación Rápida

### Prerequisitos

1. **.NET 6.0 SDK** instalado
   - Descargar: https://dotnet.microsoft.com/download/dotnet/6.0
   
2. **SQL Server** instalado (local o remoto)
   - Express: https://www.microsoft.com/sql-server/sql-server-downloads
   
3. **Permisos de Administrador** en Windows

### Pasos de Instalación

1. **Clonar o descargar el proyecto**
   ```bash
   # En VS Code, abrir la carpeta del proyecto
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Crear la base de datos**
   ```bash
   # Ejecutar Schema.sql en SQL Server
   sqlcmd -S localhost -U sa -P TuPassword -i Schema.sql
   ```

4. **Configurar appsettings.json**
   - Editar IPs de los PLCs
   - Configurar credenciales de base de datos
   - Habilitar/deshabilitar máquinas

5. **Instalar el servicio**
   ```bash
   # Ejecutar como Administrador
   Scripts\install-service.bat
   ```

6. **Iniciar el servicio**
   ```bash
   Scripts\start-service.bat
   ```

---

## ⚙️ Configuración Detallada

### appsettings.json

#### Sección: Máquinas

```json
{
  "Maquinas": [
    {
      "Id": 1,                              // ID único
      "Nombre": "Extrusora 1",              // Nombre descriptivo
      "Descripcion": "Línea principal",     // Descripción
      "Habilitada": true,                   // true = activa
      "Configuracion": {
        "Ip": "192.168.200.31",             // IP del PLC
        "Puerto": 8000,                     // Puerto TCP
        "Timeout": 3000,                    // Timeout (ms)
        "IntervaloLectura": 5,              // Segundos entre lecturas
        "IntervaloReconexion": 10,          // Segundos entre reintentos
        "MaxIntentosReconexion": 5,         // Máximo reintentos
        "datosProduccionConfig": {
          // Producción Programada (mayormente false por defecto)
          "GuardarKghProgramado": false,
          "GuardarEspessuraProgramada": false,
          "GuardarLarguraBrutaProgramada": false,
          "GuardarLarguraLiquidaProgramada": false,
          "GuardarGramaturaProgramada": false,
          "GuardarVelocidadeProgramada": false,
          
          // Producción Actual (algunos con true)
          "GuardarKghAtual": true,          // kg/h actual
          "GuardarEspessuraAtual": false,
          "GuardarLarguraBrutaAtual": false,
          "GuardarVelocidadeAtual": true,   // m/min actual
          
          // Roscas A..E (mayoría false)
          "GuardarRoscaAGramaMetro": false,
          "GuardarRoscaASilos": false,
          "GuardarRoscaATotalizadores": false,
          "GuardarRoscaADensidades": false,
          // ... (similar para Roscas B, C, D, E)
          
          // Consumo/Energía
          "GuardarConsumoWatt": true,       // consumo en kW
          "GuardarConsumoAmpere": false,
          
          // OP / Producción
          "GuardarOpNumero": true,          // número de OP
          "GuardarOpStatus": true,          // estado de OP
          "GuardarKgProduzidos": true,      // kg producidos
          "GuardarMetrosProduzidos": true,  // metros producidos
          "GuardarConsumoTotalOp": true     // consumo total de OP
        }
      }
    }
  ]
}
```

#### Sección: Base de Datos

```json
{
  "DatabaseLocal": {
    "Tipo": "SqlServer",
    "Host": "localhost",                    // Servidor SQL
    "Puerto": 1433,
    "Database": "ControlplastPLC",
    "Usuario": "sa",
    "Password": "YourPassword",             // Se encripta automáticamente
    "UsarEncriptacion": true,
    "TimeoutSegundos": 30,
    "GuardarHistorico": true                // true = guardar histórico
  },
  
  "DatabaseNube": {
    // Similar a DatabaseLocal
    // Opcional: dejar vacío si no se usa nube
    "Host": "",
    "GuardarHistorico": false               // Solo datos actuales
  }
}
```

#### Sección: General

```json
{
  "General": {
    "RutaLogs": "logs",                     // Carpeta de logs
    "RetencionLogsDias": 30,                // Días de retención
    "LogVerbose": false                     // true = logs detallados
  }
}
```

---

## 🎮 Gestión del Servicio

### Scripts Disponibles

| Script | Descripción |
|--------|-------------|
| `install-service.bat` | Compila, instala y configura el servicio |
| `start-service.bat` | Inicia el servicio |
| `stop-service.bat` | Detiene el servicio |
| `restart-service.bat` | Reinicia el servicio |
| `status-service.bat` | Muestra estado y logs |
| `uninstall-service.bat` | Desinstala el servicio |

### Comandos Manuales

```batch
# Iniciar
net start ControlplastPLC
sc start ControlplastPLC

# Detener
net stop ControlplastPLC
sc stop ControlplastPLC

# Estado
sc query ControlplastPLC

# Configuración
sc qc ControlplastPLC
```

### Interfaz Gráfica

1. Presiona `Win + R`
2. Escribe `services.msc`
3. Busca "Controlplast PLC Monitor"
4. Click derecho → Propiedades/Iniciar/Detener

---

## 📊 Logs del Servicio

### Ubicación

```
C:\Services\ControlplastPLC\logs\
├── service-20241217.log
├── service-20241218.log
└── ...
```

### Ver logs en tiempo real

**PowerShell:**
```powershell
Get-Content C:\Services\ControlplastPLC\logs\service-$(Get-Date -Format "yyyyMMdd").log -Wait -Tail 50
```

**CMD:**
```batch
Scripts\status-service.bat
# Luego elegir "S" para ver el log
```

### Formato de logs

```
2024-12-17 14:30:15 [INF] Servicio de monitoreo PLC iniciado
2024-12-17 14:30:16 [INF] Configuración: 2 máquinas
2024-12-17 14:30:17 [INF] ✅ Conectado a base de datos: localhost/ControlplastPLC
2024-12-17 14:30:18 [INF] ✅ [Extrusora 1] Estado: Desconectada → Conectada
2024-12-17 14:30:19 [INF] Sistema de monitoreo iniciado correctamente
2024-12-17 14:35:15 [INF] === Estado del Sistema ===
2024-12-17 14:35:15 [INF] Máquinas Conectadas: 2/2
```

---

## � Modelo de Datos de Producción (DatosProduccion)

### Estructura y Nombres en Español

El modelo `DatosProduccion.cs` contiene todos los datos de producción leídos del PLC, organizados en secciones:

#### Producción – Programado
- `KgHoraProgramado` - kg/h programado
- `EspesorProgramado` - espesor en mm
- `AnchoBrutoProgramado` - ancho bruto programado
- `AnchoNetoProgramado` - ancho neto programado
- `GramajeProgramado` - gramaje programado
- `MetrosPorMinProgramado` - metros/minuto programado

#### Producción – Actual
- `KgHoraActual` - kg/h actual
- `EspesorActual` - espesor actual
- `AnchoBrutoActual` - ancho bruto actual
- `AnchoNetoActual` - ancho neto actual
- `GramajeActual` - gramaje actual
- `MetrosPorMinActual` - metros/minuto actual

#### Roscas (A, B, C, D, E)
Cada rosca tiene sus propias propiedades:
- `Rosca[X]_GramaMetroActual` / `Rosca[X]_GramaMetroProgramado`
- `Rosca[X]_EspesorActual` / `Rosca[X]_EspesorProgramado`
- `Rosca[X]_PorcentajeActual` / `Rosca[X]_PorcentajeProgramado`
- `Rosca[X]_KgHoraActual` / `Rosca[X]_KgHoraProgramado`
- `Rosca[X]_Silo1Actual` a `Rosca[X]_Silo6Actual` (y programado)
- `Rosca[X]_DensidadSilo1` a `Rosca[X]_DensidadSilo6`
- `Rosca[X]_TotalSilo1` a `Rosca[X]_TotalSilo6` (kg acumulados)

#### Energía/Consumo
- `TensionL1`, `TensionL2`, `TensionL3` - voltaje por fase
- `AmperesL1`, `AmperesL2`, `AmperesL3` - amperaje por fase
- `ConsumoActualKW` - consumo en kW
- `KWTotal` - total histórico de kW
- `KWPorKg` - eficiencia kW por kg
- `KWDia` - consumo del día

#### Operación/OP
- `NumeroOP` - número de orden de producción (string)
- `EstadoOP` - estado (int): 2=Produciendo, 3=Finalizada
- `KgPorMetroOP` - kg por metro de la OP
- `TamanoBobinaOP` - tamaño de bobina
- `RecortesOP` - recortes (kg)
- `KgProducidos` - kg producidos en la OP
- `MetrosProducidos` - metros producidos
- `ConsumoTotalOP` - consumo total de la OP

#### Datos de Pedido/Máquina
- `MaquinaOcupada` - máquina ocupada (string)
- `NombreMaquina` - nombre de máquina
- `Pedido` - número de pedido
- `PedidoIniciado` - fecha de inicio
- `PorcentajeB` - porcentaje rosca B
- `PorcentajeC` - porcentaje rosca C
- `PrevisionTerminar` - previsión de término
- `NombreReceta` - nombre de la receta
- `EstadoPedido` - estado del pedido
- `TamanoBobina` - tamaño de bobina
- `TiempoTotal` - tiempo total de OP

### Direcciones PLC en controlplast-plc.cs

Las constantes de direcciones están organizadas por secciones:

```csharp
// Energía/Consumo
public const int ADDR_TENSION_L1 = 126;
public const int ADDR_AMPERES_L1 = 130;
public const int ADDR_CONSUMO_ACTUAL_KW = 132;

// Programado
public const int ADDR_KG_HORA_PROGRAMADO = 502;
public const int ADDR_ESPESOR_PROGRAMADO = 506;
public const int ADDR_ANCHO_BRUTO_PROGRAMADO = 508;

// Actual
public const int ADDR_KG_HORA_ACTUAL = 510;
public const int ADDR_ESPESOR_ACTUAL = 514;
public const int ADDR_ANCHO_BRUTO_ACTUAL = 516;

// Roscas A..E (ADDR_ROSCA_A_*, ADDR_ROSCA_B_*, etc.)
// Cada rosca: Grama/Metro, Espesor, Porcentaje, KgHora, Silos, Densidades, Totalizadores

// OP / Producción
public const int ADDR_NUMERO_OP = 30000;      // String (16)
public const int ADDR_ESTADO_OP = 30023;      // Word (entero corto)
public const int ADDR_KG_POR_METRO_OP = 30017;
public const int ADDR_TAMANO_BOBINA_OP = 30019;
```

### Mapeo a Base de Datos

Los datos se guardan en dos tablas:

1. **DatosProduccionHistorico** - histórico completo
   - INSERT con todas las propiedades
   - Se inserta en cada lectura
   - Columnas respetan nombres: `KgHoraProgramado`, `EspesorProgramado`, etc.

2. **DatosProduccionActual** - datos actuales
   - MERGE (UPDATE o INSERT)
   - Una fila por máquina
   - Mismos nombres de columnas

### Filtrado por Configuración

Mediante `DatosProduccionConfig.datosProduccionConfig`:

- Si `GuardarKghAtual = true` → se inserta el valor en BD
- Si `GuardarKghAtual = false` → se inserta `DBNull` en BD

Esto permite personalizar qué datos guardar por máquina.

---

### Configuración de VS Code

**`.vscode/launch.json`:**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (console)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net6.0/ControlplastPLCService.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "console": "internalConsole",
      "stopAtEntry": false
    }
  ]
}
```

**`.vscode/tasks.json`:**
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/ControlplastPLCService.csproj"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

### Ejecutar en modo consola (debugging)

```bash
# Ejecutar directamente
dotnet run

# Con logs detallados
dotnet run --LogVerbose=true

# Esto ejecuta el servicio como consola para debugging
# Útil durante desarrollo
```

### Compilar

```bash
# Debug
dotnet build

# Release
dotnet build -c Release

# Publicar
dotnet publish -c Release -r win-x64 --self-contained true
```

---

## 🔄 Actualización del Servicio

### Proceso Recomendado

1. **Detener el servicio**
   ```batch
   Scripts\stop-service.bat
   ```

2. **Hacer backup** (opcional pero recomendado)
   ```batch
   xcopy /Y /E C:\Services\ControlplastPLC C:\Backup\ControlplastPLC_%date%\
   ```

3. **Compilar nueva versión**
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
   ```

4. **Copiar archivos** (conservar appsettings.json)
   ```batch
   xcopy /Y bin\Release\net6.0\win-x64\publish\*.exe C:\Services\ControlplastPLC\
   xcopy /Y bin\Release\net6.0\win-x64\publish\*.dll C:\Services\ControlplastPLC\
   ```

5. **Iniciar el servicio**
   ```batch
   Scripts\start-service.bat
   ```

6. **Verificar logs**
   ```batch
   Scripts\status-service.bat
   ```

### Script de Actualización

Crea `Scripts/update-service.bat`:

```batch
@echo off
echo Actualizando servicio...
cd ..
Scripts\stop-service.bat
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
xcopy /Y bin\Release\net6.0\win-x64\publish\*.exe C:\Services\ControlplastPLC\
xcopy /Y bin\Release\net6.0\win-x64\publish\*.dll C:\Services\ControlplastPLC\
Scripts\start-service.bat
pause
```

---

## 🛠️ Solución de Problemas

### El servicio no inicia

**Error:**
```
Error 1053: El servicio no respondió a tiempo
```

**Solución:**

1. **Verificar logs**
   ```batch
   type C:\Services\ControlplastPLC\logs\service-*.log
   ```

2. **Ejecutar como consola** para ver errores
   ```batch
   cd C:\Services\ControlplastPLC
   ControlplastPLCService.exe
   ```

3. **Verificar appsettings.json**
   - IPs correctas
   - Credenciales de BD correctas
   - Formato JSON válido

4. **Verificar permisos**
   - El servicio corre como SYSTEM
   - Debe tener permisos de red y BD

### No se conecta a PLCs

1. **Verificar red**
   ```batch
   ping 192.168.200.31
   telnet 192.168.200.31 8000
   ```

2. **Revisar firewall**
   ```batch
   # Permitir conexiones salientes
   netsh advfirewall firewall add rule name="PLC Monitor" dir=out action=allow program="C:\Services\ControlplastPLC\ControlplastPLCService.exe"
   ```

3. **Verificar configuración de máquina en appsettings.json**

### No se conecta a Base de Datos

1. **Verificar SQL Server está corriendo**
   ```batch
   sc query MSSQLSERVER
   ```

2. **Probar conexión**
   ```batch
   sqlcmd -S localhost -U sa -P TuPassword -Q "SELECT @@VERSION"
   ```

3. **Verificar credenciales en appsettings.json**

4. **Revisar logs de SQL Server**

### El servicio se detiene solo

1. **Ver Event Viewer**
   ```batch
   eventvwr.msc
   # Ir a: Registros de Windows → Aplicación
   # Buscar fuente: ControlplastPLC
   ```

2. **Configurar reinicio automático**
   ```batch
   sc failure ControlplastPLC reset= 86400 actions= restart/5000/restart/10000/restart/30000
   ```

---

## 📈 Monitoreo y Mantenimiento

### Estado del Servicio

```batch
# Ver estado cada 5 minutos (está en el código)
# Los logs automáticos incluyen:
# - Máquinas conectadas
# - Estado de bases de datos
# - Errores y reconexiones
```

### Mantenimiento de Base de Datos

```sql
-- Ejecutar semanalmente
EXEC sp_LimpiarDatosAntiguos @DiasRetencion = 90;

-- Reindexar mensualmente
ALTER INDEX ALL ON DatosProduccionHistorico REBUILD;
```

### Performance

**Ver uso de recursos:**
```powershell
# CPU
Get-Counter '\Process(ControlplastPLCService)\% Processor Time'

# Memoria
Get-Counter '\Process(ControlplastPLCService)\Working Set - Private'
```

---

## 🔐 Seguridad

### Cambiar Master Key de Encriptación

En `Program.cs`, línea ~50:
```csharp
services.AddSingleton<EncryptionService>(sp => 
    new EncryptionService("TU_CLAVE_SECRETA_AQUI"));
```

### Ejecutar con usuario específico

```batch
# Crear usuario
net user PLCServiceUser ComplexPass123! /add

# Configurar servicio
sc config ControlplastPLC obj= ".\PLCServiceUser" password= "ComplexPass123!"

# Dar permisos
icacls C:\Services\ControlplastPLC /grant PLCServiceUser:(OI)(CI)F
```

---

## 📋 Checklist de Despliegue

- [ ] .NET 6.0 SDK instalado
- [ ] SQL Server instalado y configurado
- [ ] Base de datos creada con Schema.sql
- [ ] Usuario SQL con permisos adecuados
- [ ] appsettings.json configurado
  - [ ] IPs de PLCs correctas
  - [ ] Credenciales de BD correctas
  - [ ] Máquinas habilitadas
  - [ ] Sensores configurados
- [ ] Servicio instalado: `install-service.bat`
- [ ] Servicio iniciado: `start-service.bat`
- [ ] Logs generándose: revisar carpeta logs/
- [ ] Conexión a PLCs verificada
- [ ] Datos guardándose en BD
- [ ] Reinicio automático configurado
- [ ] Monitoreo periódico configurado

---

## 📞 Soporte

### Archivos de diagnóstico

Cuando solicites soporte, incluye:

1. **Logs del servicio**
   ```
   C:\Services\ControlplastPLC\logs\service-*.log
   ```

2. **Event Viewer**
   ```
   eventvwr.msc → Aplicación → filtrar por ControlplastPLC
   ```

3. **Estado del servicio**
   ```batch
   sc query ControlplastPLC
   sc qc ControlplastPLC
   ```

4. **Configuración** (sin contraseñas)
   ```
   C:\Services\ControlplastPLC\appsettings.json
   ```

---

## 📄 Licencia

Uso interno - Controlplast  
Todos los derechos reservados

---

**Versión**: 3.0  
**Fecha**: Diciembre 2024  
**Plataforma**: Windows Server 2012 R2 o superior  
**Framework**: .NET 6.0

---

¡Sistema listo para producción! 🚀