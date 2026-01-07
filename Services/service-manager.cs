using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ControlplastPLCService.Models;

namespace ControlplastPLCService.Services
{
    public class MaquinaManagerService : IDisposable
    {
        private readonly List<Maquina> _maquinas = new();
        //private readonly IDatabaseService _dbLocal;
        //private readonly IDatabaseService? _dbNube;
        private readonly ConfiguracionSistema _config;
        private CancellationTokenSource? _globalCts;
        private bool _disposed;
        
        public IReadOnlyList<Maquina> Maquinas => _maquinas.AsReadOnly();
        
        public event EventHandler<DatosProduccionEventArgs>? DatosRecibidos;
        public event EventHandler<EstadoChangedEventArgs>? EstadoCambiado;
        public event EventHandler<ModelErrorEventArgs>? ErrorOcurrido;
        
        public MaquinaManagerService(
            ConfiguracionSistema config)
            //IDatabaseService dbLocal,
            //IDatabaseService? dbNube = null)
        {
            _config = config;
            //_dbLocal = dbLocal;
            //_dbNube = dbNube;
            
            InicializarMaquinas();
        }
        
        private void InicializarMaquinas()
        {
            foreach (var maqConfig in _config.Maquinas.Where(m => m.Habilitada))
            {
                var maquina = new Maquina
                {
                    Id = maqConfig.Id,
                    Nombre = maqConfig.Nombre,
                    Descripcion = maqConfig.Descripcion,
                    Configuracion = maqConfig.Configuracion
                };
                
                // Suscribirse a eventos de la máquina
                maquina.DatosRecibidos += OnMaquinaDatosRecibidos;
                maquina.EstadoChanged += OnMaquinaEstadoChanged;
                maquina.ErrorOcurrido += OnMaquinaError;
                
                _maquinas.Add(maquina);
                
                Console.WriteLine($"📍 Máquina agregada: {maquina.Nombre} ({maquina.Configuracion.Ip})");
            }
        }
        
        public async Task<bool> IniciarTodoAsync()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          INICIANDO SISTEMA DE MONITOREO                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
            /*
            // 1. Conectar base de datos local
            Console.WriteLine("🗄️  Conectando a base de datos local...");
            if (!await _dbLocal.ConectarAsync())
            {
                Console.WriteLine("❌ No se pudo conectar a la base de datos local");
                return false;
            }
            
            // 2. Conectar base de datos nube (opcional)
            if (_dbNube != null)
            {
                Console.WriteLine("☁️  Conectando a base de datos en nube...");
                await _dbNube.ConectarAsync(); // No es crítico si falla
            }
            */
            // 3. Conectar todas las máquinas
            Console.WriteLine($"\n🏭 Conectando {_maquinas.Count} máquina(s)...");
            var tareasConexion = _maquinas.Select(m => ConectarMaquinaAsync(m)).ToArray();
            await Task.WhenAll(tareasConexion);
            
            var conectadas = _maquinas.Count(m => m.EstaConectada);
            Console.WriteLine($"\n✅ {conectadas}/{_maquinas.Count} máquinas conectadas");
            
            if (conectadas == 0)
            {
                Console.WriteLine("❌ No se pudo conectar ninguna máquina");
                return false;
            }
            
            // 4. Iniciar monitoreo de todas las máquinas
            _globalCts = new CancellationTokenSource();
            var tareasMonitoreo = _maquinas
                .Where(m => m.EstaConectada)
                .Select(m => IniciarMonitoreoMaquinaAsync(m, _globalCts.Token))
                .ToArray();
            
            // No esperar, ejecutar en background
            _ = Task.WhenAll(tareasMonitoreo).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine($"⚠️  Error en monitoreo: {t.Exception?.Message}");
                }
            });
            
            Console.WriteLine("\n🚀 Sistema de monitoreo iniciado correctamente\n");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
            
            return true;
        }
        
        private async Task ConectarMaquinaAsync(Maquina maquina)
        {
            Console.WriteLine($"  🔌 Conectando a {maquina.Nombre}...");
            
            var conectada = await maquina.ConectarAsync();
            
            if (conectada)
            {
                Console.WriteLine($"  ✅ {maquina.Nombre} conectada");
            }
            else
            {
                Console.WriteLine($"  ❌ {maquina.Nombre} no pudo conectar");
            }
        }
        
        private async Task IniciarMonitoreoMaquinaAsync(Maquina maquina, CancellationToken cancellationToken)
        {
            try
            {
                await maquina.IniciarMonitoreoAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en monitoreo de {maquina.Nombre}: {ex.Message}");
            }
        }
        
        private async void OnMaquinaDatosRecibidos(object? sender, DatosProduccionEventArgs e)
        {
            var maquina = _maquinas.FirstOrDefault(m => m.Id == e.MaquinaId);
            if (maquina == null) return;
            
            try
            {/*
                // Guardar en base de datos local (histórico)
                if (_config.DatabaseLocal.GuardarHistorico)
                {
                    await _dbLocal.GuardarDatosProduccionAsync(
                        e.MaquinaId,
                        e.Datos,
                        maquina.Configuracion.Sensores
                    );
                }
                
                // Actualizar datos actuales en local
                await _dbLocal.ActualizarDatosActualesAsync(
                    e.MaquinaId,
                    e.Datos,
                    maquina.Configuracion.Sensores
                );
                
                // Intentar guardar en nube
                if (_dbNube != null && _dbNube.EstaConectada)
                {
                    await _dbNube.ActualizarDatosActualesAsync(
                        e.MaquinaId,
                        e.Datos,
                        maquina.Configuracion.Sensores
                    );
                }
                else if (_dbNube != null && !_dbNube.EstaConectada)
                {
                    // Intentar reconectar a la nube
                    _ = Task.Run(async () => await _dbNube.ConectarAsync());
                }
                */
                // Propagar evento
                DatosRecibidos?.Invoke(this, e);
                
                // Log resumido solo si verbose está activado
                if (_config.General.LogVerbose)
                {
                    Console.WriteLine($"📊 [{maquina.Nombre}] kg/h: {e.Datos.KgHoraActual:F2} | " +
                                    $"m/min: {e.Datos.MetrosPorMinActual:F2} | " +
                                    $"OP: {e.Datos.NumeroOP}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Error procesando datos de {maquina.Nombre}: {ex.Message}");
            }
        }
        
        private void OnMaquinaEstadoChanged(object? sender, EstadoChangedEventArgs e)
        {
            var maquina = _maquinas.FirstOrDefault(m => m.Id == e.MaquinaId);
            if (maquina == null) return;
            
            var emoji = e.EstadoNuevo switch
            {
                EstadoMaquina.Conectada => "✅",
                EstadoMaquina.Monitoreando => "🔄",
                EstadoMaquina.Reconectando => "🔄",
                EstadoMaquina.ErrorConexion => "❌",
                EstadoMaquina.ErrorLectura => "⚠️",
                EstadoMaquina.Detenida => "⏸️",
                _ => "ℹ️"
            };
            
            Console.WriteLine($"{emoji} [{maquina.Nombre}] Estado: {e.EstadoAnterior} → {e.EstadoNuevo}");
            
            // Propagar evento
            EstadoCambiado?.Invoke(this, e);
        }
        
        private void OnMaquinaError(object? sender, Models.ModelErrorEventArgs e)
        {
            var maquina = _maquinas.FirstOrDefault(m => m.Id == e.MaquinaId);
            if (maquina == null) return;
            
            Console.WriteLine($"❌ [{maquina.Nombre}] Error: {e.Error.Message}");
            
            // Propagar evento
            ErrorOcurrido?.Invoke(this, e);
        }
        
        public void DetenerTodo()
        {
            Console.WriteLine("\n🛑 Deteniendo sistema de monitoreo...");
            
            _globalCts?.Cancel();
            
            foreach (var maquina in _maquinas)
            {
                try
                {
                    maquina.DetenerMonitoreo();
                    maquina.Desconectar();
                    Console.WriteLine($"  ✅ {maquina.Nombre} detenida");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️  Error deteniendo {maquina.Nombre}: {ex.Message}");
                }
            }
            
            Console.WriteLine("✅ Sistema detenido correctamente");
        }
        
        public async Task<Dictionary<string, object>> ObtenerEstadoSistemaAsync()
        {
            var estado = new Dictionary<string, object>
            {
                ["TotalMaquinas"] = _maquinas.Count,
                ["MaquinasConectadas"] = _maquinas.Count(m => m.EstaConectada),
                ["MaquinasMonitoreando"] = _maquinas.Count(m => m.EstaMonitoreando),
                //["BaseDatosLocal"] = _dbLocal.EstaConectada ? "Conectada" : "Desconectada",
                //["BaseDatosNube"] = _dbNube?.EstaConectada == true ? "Conectada" : "Desconectada",
                ["UltimaActualizacion"] = DateTime.Now
            };
            
            var maquinasDetalle = _maquinas.Select(m => new Dictionary<string, object>
            {
                ["Id"] = m.Id,
                ["Nombre"] = m.Nombre,
                ["Estado"] = m.Estado.ToString(),
                ["IP"] = m.Configuracion.Ip,
                ["UltimaLectura"] = m.UltimaLectura?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca",
                ["IntentosReconexion"] = m.IntentosReconexion
            }).ToList();
            
            estado["Maquinas"] = maquinasDetalle;
            
            return await Task.FromResult(estado);
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            DetenerTodo();
            
            _globalCts?.Dispose();
            
            try
            {
                // _dbLocal?.DesconectarAsync().Wait();
                // _dbNube?.DesconectarAsync().Wait();
            }
            catch { }
            
            _disposed = true;
        }
    }
}