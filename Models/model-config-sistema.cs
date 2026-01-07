using System.Collections.Generic;

namespace ControlplastPLCService.Models
{
    /// <summary>
    /// Configuración general del sistema de monitoreo
    /// </summary>
    public class ConfiguracionSistema
    {
        public List<MaquinaConfig> Maquinas { get; set; } = new();
        public GeneralConfig General { get; set; } = new();
    }
    
    /// <summary>
    /// Configuración individual de cada máquina PLC
    /// </summary>
    public class MaquinaConfig
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Habilitada { get; set; } = true;
        public ConfiguracionMaquina Configuracion { get; set; } = new();
    }
    
    /// <summary>
    /// Configuración general del sistema (logs, etc)
    /// </summary>
    public class GeneralConfig
    {
        public string RutaLogs { get; set; } = "logs";
        public int RetencionLogsDias { get; set; } = 30;
        public bool LogVerbose { get; set; } = false;
        public int IntervaloLogEstadoMinutos { get; set; } = 5;
    }
}