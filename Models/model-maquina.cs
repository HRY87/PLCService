using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlplastPLCService.Models
{
    public class Maquina
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public ConfiguracionMaquina Configuracion { get; set; } = new();
        public EstadoMaquina Estado { get; set; } = EstadoMaquina.Desconectada;
        public DateTime? UltimaLectura { get; set; }
        public int IntentosReconexion { get; set; }
        public string? UltimoError { get; set; }
        
        private ControlplastPLC? _plcClient;
        private CancellationTokenSource? _monitorCts;
        
        public bool EstaConectada => Estado == EstadoMaquina.Conectada || Estado == EstadoMaquina.Monitoreando;
        public bool EstaMonitoreando => Estado == EstadoMaquina.Monitoreando;
        
        public event EventHandler<DatosProduccionEventArgs>? DatosRecibidos;
        public event EventHandler<EstadoChangedEventArgs>? EstadoChanged;
        public event EventHandler<ModelErrorEventArgs>? ErrorOcurrido;
        
        public async Task<bool> ConectarAsync()
        {
            try
            {
                CambiarEstado(EstadoMaquina.Conectando);
                
                _plcClient = new ControlplastPLC(
                    Configuracion.Ip,
                    Configuracion.Puerto,
                    Configuracion.Timeout
                );
                
                var conectado = await _plcClient.ConnectAsync();
                
                if (conectado)
                {
                    CambiarEstado(EstadoMaquina.Conectada);
                    IntentosReconexion = 0;
                    UltimoError = null;
                    return true;
                }
                else
                {
                    CambiarEstado(EstadoMaquina.ErrorConexion);
                    return false;
                }
            }
            catch (Exception ex)
            {
                UltimoError = ex.Message;
                CambiarEstado(EstadoMaquina.ErrorConexion);
                NotificarError(ex);
                return false;
            }
        }
        
        public async Task IniciarMonitoreoAsync(CancellationToken cancellationToken = default)
        {
            if (_plcClient == null || !_plcClient.Connected)
            {
                throw new InvalidOperationException("La máquina no está conectada");
            }
            
            _monitorCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CambiarEstado(EstadoMaquina.Monitoreando);
            
            await MonitorLoopAsync(_monitorCts.Token);
        }
        
        private async Task MonitorLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_plcClient == null || !_plcClient.Connected)
                    {
                        CambiarEstado(EstadoMaquina.Reconectando);
                        var reconectado = await IntentarReconexionAsync();
                        
                        if (!reconectado)
                        {
                            await Task.Delay(Configuracion.IntervaloReconexion * 1000, cancellationToken);
                            continue;
                        }
                    }
                    
                    var datos = await _plcClient!.GetDatosProduccionAsync();
                    UltimaLectura = DateTime.Now;
                    
                    // Notificar datos recibidos
                    DatosRecibidos?.Invoke(this, new DatosProduccionEventArgs
                    {
                        MaquinaId = Id,
                        Datos = datos,
                        Timestamp = DateTime.Now
                    });
                    
                    await Task.Delay(Configuracion.IntervaloLectura * 1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    UltimoError = ex.Message;
                    NotificarError(ex);
                    
                    CambiarEstado(EstadoMaquina.ErrorLectura);
                    await Task.Delay(5000, cancellationToken);
                }
            }
            
            CambiarEstado(EstadoMaquina.Detenida);
        }
        
        private async Task<bool> IntentarReconexionAsync()
        {
            IntentosReconexion++;
            
            if (IntentosReconexion > Configuracion.MaxIntentosReconexion)
            {
                CambiarEstado(EstadoMaquina.ErrorConexion);
                return false;
            }
            
            Console.WriteLine($"🔄 [{Nombre}] Intento de reconexión #{IntentosReconexion}...");
            
            try
            {
                _plcClient?.Disconnect();
                _plcClient?.Dispose();
                
                var conectado = await ConectarAsync();
                
                if (conectado)
                {
                    Console.WriteLine($"✅ [{Nombre}] Reconexión exitosa");
                    CambiarEstado(EstadoMaquina.Monitoreando);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [{Nombre}] Error en reconexión: {ex.Message}");
                return false;
            }
        }
        
        public void DetenerMonitoreo()
        {
            _monitorCts?.Cancel();
            CambiarEstado(EstadoMaquina.Detenida);
        }
        
        public void Desconectar()
        {
            DetenerMonitoreo();
            _plcClient?.Disconnect();
            _plcClient?.Dispose();
            _plcClient = null;
            CambiarEstado(EstadoMaquina.Desconectada);
        }
        
        private void CambiarEstado(EstadoMaquina nuevoEstado)
        {
            var estadoAnterior = Estado;
            Estado = nuevoEstado;
            
            EstadoChanged?.Invoke(this, new EstadoChangedEventArgs
            {
                MaquinaId = Id,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = nuevoEstado,
                Timestamp = DateTime.Now
            });
        }
        
        private void NotificarError(Exception ex)
        {
            ErrorOcurrido?.Invoke(this, new ModelErrorEventArgs
            {
                MaquinaId = Id,
                Error = ex,
                Timestamp = DateTime.Now
            });
        }
    }
    
    public enum EstadoMaquina
    {
        Desconectada,
        Conectando,
        Conectada,
        Monitoreando,
        Reconectando,
        ErrorConexion,
        ErrorLectura,
        Detenida
    }
}