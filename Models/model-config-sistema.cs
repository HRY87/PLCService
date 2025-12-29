using System.Collections.Generic;

namespace ControlplastPLCService.Models
{
    public class ConfiguracionSistema
    {
        public List<MaquinaConfig> Maquinas { get; set; } = new();
        public DatabaseConfig DatabaseLocal { get; set; } = new();
        public DatabaseConfig? DatabaseNube { get; set; }
        public GeneralConfig General { get; set; } = new();
    }
    
    public class MaquinaConfig
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Habilitada { get; set; } = true;
        public ConfiguracionMaquina Configuracion { get; set; } = new();
    }
    
    public class DatabaseConfig
    {
        public string Tipo { get; set; } = "SqlServer";
        public string Host { get; set; } = "localhost";
        public int Puerto { get; set; } = 1433;
        public string Database { get; set; } = "ControlplastPLC";
        public string Usuario { get; set; } = "sa";
        public string Password { get; set; } = string.Empty;
        public bool UsarEncriptacion { get; set; } = true;
        public int TimeoutSegundos { get; set; } = 30;
        public bool GuardarHistorico { get; set; } = true;
    }
    
    public class GeneralConfig
    {
        public string RutaLogs { get; set; } = "logs";
        public int RetencionLogsDias { get; set; } = 30;
        public bool LogVerbose { get; set; } = false;
    }
}