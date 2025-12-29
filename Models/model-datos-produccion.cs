namespace ControlplastPLCService.Models
{
    public class DatosProduccion
    {
        // ==================================================
        // PRODUCCIÓN – PROGRAMADO
        // ==================================================
        public double KgHoraProgramado { get; set; }
        public double EspesorProgramado { get; set; }
        public double AnchoBrutoProgramado { get; set; }
        public double AnchoNetoProgramado { get; set; }
        public double GramajeProgramado { get; set; }
        public double MetrosPorMinProgramado { get; set; }

        // ==================================================
        // PRODUCCIÓN – ACTUAL
        // ==================================================
        public double KgHoraActual { get; set; }
        public double EspesorActual { get; set; }
        public double AnchoBrutoActual { get; set; }
        public double AnchoNetoActual { get; set; }
        public double GramajeActual { get; set; }
        public double MetrosPorMinActual { get; set; }

        // ==================================================
        // ROSCAS – ACTUAL (A..E)
        // ==================================================
        // Rosca A
        public double RoscaA_GramaMetroActual { get; set; }
        public double RoscaA_EspesorActual { get; set; }
        public double RoscaA_PorcentajeActual { get; set; }
        public double RoscaA_KgHoraActual { get; set; }
        public double RoscaA_Silo1Actual { get; set; }
        public double RoscaA_Silo2Actual { get; set; }
        public double RoscaA_Silo3Actual { get; set; }
        public double RoscaA_Silo4Actual { get; set; }
        public double RoscaA_Silo5Actual { get; set; }
        public double RoscaA_Silo6Actual { get; set; }

        // Rosca B
        public double RoscaB_GramaMetroActual { get; set; }
        public double RoscaB_EspesorActual { get; set; }
        public double RoscaB_PorcentajeActual { get; set; }
        public double RoscaB_KgHoraActual { get; set; }
        public double RoscaB_Silo1Actual { get; set; }
        public double RoscaB_Silo2Actual { get; set; }
        public double RoscaB_Silo3Actual { get; set; }
        public double RoscaB_Silo4Actual { get; set; }
        public double RoscaB_Silo5Actual { get; set; }
        public double RoscaB_Silo6Actual { get; set; }

        // Rosca C
        public double RoscaC_GramaMetroActual { get; set; }
        public double RoscaC_EspesorActual { get; set; }
        public double RoscaC_PorcentajeActual { get; set; }
        public double RoscaC_KgHoraActual { get; set; }
        public double RoscaC_Silo1Actual { get; set; }
        public double RoscaC_Silo2Actual { get; set; }
        public double RoscaC_Silo3Actual { get; set; }
        public double RoscaC_Silo4Actual { get; set; }
        public double RoscaC_Silo5Actual { get; set; }
        public double RoscaC_Silo6Actual { get; set; }

        // Rosca D
        public double RoscaD_GramaMetroActual { get; set; }
        public double RoscaD_EspesorActual { get; set; }
        public double RoscaD_PorcentajeActual { get; set; }
        public double RoscaD_KgHoraActual { get; set; }
        public double RoscaD_Silo1Actual { get; set; }
        public double RoscaD_Silo2Actual { get; set; }
        public double RoscaD_Silo3Actual { get; set; }
        public double RoscaD_Silo4Actual { get; set; }
        public double RoscaD_Silo5Actual { get; set; }
        public double RoscaD_Silo6Actual { get; set; }

        // Rosca E
        public double RoscaE_GramaMetroActual { get; set; }
        public double RoscaE_EspesorActual { get; set; }
        public double RoscaE_PorcentajeActual { get; set; }
        public double RoscaE_KgHoraActual { get; set; }
        public double RoscaE_Silo1Actual { get; set; }
        public double RoscaE_Silo2Actual { get; set; }
        public double RoscaE_Silo3Actual { get; set; }
        public double RoscaE_Silo4Actual { get; set; }
        public double RoscaE_Silo5Actual { get; set; }
        public double RoscaE_Silo6Actual { get; set; }

        // ==================================================
        // ROSCAS – PROGRAMADO (A..E)
        // ==================================================
        // Rosca A programado
        public double RoscaA_GramaMetroProgramado { get; set; }
        public double RoscaA_EspesorProgramado { get; set; }
        public double RoscaA_PorcentajeProgramado { get; set; }
        public double RoscaA_KgHoraProgramado { get; set; }
        public double RoscaA_Silo1Programado { get; set; }
        public double RoscaA_Silo2Programado { get; set; }
        public double RoscaA_Silo3Programado { get; set; }
        public double RoscaA_Silo4Programado { get; set; }
        public double RoscaA_Silo5Programado { get; set; }
        public double RoscaA_Silo6Programado { get; set; }

        // Rosca B programado
        public double RoscaB_GramaMetroProgramado { get; set; }
        public double RoscaB_EspesorProgramado { get; set; }
        public double RoscaB_PorcentajeProgramado { get; set; }
        public double RoscaB_KgHoraProgramado { get; set; }
        public double RoscaB_Silo1Programado { get; set; }
        public double RoscaB_Silo2Programado { get; set; }
        public double RoscaB_Silo3Programado { get; set; }
        public double RoscaB_Silo4Programado { get; set; }
        public double RoscaB_Silo5Programado { get; set; }
        public double RoscaB_Silo6Programado { get; set; }

        // Rosca C programado
        public double RoscaC_GramaMetroProgramado { get; set; }
        public double RoscaC_EspesorProgramado { get; set; }
        public double RoscaC_PorcentajeProgramado { get; set; }
        public double RoscaC_KgHoraProgramado { get; set; }
        public double RoscaC_Silo1Programado { get; set; }
        public double RoscaC_Silo2Programado { get; set; }
        public double RoscaC_Silo3Programado { get; set; }
        public double RoscaC_Silo4Programado { get; set; }
        public double RoscaC_Silo5Programado { get; set; }
        public double RoscaC_Silo6Programado { get; set; }

        // Rosca D programado
        public double RoscaD_GramaMetroProgramado { get; set; }
        public double RoscaD_EspesorProgramado { get; set; }
        public double RoscaD_PorcentajeProgramado { get; set; }
        public double RoscaD_KgHoraProgramado { get; set; }
        public double RoscaD_Silo1Programado { get; set; }
        public double RoscaD_Silo2Programado { get; set; }
        public double RoscaD_Silo3Programado { get; set; }
        public double RoscaD_Silo4Programado { get; set; }
        public double RoscaD_Silo5Programado { get; set; }
        public double RoscaD_Silo6Programado { get; set; }

        // Rosca E programado
        public double RoscaE_GramaMetroProgramado { get; set; }
        public double RoscaE_EspesorProgramado { get; set; }
        public double RoscaE_PorcentajeProgramado { get; set; }
        public double RoscaE_KgHoraProgramado { get; set; }
        public double RoscaE_Silo1Programado { get; set; }
        public double RoscaE_Silo2Programado { get; set; }
        public double RoscaE_Silo3Programado { get; set; }
        public double RoscaE_Silo4Programado { get; set; }
        public double RoscaE_Silo5Programado { get; set; }
        public double RoscaE_Silo6Programado { get; set; }

        // ==================================================
        // ROSCAS – DENSIDADES (A..E)
        // ==================================================
        public double RoscaA_DensidadSilo1 { get; set; }
        public double RoscaA_DensidadSilo2 { get; set; }
        public double RoscaA_DensidadSilo3 { get; set; }
        public double RoscaA_DensidadSilo4 { get; set; }
        public double RoscaA_DensidadSilo5 { get; set; }
        public double RoscaA_DensidadSilo6 { get; set; }

        public double RoscaB_DensidadSilo1 { get; set; }
        public double RoscaB_DensidadSilo2 { get; set; }
        public double RoscaB_DensidadSilo3 { get; set; }
        public double RoscaB_DensidadSilo4 { get; set; }
        public double RoscaB_DensidadSilo5 { get; set; }
        public double RoscaB_DensidadSilo6 { get; set; }

        public double RoscaC_DensidadSilo1 { get; set; }
        public double RoscaC_DensidadSilo2 { get; set; }
        public double RoscaC_DensidadSilo3 { get; set; }
        public double RoscaC_DensidadSilo4 { get; set; }
        public double RoscaC_DensidadSilo5 { get; set; }
        public double RoscaC_DensidadSilo6 { get; set; }

        public double RoscaD_DensidadSilo1 { get; set; }
        public double RoscaD_DensidadSilo2 { get; set; }
        public double RoscaD_DensidadSilo3 { get; set; }
        public double RoscaD_DensidadSilo4 { get; set; }
        public double RoscaD_DensidadSilo5 { get; set; }
        public double RoscaD_DensidadSilo6 { get; set; }

        public double RoscaE_DensidadSilo1 { get; set; }
        public double RoscaE_DensidadSilo2 { get; set; }
        public double RoscaE_DensidadSilo3 { get; set; }
        public double RoscaE_DensidadSilo4 { get; set; }
        public double RoscaE_DensidadSilo5 { get; set; }
        public double RoscaE_DensidadSilo6 { get; set; }

        // ==================================================
        // ROSCAS – TOTALIZADORES (KG) (A..E)
        // ==================================================
        public double RoscaA_TotalSilo1 { get; set; }
        public double RoscaA_TotalSilo2 { get; set; }
        public double RoscaA_TotalSilo3 { get; set; }
        public double RoscaA_TotalSilo4 { get; set; }
        public double RoscaA_TotalSilo5 { get; set; }
        public double RoscaA_TotalSilo6 { get; set; }

        public double RoscaB_TotalSilo1 { get; set; }
        public double RoscaB_TotalSilo2 { get; set; }
        public double RoscaB_TotalSilo3 { get; set; }
        public double RoscaB_TotalSilo4 { get; set; }
        public double RoscaB_TotalSilo5 { get; set; }
        public double RoscaB_TotalSilo6 { get; set; }

        public double RoscaC_TotalSilo1 { get; set; }
        public double RoscaC_TotalSilo2 { get; set; }
        public double RoscaC_TotalSilo3 { get; set; }
        public double RoscaC_TotalSilo4 { get; set; }
        public double RoscaC_TotalSilo5 { get; set; }
        public double RoscaC_TotalSilo6 { get; set; }

        public double RoscaD_TotalSilo1 { get; set; }
        public double RoscaD_TotalSilo2 { get; set; }
        public double RoscaD_TotalSilo3 { get; set; }
        public double RoscaD_TotalSilo4 { get; set; }
        public double RoscaD_TotalSilo5 { get; set; }
        public double RoscaD_TotalSilo6 { get; set; }

        public double RoscaE_TotalSilo1 { get; set; }
        public double RoscaE_TotalSilo2 { get; set; }
        public double RoscaE_TotalSilo3 { get; set; }
        public double RoscaE_TotalSilo4 { get; set; }
        public double RoscaE_TotalSilo5 { get; set; }
        public double RoscaE_TotalSilo6 { get; set; }

        // ==================================================
        // CONSUMO / ENERGÍA
        // ==================================================
        public double TensionL1 { get; set; }
        public double TensionL2 { get; set; }
        public double TensionL3 { get; set; }
        public double AmperesL1 { get; set; }
        public double AmperesL2 { get; set; }
        public double AmperesL3 { get; set; }
        public double ConsumoActualKW { get; set; }
        public double KWTotal { get; set; }
        public double KWPorKg { get; set; }
        public double KWDia { get; set; }

        // ==================================================
        // OPERACIÓN / PRODUCCIÓN GENERAL (OP)
        // ==================================================
        public string? NumeroOP { get; set; }
        public int EstadoOP { get; set; }
        public double KgPorMetroOP { get; set; }
        public double TamanoBobinaOP { get; set; }
        public double RecortesOP { get; set; }

        public double KgProducidos { get; set; }
        public double MetrosProducidos { get; set; }
        public double ConsumoTotalOP { get; set; }

        // ==================================================
        // DATOS DE PEDIDO / MÁQUINA
        // ==================================================
        public string? MaquinaOcupada { get; set; }
        public string? NombreMaquina { get; set; }
        public string? Pedido { get; set; }
        public string? PedidoIniciado { get; set; }
        public double PorcentajeB { get; set; }
        public double PorcentajeC { get; set; }
        public string? PrevisionTerminar { get; set; }
        public string? NombreReceta { get; set; }
        public string? EstadoPedido { get; set; }
        public double TamanoBobina { get; set; }
        public string? TiempoTotal { get; set; }

        // ==================================================
        // MÉTODOS AUXILIARES
        // ==================================================
        public string GetDescripcionEstadoOP()
        {
            return EstadoOP switch
            {
                2 => "Produciendo",
                3 => "Finalizada",
                _ => "Desconocido"
            };
        }
    }
}