# 🪟 Servicio de Monitoreo PLC - Modo Pruebas

Sistema simplificado de monitoreo para PLCs Controlplast **sin base de datos**. Diseñado para pruebas unitarias y validación de lecturas.

---

## 📁 Estructura del Proyecto

```
ControlplastPLCService/
├── Models/
│   ├── model-config-maquina.cs         # Configuración de conexión PLC
│   ├── model-config-sistema.cs         # Configuración general
│   └── model-datos-produccion.cs       # Modelo de datos del PLC
├── Scripts/
│   ├── script-install.bat              # Instalación del servicio
│   ├── script-start.bat                # Iniciar servicio
│   ├── script-stop.bat                 # Detener servicio
│   └── script-status.bat               # Ver logs en tiempo real
├── controlplast-plc.cs                 # Cliente de comunicación con PLC
├── program-service.cs                  # Configuración del servicio
├── plc-worker.cs                       # Worker de monitoreo
├── ControlplastPLCService.csproj       # Proyecto
├── appsetting.json                     # Configuración
└── README.md                           # Este archivo
```

---

## 🚀 Instalación Rápida

### Prerequisitos

1. **.NET 6.0 SDK** instalado
   - Descargar: https://dotnet.microsoft.com/download/dotnet/6.0
   
2. **Permisos de Administrador** en Windows

### Pasos de Instalación

1. **Clonar o descargar el proyecto**

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar appsettings.json**
   - Editar IPs de los PLCs
   - Configurar intervalos de lectura
   - Habilitar/deshabilitar máquinas

4. **Ejecutar en modo consola** (recomendado para pruebas)
   ```bash
   dotnet run
   ```

5. **O instalar como servicio de Windows**
   ```bash
   Scripts\install-service.bat
   Scripts\start-service.bat
   ```

---

## ⚙️ Configuración

### appsettings.json

```json
{
  "ConfiguracionSistema": {
    "Maquinas": [
      {
        "Id": 1,
        "Nombre": "Extrusora 1",
        "Descripcion": "Línea principal",
        "Habilitada": true,
        "Configuracion": {
          "Ip": "192.168.200.30",           // IP del PLC
          "Puerto": 8000,                   // Puerto TCP
          "Timeout": 3000,                  // Timeout (ms)
          "IntervaloLectura": 5,            // Segundos entre lecturas
          "IntervaloReconexion": 10,        // Segundos entre reintentos
          "MaxIntentosReconexion": 5        // Máximo reintentos
        }
      }
    ],
    "General": {
      "RutaLogs": "logs",
      "RetencionLogsDias": 30,
      "LogVerbose": true,                   // true = logs detallados
      "IntervaloLogEstadoMinutos": 5        // Resumen cada N minutos
    }
  }
}
```

#### Parámetros Importantes

- **LogVerbose = true**: Muestra todos los datos de producción en cada lectura
- **LogVerbose = false**: Solo muestra resumen de datos clave (OP, kg/h, m/min)
- **IntervaloLectura**: Tiempo entre lecturas del PLC
- **IntervaloLogEstadoMinutos**: Frecuencia del resumen de estado

---

## 🎮 Ejecución

### Modo Consola (Recomendado para pruebas)

```bash
# Ejecutar directamente
dotnet run

# Compilar y ejecutar
dotnet build
dotnet run

# Ver logs en tiempo real
# (Los logs también se muestran en consola)
```

### Modo Servicio de Windows

```batch
# Instalar
Scripts\install-service.bat

# Iniciar
Scripts\start-service.bat

# Detener
Scripts\stop-service.bat

# Ver logs
Scripts\status-service.bat
```

---

## 📊 Salida en Pantalla

### Modo Verbose (LogVerbose = true)

```
╔════════════════════════════════════════════════════════════╗
║  [Extrusora 1] - LECTURA COMPLETA - 14:30:15
╠════════════════════════════════════════════════════════════╣
║ 📋 PROGRAMADO:
║   • Kg/h: 150.50 | Espesor: 0.05 | Ancho Bruto: 800.00
║   • Ancho Neto: 750.00 | Gramaje: 45.20 | m/min: 25.30
║ 📊 ACTUAL:
║   • Kg/h: 148.30 | Espesor: 0.05 | Ancho Bruto: 798.50
║   • Ancho Neto: 748.20 | Gramaje: 44.90 | m/min: 25.10
║ 🔧 ROSCA A:
║   • g/m: 450.20 | Espesor: 0.05 | %: 65.50 | Kg/h: 148.30
║   • Silos: S1=12.5 S2=8.3 S3=15.2 S4=0.0 S5=0.0 S6=0.0
║   • Totales: T1=1250.5 T2=830.2 T3=1520.8 T4=0.0 T5=0.0 T6=0.0
║   • Densidades: D1=0.92 D2=0.95 D3=0.89 D4=0.00 D5=0.00 D6=0.00
║ ⚡ CONSUMO:
║   • Amperios L1: 45.30 | KW Actual: 32.50
║ 🏭 ORDEN DE PRODUCCIÓN:
║   • Número OP: OP-2024-001 | Estado: 2 (Produciendo)
║   • Kg Producidos: 3542.80 | Metros: 14250.30
║   • Consumo Total OP: 850.25 kW
╚════════════════════════════════════════════════════════════╝
```

### Modo Resumido (LogVerbose = false)

```
📊 [Extrusora 1] OP:OP-2024-001 | Kg/h:148.30 | m/min:25.10 | KgProd:3542.80 | KW:32.50
```

### Resumen de Estado (cada N minutos)

```
═══════════════════════════════════════════════════════════
📈 RESUMEN DE ESTADO DEL SISTEMA - 2024-12-17 14:35:00
═══════════════════════════════════════════════════════════
🖥️  Extrusora 1 (192.168.200.30)
   Estado: ✅ CONECTADA | Última lectura: 14:34:55
   Lecturas exitosas: 125 | Errores: 2

═══════════════════════════════════════════════════════════
```

---

## 📄 Logs

### Ubicación

```
./logs/
├── service-20241217.log
├── service-20241218.log
└── ...
```

### Ver logs en tiempo real

**PowerShell:**
```powershell
Get-Content ./logs/service-$(Get-Date -Format "yyyyMMdd").log -Wait -Tail 50
```

**CMD:**
```batch
tail -f logs\service-*.log
```

---

## 🔧 Desarrollo y Debugging

### VS Code

**Ejecutar en modo debug:**
1. Presiona `F5`
2. O: Run → Start Debugging

**.vscode/launch.json:**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch",
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

### Compilar

```bash
# Debug
dotnet build

# Release
dotnet build -c Release

# Publicar (single file)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

---

## 🛠️ Solución de Problemas

### El servicio no conecta al PLC

1. **Verificar red**
   ```batch
   ping 192.168.200.30
   telnet 192.168.200.30 8000
   ```

2. **Revisar configuración**
   - IP correcta en appsettings.json
   - Puerto correcto (generalmente 8000)
   - Firewall no bloqueando

3. **Revisar logs**
   ```bash
   # Buscar errores de conexión
   type logs\service-*.log | findstr "Error"
   ```

### Datos incorrectos o en ceros

1. **Verificar direcciones de memoria**
   - Revisar constantes en `controlplast-plc.cs`
   - Validar con documentación del PLC

2. **Modo verbose para debug**
   ```json
   "LogVerbose": true
   ```

3. **Timeout muy bajo**
   ```json
   "Timeout": 5000  // Aumentar a 5 segundos
   ```

---

## 📋 Checklist de Despliegue

- [ ] .NET 6.0 SDK instalado
- [ ] appsettings.json configurado
  - [ ] IPs de PLCs correctas
  - [ ] Puertos correctos
  - [ ] Intervalos configurados
  - [ ] Máquinas habilitadas
- [ ] Conexión de red al PLC verificada
- [ ] Logs generándose correctamente
- [ ] Datos leyéndose exitosamente

---

## 🎯 Casos de Uso

### Pruebas Unitarias

```bash
# Ejecutar en consola con verbose
# Ver todos los datos en tiempo real
dotnet run
```

### Validación de Direcciones

```bash
# Ejecutar y verificar que todos los datos se lean correctamente
# Comparar con pantalla del PLC
```

### Monitoreo Continuo

```bash
# Instalar como servicio
# Dejar corriendo en background
Scripts\install-service.bat
Scripts\start-service.bat
```

---

## 📞 Notas Importantes

### Cambios respecto a versión completa

- ✅ **Eliminado**: Base de datos local
- ✅ **Eliminado**: Base de datos en nube
- ✅ **Eliminado**: Servicio de encriptación
- ✅ **Eliminado**: Configuración de sensores a guardar
- ✅ **Mantenido**: Lectura completa de datos
- ✅ **Mantenido**: Sistema de reconexión
- ✅ **Mantenido**: Logs detallados
- ✅ **Simplificado**: Configuración
- ✅ **Optimizado**: Para pruebas y debugging

### Ventajas de esta versión

- 🚀 **Más rápido**: Sin overhead de BD
- 🔧 **Más simple**: Menos dependencias
- 🐛 **Fácil debug**: Salida directa en pantalla
- ✅ **Ideal para pruebas**: Validar lecturas del PLC
- 📊 **Datos en tiempo real**: Sin delays de escritura

---

**Versión**: 3.0 Simplificada  
**Fecha**: Diciembre 2024  
**Propósito**: Pruebas y validación de lecturas PLC  
**Framework**: .NET 6.0

---

¡Listo para pruebas unitarias! 🧪