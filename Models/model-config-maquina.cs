namespace ControlplastPLCService.Models
{
    public class ConfiguracionMaquina
    {
        public string Ip { get; set; } = "192.168.200.31";
        public int Puerto { get; set; } = 8000;
        public int Timeout { get; set; } = 3000;
        public int IntervaloLectura { get; set; } = 5;
        public int IntervaloReconexion { get; set; } = 10;
        public int MaxIntentosReconexion { get; set; } = 5;
        
        // Configuración de datos de produccion a guardar
        public DatosProduccionConfig datosProduccionConfig { get; set; } = new();
    }
    
    public class DatosProduccionConfig
    {
        // =======================
        // PRODUCCIÓN – PROGRAMADO
        // =======================
        public bool GuardarKghProgramado { get; set; } = false;
        public bool GuardarEspessuraProgramada { get; set; } = false;
        public bool GuardarLarguraBrutaProgramada { get; set; } = false;
        public bool GuardarLarguraLiquidaProgramada { get; set; } = false;
        public bool GuardarGramaturaProgramada { get; set; } = false;
        public bool GuardarVelocidadeProgramada { get; set; } = false;

        // =======================
        // PRODUCCIÓN – ACTUAL
        // =======================
        // Mantengo el nombre existente (por compatibilidad) y dejo los principales en false,
        // excepto los valores de producción clave que suelen ser interesantes.
        public bool GuardarKghAtual { get; set; } = true;      // kg/h actual
        public bool GuardarEspessuraAtual { get; set; } = false;
        public bool GuardarLarguraBrutaAtual { get; set; } = false;
        public bool GuardarLarguraLiquidaAtual { get; set; } = false;
        public bool GuardarGramaturaAtual { get; set; } = false;
        public bool GuardarVelocidadeAtual { get; set; } = true; // m/min actual

        // =======================
        // ROSCAS – A..E (grupos)
        // =======================
        // Rosca A
        public bool GuardarRoscaAGramaMetro { get; set; } = false;
        public bool GuardarRoscaAEspessura { get; set; } = false;
        public bool GuardarRoscaAPercentual { get; set; } = false;
        public bool GuardarRoscaAKgh { get; set; } = false;
        public bool GuardarRoscaASilos { get; set; } = false;
        public bool GuardarRoscaATotalizadores { get; set; } = false;
        public bool GuardarRoscaADensidades { get; set; } = false;

        // Rosca B
        public bool GuardarRoscaBGramaMetro { get; set; } = false;
        public bool GuardarRoscaBEspessura { get; set; } = false;
        public bool GuardarRoscaBPercentual { get; set; } = false;
        public bool GuardarRoscaBKgh { get; set; } = false;
        public bool GuardarRoscaBSilos { get; set; } = false;
        public bool GuardarRoscaBTotalizadores { get; set; } = false;
        public bool GuardarRoscaBDensidades { get; set; } = false;

        // Rosca C
        public bool GuardarRoscaCGramaMetro { get; set; } = false;
        public bool GuardarRoscaCEspessura { get; set; } = false;
        public bool GuardarRoscaCPercentual { get; set; } = false;
        public bool GuardarRoscaCKgh { get; set; } = false;
        public bool GuardarRoscaCSilos { get; set; } = false;
        public bool GuardarRoscaCTotalizadores { get; set; } = false;
        public bool GuardarRoscaCDensidades { get; set; } = false;

        // Rosca D
        public bool GuardarRoscaDGramaMetro { get; set; } = false;
        public bool GuardarRoscaDEspessura { get; set; } = false;
        public bool GuardarRoscaDPercentual { get; set; } = false;
        public bool GuardarRoscaDKgh { get; set; } = false;
        public bool GuardarRoscaDSilos { get; set; } = false;
        public bool GuardarRoscaDTotalizadores { get; set; } = false;
        public bool GuardarRoscaDDensidades { get; set; } = false;

        // Rosca E
        public bool GuardarRoscaEGramaMetro { get; set; } = false;
        public bool GuardarRoscaEEspessura { get; set; } = false;
        public bool GuardarRoscaEPercentual { get; set; } = false;
        public bool GuardarRoscaEKgh { get; set; } = false;
        public bool GuardarRoscaESilos { get; set; } = false;
        public bool GuardarRoscaETotalizadores { get; set; } = false;
        public bool GuardarRoscaEDensidades { get; set; } = false;

        // =======================
        // CONSUMO / ENERGÍA
        // =======================
        public bool GuardarTension { get; set; } = false;
        public bool GuardarConsumoAmpere { get; set; } = false;
        public bool GuardarConsumoWatt { get; set; } = true; // consumo en kW: útil por defecto
        public bool GuardarKWTotal { get; set; } = false;
        public bool GuardarKWPorKg { get; set; } = false;
        public bool GuardarKWDia { get; set; } = false;

        // =======================
        // OP / PRODUCCIÓN GENERAL
        // =======================
        public bool GuardarOpNumero { get; set; } = true;
        public bool GuardarOpStatus { get; set; } = true;
        public bool GuardarKgPorMetroOP { get; set; } = false;
        public bool GuardarTamanoBobinaOP { get; set; } = false;
        public bool GuardarRecortesOP { get; set; } = false;

        public bool GuardarKgProduzidos { get; set; } = true;
        public bool GuardarMetrosProduzidos { get; set; } = true;
        public bool GuardarConsumoTotalOp { get; set; } = true;

        // =======================
        // DATOS DE PEDIDO / MÁQUINA
        // =======================
        public bool GuardarMaquinaOcupada { get; set; } = false;
        public bool GuardarNombreMaquina { get; set; } = false;
        public bool GuardarPedido { get; set; } = false;
        public bool GuardarPedidoIniciado { get; set; } = false;
        public bool GuardarPorcentajeB { get; set; } = false;
        public bool GuardarPorcentajeC { get; set; } = false;
        public bool GuardarPrevisionTerminar { get; set; } = false;
        public bool GuardarNombreReceta { get; set; } = false;
        public bool GuardarEstadoPedido { get; set; } = false;
        public bool GuardarTamanoBobina { get; set; } = false;
        public bool GuardarTiempoTotal { get; set; } = false;
    }
}