namespace ControlplastPLCService.Models
{
    /// <summary>
    /// Configuración de conexión y monitoreo de una máquina PLC
    /// </summary>
    public class ConfiguracionMaquina
    {
        /// <summary>
        /// Dirección IP del PLC
        /// </summary>
        public string Ip { get; set; } = "192.168.200.31";
        
        /// <summary>
        /// Puerto TCP para la conexión
        /// </summary>
        public int Puerto { get; set; } = 8000;
        
        /// <summary>
        /// Timeout de conexión en milisegundos
        /// </summary>
        public int Timeout { get; set; } = 3000;
        
        /// <summary>
        /// Intervalo entre lecturas en segundos
        /// </summary>
        public int IntervaloLectura { get; set; } = 5;
        
        /// <summary>
        /// Intervalo de espera entre intentos de reconexión en segundos
        /// </summary>
        public int IntervaloReconexion { get; set; } = 10;
        
        /// <summary>
        /// Número máximo de intentos de reconexión antes de detener el monitoreo
        /// </summary>
        public int MaxIntentosReconexion { get; set; } = 5;
    }
}