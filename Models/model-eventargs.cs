using System;

namespace ControlplastPLCService.Models
{
    public class DatosProduccionEventArgs : EventArgs
    {
        public int MaquinaId { get; set; }
        public DatosProduccion Datos { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
    
    public class EstadoChangedEventArgs : EventArgs
    {
        public int MaquinaId { get; set; }
        public EstadoMaquina EstadoAnterior { get; set; }
        public EstadoMaquina EstadoNuevo { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public class ModelErrorEventArgs : EventArgs
    {
        public int MaquinaId { get; set; }
        public Exception Error { get; set; } = new Exception();
        public DateTime Timestamp { get; set; }
    }
}