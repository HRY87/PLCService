using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ControlplastPLCService.Models;
using ControlplastPLCService.Services;

namespace ControlplastPLCService
{
    public class PLCMonitorWorker : BackgroundService
    {
        private readonly ILogger<PLCMonitorWorker> _logger;
        private readonly MaquinaManagerService _manager;
        private readonly ConfiguracionSistema _config;
        private Timer? _statusTimer;

        public PLCMonitorWorker(
            ILogger<PLCMonitorWorker> logger,
            MaquinaManagerService manager,
            ConfiguracionSistema config)
        {
            _logger = logger;
            _manager = manager;
            _config = config;
        }
        
        //Mostrar datos en pantalla para pruebas
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de prueba Producción iniciado");

            var plc = new ControlplastPLC("192.168.200.30", 8000, 3000);
            await plc.ConnectAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                var datos = await plc.GetDatosProduccionAsync();

                // Mostrar todo el objeto DatosProduccion
                _logger.LogInformation("=== Lectura periódica completa ===");
                _logger.LogInformation("Programado: Kg/h={KgHoraProg}, Espesor={EspesorProg}, AnchoBruto={AnchoBrutoProg}, AnchoNeto={AnchoNetoProg}, Gramaje={GramajeProg}, m/min={MetrosProg}",
                    datos.KgHoraProgramado,
                    datos.EspesorProgramado,
                    datos.AnchoBrutoProgramado,
                    datos.AnchoNetoProgramado,
                    datos.GramajeProgramado,
                    datos.MetrosPorMinProgramado);

                _logger.LogInformation("Actual: Kg/h={KgHoraAct}, Espesor={EspesorAct}, AnchoBruto={AnchoBrutoAct}, AnchoNeto={AnchoNetoAct}, Gramaje={GramajeAct}, m/min={MetrosAct}",
                    datos.KgHoraActual,
                    datos.EspesorActual,
                    datos.AnchoBrutoActual,
                    datos.AnchoNetoActual,
                    datos.GramajeActual,
                    datos.MetrosPorMinActual);

                _logger.LogInformation("Rosca A Programado: GramaMetro={GramaProg}, Espesor={EspesorProg}, %={PorcentajeProg}, Kg/h={KgHoraProg}",
                    datos.RoscaA_GramaMetroProgramado,
                    datos.RoscaA_EspesorProgramado,
                    datos.RoscaA_PorcentajeProgramado,
                    datos.RoscaA_KgHoraProgramado);

                _logger.LogInformation("Rosca A Actual: GramaMetro={GramaAct}, Espesor={EspesorAct}, %={PorcentajeAct}, Kg/h={KgHoraAct}",
                    datos.RoscaA_GramaMetroActual,
                    datos.RoscaA_EspesorActual,
                    datos.RoscaA_PorcentajeActual,
                    datos.RoscaA_KgHoraActual);

                _logger.LogInformation("Consumo: AmperesL1={AmpL1}, KW Actual={KwAct}, KW Total OP={KwTotalOp}",
                    datos.AmperesL1,
                    datos.ConsumoActualKW,
                    datos.ConsumoTotalOP);

                _logger.LogInformation("OP: Numero={NumOP}, Estado={EstadoOP}, KgProducidos={KgProd}, MetrosProducidos={MetrosProd}",
                    datos.NumeroOP,
                    datos.EstadoOP,
                    datos.KgProducidos,
                    datos.MetrosProducidos);

                _logger.LogInformation("Totalizadores Rosca A: Silo1={S1}, Silo2={S2}, Silo3={S3}, Silo4={S4}, Silo5={S5}, Silo6={S6}",
                    datos.RoscaA_TotalSilo1,
                    datos.RoscaA_TotalSilo2,
                    datos.RoscaA_TotalSilo3,
                    datos.RoscaA_TotalSilo4,
                    datos.RoscaA_TotalSilo5,
                    datos.RoscaA_TotalSilo6);

                _logger.LogInformation("Densidades Rosca A: Silo1={D1}, Silo2={D2}, Silo3={D3}, Silo4={D4}, Silo5={D5}, Silo6={D6}",
                    datos.RoscaA_DensidadSilo1,
                    datos.RoscaA_DensidadSilo2,
                    datos.RoscaA_DensidadSilo3,
                    datos.RoscaA_DensidadSilo4,
                    datos.RoscaA_DensidadSilo5,
                    datos.RoscaA_DensidadSilo6);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

/*
        //Mostrar estado del sistema cada 5 minutos
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            try
            {
                _logger.LogInformation("Servicio de monitoreo PLC iniciado");
                _logger.LogInformation("Configuración: {MaquinasCount} máquinas", 
                    _config.Maquinas.Count);

                // Suscribirse a eventos
                _manager.DatosRecibidos += OnDatosRecibidos;
                _manager.EstadoCambiado += OnEstadoCambiado;
                _manager.ErrorOcurrido += OnErrorOcurrido;

                // Iniciar sistema
                var iniciado = await _manager.IniciarTodoAsync();
                
                if (!iniciado)
                {
                    _logger.LogError("No se pudo iniciar el sistema de monitoreo");
                    return;
                }

                _logger.LogInformation("Sistema de monitoreo iniciado correctamente");
                
                // Timer para log de estado cada 5 minutos
                _statusTimer = new Timer(
                    LogEstadoSistema,
                    null,
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(5)
                );

                // Mantener el servicio ejecutándose
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Servicio detenido por solicitud de cancelación");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error crítico en el servicio");
                throw;
            }

        }
*/
        private void LogEstadoSistema(object? state)
        {
            try
            {
                var estado = _manager.ObtenerEstadoSistemaAsync().Result;
                
                _logger.LogInformation("=== Estado del Sistema ===");
                _logger.LogInformation("Máquinas Conectadas: {Conectadas}/{Total}",
                    estado["MaquinasConectadas"], estado["TotalMaquinas"]);
                _logger.LogInformation("Máquinas Monitoreando: {Monitoreando}",
                    estado["MaquinasMonitoreando"]);
                _logger.LogInformation("BD Local: {Estado}", estado["BaseDatosLocal"]);
                _logger.LogInformation("BD Nube: {Estado}", estado["BaseDatosNube"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estado del sistema");
            }
        }

        private void OnDatosRecibidos(object? sender, DatosProduccionEventArgs e)
        {
            if (_config.General.LogVerbose)
            {
                _logger.LogDebug("Datos recibidos de máquina {MaquinaId}: kg/h={KgHora}, m/min={Metros}",
                    e.MaquinaId, e.Datos.KgHoraActual, e.Datos.MetrosPorMinActual);
            }
        }

        private void OnEstadoCambiado(object? sender, EstadoChangedEventArgs e)
        {
            _logger.LogInformation("Máquina {MaquinaId}: Estado cambió de {EstadoAnterior} a {EstadoNuevo}",
                e.MaquinaId, e.EstadoAnterior, e.EstadoNuevo);
        }

        private void OnErrorOcurrido(object? sender, Models.ModelErrorEventArgs e)
        {
            _logger.LogError(e.Error, "Error en máquina {MaquinaId}", e.MaquinaId);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deteniendo servicio de monitoreo PLC...");
            
            _statusTimer?.Dispose();
            _manager?.DetenerTodo();
            _manager?.Dispose();
            
            await base.StopAsync(cancellationToken);
            
            _logger.LogInformation("Servicio detenido correctamente");
        }
    }
}