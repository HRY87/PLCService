using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ControlplastPLCService.Models;
using System.Collections.Generic;
using System.Linq;

namespace ControlplastPLCService
{
    /// <summary>
    /// Worker principal que gestiona el monitoreo de múltiples PLCs
    /// Lee datos periódicamente y los muestra en pantalla/logs
    /// </summary>
    public class PLCMonitorWorker : BackgroundService
    {
        private readonly ILogger<PLCMonitorWorker> _logger;
        private readonly ConfiguracionSistema _config;
        private readonly List<MonitoreoMaquina> _monitores = new();
        private Timer? _statusTimer;

        public PLCMonitorWorker(
            ILogger<PLCMonitorWorker> logger,
            ConfiguracionSistema config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("╔════════════════════════════════════════════════════════════╗");
                _logger.LogInformation("║     SERVICIO DE MONITOREO PLC - MODO PRUEBAS              ║");
                _logger.LogInformation("╚════════════════════════════════════════════════════════════╝");
                _logger.LogInformation("");
                
                // Validar configuración
                if (_config == null)
                {
                    _logger.LogError("❌ Configuración es null");
                    return;
                }
                
                if (_config.Maquinas == null)
                {
                    _logger.LogError("❌ Lista de máquinas es null");
                    return;
                }
                
                var maquinasHabilitadas = _config.Maquinas.Where(m => m.Habilitada).ToList();
                
                _logger.LogInformation("📊 Configuración cargada:");
                _logger.LogInformation("   Total máquinas definidas: {Total}", _config.Maquinas.Count);
                _logger.LogInformation("   Máquinas habilitadas: {Habilitadas}", maquinasHabilitadas.Count);
                _logger.LogInformation("");
                
                if (maquinasHabilitadas.Count == 0)
                {
                    _logger.LogError("❌ No hay máquinas habilitadas en la configuración");
                    _logger.LogInformation("💡 Verifique appsettings.json y asegúrese de que 'Habilitada': true");
                    return;
                }

                // Inicializar monitores para cada máquina habilitada
                foreach (var maqConfig in maquinasHabilitadas)
                {
                    // Validar configuración de la máquina
                    if (maqConfig.Configuracion == null)
                    {
                        _logger.LogWarning("⚠️  Máquina '{Nombre}' no tiene configuración, se omite", 
                            maqConfig.Nombre);
                        continue;
                    }
                    
                    if (string.IsNullOrEmpty(maqConfig.Configuracion.Ip))
                    {
                        _logger.LogWarning("⚠️  Máquina '{Nombre}' no tiene IP configurada, se omite", 
                            maqConfig.Nombre);
                        continue;
                    }
                    
                    var monitor = new MonitoreoMaquina
                    {
                        Config = maqConfig,
                        PLC = new ControlplastPLC(
                            maqConfig.Configuracion.Ip,
                            maqConfig.Configuracion.Puerto,
                            maqConfig.Configuracion.Timeout
                        )
                    };

                    _monitores.Add(monitor);
                    _logger.LogInformation("📍 Máquina agregada: {Nombre} ({IP}:{Puerto})",
                        maqConfig.Nombre, 
                        maqConfig.Configuracion.Ip, 
                        maqConfig.Configuracion.Puerto);
                }
                
                if (_monitores.Count == 0)
                {
                    _logger.LogError("❌ No se pudo agregar ninguna máquina válida");
                    _logger.LogInformation("💡 Verifique que las máquinas tengan IP y configuración correcta");
                    return;
                }

                _logger.LogInformation("");
                _logger.LogInformation("🔌 Conectando a máquinas...");
                _logger.LogInformation("");

                // Conectar todas las máquinas
                var tareasConexion = _monitores.Select(m => ConectarMaquinaAsync(m)).ToArray();
                await Task.WhenAll(tareasConexion);

                var conectadas = _monitores.Count(m => m.Conectada);
                _logger.LogInformation("");
                _logger.LogInformation("✅ {Conectadas}/{Total} máquinas conectadas exitosamente",
                    conectadas, _monitores.Count);

                if (conectadas == 0)
                {
                    _logger.LogError("❌ No se pudo conectar ninguna máquina. Verifique la configuración de red.");
                    return;
                }

                _logger.LogInformation("");
                _logger.LogInformation("🚀 Iniciando monitoreo continuo...");
                _logger.LogInformation("════════════════════════════════════════════════════════════");
                _logger.LogInformation("");

                // Configurar timer para resumen de estado periódico
                _statusTimer = new Timer(
                    LogResumenEstado,
                    null,
                    TimeSpan.FromMinutes(_config.General.IntervaloLogEstadoMinutos),
                    TimeSpan.FromMinutes(_config.General.IntervaloLogEstadoMinutos)
                );

                // Iniciar loops de monitoreo para cada máquina conectada
                var tareasMonitoreo = _monitores
                    .Where(m => m.Conectada)
                    .Select(m => MonitorearMaquinaAsync(m, stoppingToken))
                    .ToArray();

                await Task.WhenAll(tareasMonitoreo);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("⏹️  Servicio detenido por solicitud de cancelación");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "❌ Error crítico en el servicio");
                throw;
            }
        }

        /// <summary>
        /// Conecta a una máquina PLC específica
        /// </summary>
        private async Task ConectarMaquinaAsync(MonitoreoMaquina monitor)
        {
            try
            {
                _logger.LogInformation("  🔌 Conectando a {Nombre}...", monitor.Config.Nombre);
                
                monitor.Conectada = await monitor.PLC.ConnectAsync();
                
                if (monitor.Conectada)
                {
                    _logger.LogInformation("  ✅ {Nombre} conectada exitosamente", monitor.Config.Nombre);
                }
                else
                {
                    _logger.LogWarning("  ⚠️  {Nombre} no pudo conectar", monitor.Config.Nombre);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "  ❌ Error conectando a {Nombre}", monitor.Config.Nombre);
                monitor.Conectada = false;
            }
        }

        /// <summary>
        /// Loop principal de monitoreo para una máquina
        /// Lee datos periódicamente y los muestra en pantalla
        /// </summary>
        private async Task MonitorearMaquinaAsync(MonitoreoMaquina monitor, CancellationToken stoppingToken)
        {
            var intervalo = monitor.Config.Configuracion.IntervaloLectura;
            var intentosReconexion = 0;
            var maxIntentos = monitor.Config.Configuracion.MaxIntentosReconexion;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Si no está conectado, intentar reconectar
                    if (!monitor.PLC.Connected)
                    {
                        intentosReconexion++;
                        
                        if (intentosReconexion > maxIntentos)
                        {
                            _logger.LogError("❌ [{Nombre}] Máximo de reintentos alcanzado. Deteniendo monitoreo.",
                                monitor.Config.Nombre);
                            break;
                        }

                        _logger.LogWarning("🔄 [{Nombre}] Intentando reconectar... (intento {Intento}/{Max})",
                            monitor.Config.Nombre, intentosReconexion, maxIntentos);

                        monitor.PLC.Disconnect();
                        monitor.Conectada = await monitor.PLC.ConnectAsync();

                        if (monitor.Conectada)
                        {
                            _logger.LogInformation("✅ [{Nombre}] Reconexión exitosa", monitor.Config.Nombre);
                            intentosReconexion = 0;
                        }
                        else
                        {
                            await Task.Delay(monitor.Config.Configuracion.IntervaloReconexion * 1000, stoppingToken);
                            continue;
                        }
                    }

                    // Leer datos del PLC
                    var datos = await monitor.PLC.GetDatosProduccionAsync();
                    monitor.UltimaLectura = DateTime.Now;
                    monitor.LecturasExitosas++;

                    // Mostrar datos en pantalla
                    MostrarDatosProduccion(monitor.Config.Nombre, datos);

                    // Resetear contador de errores en lectura exitosa
                    intentosReconexion = 0;

                    // Esperar intervalo configurado
                    await Task.Delay(intervalo * 1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    monitor.ErroresLectura++;
                    _logger.LogError(ex, "⚠️  [{Nombre}] Error leyendo datos del PLC", monitor.Config.Nombre);
                    
                    // Marcar como desconectado para intentar reconexión
                    monitor.PLC.Disconnect();
                    monitor.Conectada = false;

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// Muestra los datos de producción en pantalla/log
        /// </summary>
        private void MostrarDatosProduccion(string nombreMaquina, DatosProduccion datos)
        {
            // Si verbose está activado, mostrar datos detallados
            if (_config.General.LogVerbose)
            {
                _logger.LogInformation("╔════════════════════════════════════════════════════════════╗");
                _logger.LogInformation("║  [{Nombre}] - LECTURA COMPLETA - {Timestamp}",
                    nombreMaquina, DateTime.Now.ToString("HH:mm:ss"));
                _logger.LogInformation("╠════════════════════════════════════════════════════════════╣");
                
                // Producción Programada
                _logger.LogInformation("║ 📋 PROGRAMADO:");
                _logger.LogInformation("║   • Kg/h: {KgH:F2} | Espesor: {Esp:F2} | Ancho Bruto: {AB:F2}",
                    datos.KgHoraProgramado, datos.EspesorProgramado, datos.AnchoBrutoProgramado);
                _logger.LogInformation("║   • Ancho Neto: {AN:F2} | Gramaje: {G:F2} | m/min: {V:F2}",
                    datos.AnchoNetoProgramado, datos.GramajeProgramado, datos.MetrosPorMinProgramado);
                
                // Producción Actual
                _logger.LogInformation("║ 📊 ACTUAL:");
                _logger.LogInformation("║   • Kg/h: {KgH:F2} | Espesor: {Esp:F2} | Ancho Bruto: {AB:F2}",
                    datos.KgHoraActual, datos.EspesorActual, datos.AnchoBrutoActual);
                _logger.LogInformation("║   • Ancho Neto: {AN:F2} | Gramaje: {G:F2} | m/min: {V:F2}",
                    datos.AnchoNetoActual, datos.GramajeActual, datos.MetrosPorMinActual);
                
                // Rosca A
                _logger.LogInformation("║ 🔧 ROSCA A:");
                _logger.LogInformation("║   • g/m: {GM:F2} | Espesor: {E:F2} | %: {P:F2} | Kg/h: {K:F2}",
                    datos.RoscaA_GramaMetroActual, datos.RoscaA_EspesorActual,
                    datos.RoscaA_PorcentajeActual, datos.RoscaA_KgHoraActual);
                _logger.LogInformation("║   • Silos: S1={S1:F1} S2={S2:F1} S3={S3:F1} S4={S4:F1} S5={S5:F1} S6={S6:F1}",
                    datos.RoscaA_Silo1Actual, datos.RoscaA_Silo2Actual, datos.RoscaA_Silo3Actual,
                    datos.RoscaA_Silo4Actual, datos.RoscaA_Silo5Actual, datos.RoscaA_Silo6Actual);
                
                // Totalizadores
                _logger.LogInformation("║   • Totales: T1={T1:F1} T2={T2:F1} T3={T3:F1} T4={T4:F1} T5={T5:F1} T6={T6:F1}",
                    datos.RoscaA_TotalSilo1, datos.RoscaA_TotalSilo2, datos.RoscaA_TotalSilo3,
                    datos.RoscaA_TotalSilo4, datos.RoscaA_TotalSilo5, datos.RoscaA_TotalSilo6);
                
                // Densidades
                _logger.LogInformation("║   • Densidades: D1={D1:F2} D2={D2:F2} D3={D3:F2} D4={D4:F2} D5={D5:F2} D6={D6:F2}",
                    datos.RoscaA_DensidadSilo1, datos.RoscaA_DensidadSilo2, datos.RoscaA_DensidadSilo3,
                    datos.RoscaA_DensidadSilo4, datos.RoscaA_DensidadSilo5, datos.RoscaA_DensidadSilo6);
                
                // Consumo
                _logger.LogInformation("║ ⚡ CONSUMO:");
                _logger.LogInformation("║   • Amperios L1: {A:F2} | KW Actual: {KW:F2}",
                    datos.AmperesL1, datos.ConsumoActualKW);
                
                // OP
                _logger.LogInformation("║ 🏭 ORDEN DE PRODUCCIÓN:");
                _logger.LogInformation("║   • Número OP: {OP} | Estado: {Estado} ({Desc})",
                    datos.NumeroOP, datos.EstadoOP, datos.GetDescripcionEstadoOP());
                _logger.LogInformation("║   • Kg Producidos: {KgP:F2} | Metros: {M:F2}",
                    datos.KgProducidos, datos.MetrosProducidos);
                _logger.LogInformation("║   • Consumo Total OP: {C:F2} kW",
                    datos.ConsumoTotalOP);
                
                _logger.LogInformation("╚════════════════════════════════════════════════════════════╝");
                _logger.LogInformation("");
            }
            else
            {
                // Modo resumido: solo datos clave
                _logger.LogInformation("📊 [{Nombre}] OP:{OP} | Kg/h:{KgH:F2} | m/min:{Vel:F2} | KgProd:{KgP:F2} | KW:{KW:F2}",
                    nombreMaquina,
                    datos.NumeroOP ?? "N/A",
                    datos.KgHoraActual,
                    datos.MetrosPorMinActual,
                    datos.KgProducidos,
                    datos.ConsumoActualKW);
            }
        }

        /// <summary>
        /// Log periódico del estado general del sistema
        /// </summary>
        private void LogResumenEstado(object? state)
        {
            try
            {
                _logger.LogInformation("");
                _logger.LogInformation("═══════════════════════════════════════════════════════════");
                _logger.LogInformation("📈 RESUMEN DE ESTADO DEL SISTEMA - {Timestamp}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _logger.LogInformation("═══════════════════════════════════════════════════════════");

                foreach (var monitor in _monitores)
                {
                    var estado = monitor.Conectada ? "✅ CONECTADA" : "❌ DESCONECTADA";
                    var ultimaLectura = monitor.UltimaLectura?.ToString("HH:mm:ss") ?? "Nunca";

                    _logger.LogInformation("🖥️  {Nombre} ({IP})",
                        monitor.Config.Nombre,
                        monitor.Config.Configuracion.Ip);
                    _logger.LogInformation("   Estado: {Estado} | Última lectura: {Ultima}",
                        estado, ultimaLectura);
                    _logger.LogInformation("   Lecturas exitosas: {Exitosas} | Errores: {Errores}",
                        monitor.LecturasExitosas, monitor.ErroresLectura);
                    _logger.LogInformation("");
                }

                _logger.LogInformation("═══════════════════════════════════════════════════════════");
                _logger.LogInformation("");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando resumen de estado");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("");
            _logger.LogInformation("🛑 Deteniendo servicio de monitoreo...");
            
            _statusTimer?.Dispose();

            // Desconectar todas las máquinas
            foreach (var monitor in _monitores)
            {
                try
                {
                    monitor.PLC?.Disconnect();
                    monitor.PLC?.Dispose();
                    _logger.LogInformation("  ✅ {Nombre} desconectada", monitor.Config.Nombre);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "  ⚠️  Error desconectando {Nombre}", monitor.Config.Nombre);
                }
            }

            await base.StopAsync(cancellationToken);
            
            _logger.LogInformation("✅ Servicio detenido correctamente");
            _logger.LogInformation("");
        }

        /// <summary>
        /// Clase auxiliar para gestionar el monitoreo de cada máquina
        /// </summary>
        private class MonitoreoMaquina
        {
            public MaquinaConfig Config { get; set; } = null!;
            public ControlplastPLC PLC { get; set; } = null!;
            public bool Conectada { get; set; }
            public DateTime? UltimaLectura { get; set; }
            public long LecturasExitosas { get; set; }
            public long ErroresLectura { get; set; }
        }
    }
}