using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ControlplastPLCService.Models;

namespace ControlplastPLCService.Services
{
    public interface IDatabaseService
    {
        Task<bool> ConectarAsync();
        Task DesconectarAsync();
        Task<bool> GuardarDatosProduccionAsync(int maquinaId, DatosProduccion datos, DatosProduccionConfig config);
        Task<bool> ActualizarDatosActualesAsync(int maquinaId, DatosProduccion datos, DatosProduccionConfig config);
        bool EstaConectada { get; }
    }
    
    public class SqlServerDatabaseService : IDatabaseService
    {
        private readonly DatabaseConfig _config;
        private readonly EncryptionService _encryption;
        private SqlConnection? _connection;
        private readonly object _lockObj = new object();
        
        public bool EstaConectada { get; private set; }
        
        public SqlServerDatabaseService(DatabaseConfig config, EncryptionService encryption)
        {
            _config = config;
            _encryption = encryption;
        }
        
        public async Task<bool> ConectarAsync()
        {
            try
            {
                var password = _config.UsarEncriptacion 
                    ? _encryption.Decrypt(_config.Password)
                    : _config.Password;
                
                var connectionString = $"Server={_config.Host},{_config.Puerto};" +
                                     $"Database={_config.Database};" +
                                     $"User Id={_config.Usuario};" +
                                     $"Password={password};" +
                                     $"Connection Timeout={_config.TimeoutSegundos};" +
                                     $"Encrypt=False;";
                
                _connection = new SqlConnection(connectionString);
                await _connection.OpenAsync();
                
                EstaConectada = true;
                Console.WriteLine($"✅ Conectado a base de datos: {_config.Host}/{_config.Database}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error conectando a BD: {ex.Message}");
                EstaConectada = false;
                return false;
            }
        }
        
        public async Task DesconectarAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
            EstaConectada = false;
        }
        
        public async Task<bool> GuardarDatosProduccionAsync(int maquinaId, DatosProduccion datos, DatosProduccionConfig config)
        {
            if (!EstaConectada || _connection == null)
                return false;
            
            try
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO DatosProduccionHistorico 
                    (MaquinaId, Timestamp, KgHoraProgramado, EspesorProgramado, 
                     AnchoBrutoProgramado, AnchoNetoProgramado, GramajeProgramado, 
                     MetrosPorMinProgramado, KgHoraActual, EspesorActual, AnchoBrutoActual, 
                     AnchoNetoActual, GramajeActual, MetrosPorMinActual, 
                     RoscaA_GramaMetroActual, RoscaA_EspesorActual, RoscaA_PorcentajeActual, RoscaA_KgHoraActual,
                     RoscaA_Silo1Actual, RoscaA_Silo2Actual, RoscaA_Silo3Actual, RoscaA_Silo4Actual, RoscaA_Silo5Actual, RoscaA_Silo6Actual,
                     RoscaA_TotalSilo1, RoscaA_TotalSilo2, RoscaA_TotalSilo3, 
                     RoscaA_TotalSilo4, RoscaA_TotalSilo5, RoscaA_TotalSilo6,
                     RoscaA_DensidadSilo1, RoscaA_DensidadSilo2, RoscaA_DensidadSilo3,
                     RoscaA_DensidadSilo4, RoscaA_DensidadSilo5, RoscaA_DensidadSilo6,
                     NumeroOP, EstadoOP, KgPorMetroOP, TamanoBobinaOP, RecortesOP,
                     KgProducidos, MetrosProducidos, ConsumoTotalOP,
                     ConsumoAmpere, ConsumoWatt, DatosJson)
                    VALUES 
                    (@MaquinaId, @Timestamp, @KgHoraProgramado, @EspesorProgramado,
                     @AnchoBrutoProgramado, @AnchoNetoProgramado, @GramajeProgramado,
                     @MetrosPorMinProgramado, @KgHoraActual, @EspesorActual, @AnchoBrutoActual,
                     @AnchoNetoActual, @GramajeActual, @MetrosPorMinActual,
                     @RoscaA_GramaMetroActual, @RoscaA_EspesorActual, @RoscaA_PorcentajeActual, @RoscaA_KgHoraActual,
                     @RoscaA_Silo1Actual, @RoscaA_Silo2Actual, @RoscaA_Silo3Actual, @RoscaA_Silo4Actual, @RoscaA_Silo5Actual, @RoscaA_Silo6Actual,
                     @RoscaA_TotalSilo1, @RoscaA_TotalSilo2, @RoscaA_TotalSilo3,
                     @RoscaA_TotalSilo4, @RoscaA_TotalSilo5, @RoscaA_TotalSilo6,
                     @RoscaA_DensidadSilo1, @RoscaA_DensidadSilo2, @RoscaA_DensidadSilo3,
                     @RoscaA_DensidadSilo4, @RoscaA_DensidadSilo5, @RoscaA_DensidadSilo6,
                     @NumeroOP, @EstadoOP, @KgPorMetroOP, @TamanoBobinaOP, @RecortesOP,
                     @KgProducidos, @MetrosProducidos, @ConsumoTotalOP,
                     @ConsumoAmpere, @ConsumoWatt, @DatosJson)"; 
                
                cmd.Parameters.AddWithValue("@MaquinaId", maquinaId);
                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                
                // Producción Programada
                cmd.Parameters.AddWithValue("@KgHoraProgramado", 
                    config.GuardarKghProgramado ? datos.KgHoraProgramado : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EspesorProgramado", 
                    config.GuardarEspessuraProgramada ? datos.EspesorProgramado : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AnchoBrutoProgramado", 
                    config.GuardarLarguraBrutaProgramada ? datos.AnchoBrutoProgramado : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AnchoNetoProgramado", 
                    config.GuardarLarguraLiquidaProgramada ? datos.AnchoNetoProgramado : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GramajeProgramado", 
                    config.GuardarGramaturaProgramada ? datos.GramajeProgramado : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MetrosPorMinProgramado", 
                    config.GuardarVelocidadeProgramada ? datos.MetrosPorMinProgramado : (object)DBNull.Value);
                
                // Producción Actual
                cmd.Parameters.AddWithValue("@KgHoraActual", 
                    config.GuardarKghAtual ? datos.KgHoraActual : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EspesorActual", 
                    config.GuardarEspessuraAtual ? datos.EspesorActual : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AnchoBrutoActual", 
                    config.GuardarLarguraBrutaAtual ? datos.AnchoBrutoActual : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AnchoNetoActual", 
                    config.GuardarLarguraLiquidaAtual ? datos.AnchoNetoActual : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GramajeActual", 
                    config.GuardarGramaturaAtual ? datos.GramajeActual : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MetrosPorMinActual", 
                    config.GuardarVelocidadeAtual ? datos.MetrosPorMinActual : (object)DBNull.Value);
                
                // Rosca A
                cmd.Parameters.AddWithValue("@RoscaA_GramaMetroActual", 
                    config.GuardarRoscaAGramaMetro ? datos.RoscaA_GramaMetroActual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_EspesorActual", 
                    config.GuardarRoscaAEspessura ? datos.RoscaA_EspesorActual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_PorcentajeActual", 
                    config.GuardarRoscaAPercentual ? datos.RoscaA_PorcentajeActual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_KgHoraActual", 
                    config.GuardarRoscaAKgh ? datos.RoscaA_KgHoraActual : DBNull.Value);
                
                // Silos
                cmd.Parameters.AddWithValue("@RoscaA_Silo1Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo1Actual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_Silo2Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo2Actual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_Silo3Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo3Actual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_Silo4Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo4Actual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_Silo5Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo5Actual : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_Silo6Actual", 
                    config.GuardarRoscaASilos ? datos.RoscaA_Silo6Actual : DBNull.Value);
                
                // Totalizadores
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo1", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo1 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo2", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo2 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo3", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo3 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo4", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo4 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo5", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo5 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_TotalSilo6", 
                    config.GuardarRoscaATotalizadores ? datos.RoscaA_TotalSilo6 : DBNull.Value);
                
                // Densidades
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo1", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo1 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo2", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo2 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo3", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo3 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo4", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo4 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo5", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo5 : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoscaA_DensidadSilo6", 
                    config.GuardarRoscaADensidades ? datos.RoscaA_DensidadSilo6 : DBNull.Value);
                
                // OP / Producción
                cmd.Parameters.AddWithValue("@NumeroOP", 
                    config.GuardarOpNumero ? datos.NumeroOP ?? "N/A" : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EstadoOP", 
                    config.GuardarOpStatus ? datos.EstadoOP : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KgPorMetroOP", datos.KgPorMetroOP);
                cmd.Parameters.AddWithValue("@TamanoBobinaOP", datos.TamanoBobinaOP);
                cmd.Parameters.AddWithValue("@RecortesOP", datos.RecortesOP);
                cmd.Parameters.AddWithValue("@KgProducidos", 
                    config.GuardarKgProduzidos ? datos.KgProducidos : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MetrosProduzidos", 
                    config.GuardarMetrosProduzidos ? datos.MetrosProducidos : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ConsumoTotalOP", 
                    config.GuardarConsumoTotalOp ? datos.ConsumoTotalOP : (object)DBNull.Value);
                
                // Consumo
                cmd.Parameters.AddWithValue("@ConsumoAmpere", 
                    config.GuardarConsumoAmpere ? (object)(datos.AmperesL1 + datos.AmperesL2 + datos.AmperesL3) : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ConsumoWatt", 
                    config.GuardarConsumoWatt ? (object)datos.ConsumoActualKW : (object)DBNull.Value);
                
                // JSON completo
                cmd.Parameters.AddWithValue("@DatosJson", 
                    System.Text.Json.JsonSerializer.Serialize(datos));
                
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando en BD: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ActualizarDatosActualesAsync(int maquinaId, DatosProduccion datos, DatosProduccionConfig config)
        {
            if (!EstaConectada || _connection == null)
                return false;
            
            try
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = @"
                    MERGE DatosProduccionActual AS target
                    USING (SELECT @MaquinaId AS MaquinaId) AS source
                    ON target.MaquinaId = source.MaquinaId
                    WHEN MATCHED THEN
                        UPDATE SET 
                            UltimaActualizacion = @Timestamp,
                            KgHoraActual = @KgHoraActual,
                            MetrosPorMinActual = @MetrosPorMinActual,
                            EspesorActual = @EspesorActual,
                            GramajeActual = @GramajeActual,
                            NumeroOP = @NumeroOP,
                            EstadoOP = @EstadoOP,
                            KgProducidos = @KgProducidos,
                            ConsumoActualKW = @ConsumoActualKW,
                            DatosJson = @DatosJson
                    WHEN NOT MATCHED THEN
                        INSERT (MaquinaId, UltimaActualizacion, KgHoraActual, MetrosPorMinActual, 
                                EspesorActual, GramajeActual, NumeroOP, EstadoOP, 
                                KgProducidos, ConsumoActualKW, DatosJson)
                        VALUES (@MaquinaId, @Timestamp, @KgHoraActual, @MetrosPorMinActual,
                                @EspesorActual, @GramajeActual, @NumeroOP, @EstadoOP,
                                @KgProducidos, @ConsumoActualKW, @DatosJson);";
                
                cmd.Parameters.AddWithValue("@MaquinaId", maquinaId);
                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@KgHoraActual", datos.KgHoraActual);
                cmd.Parameters.AddWithValue("@MetrosPorMinActual", datos.MetrosPorMinActual);
                cmd.Parameters.AddWithValue("@EspesorActual", datos.EspesorActual);
                cmd.Parameters.AddWithValue("@GramajeActual", datos.GramajeActual);
                cmd.Parameters.AddWithValue("@NumeroOP", datos.NumeroOP ?? "N/A");
                cmd.Parameters.AddWithValue("@EstadoOP", datos.EstadoOP);
                cmd.Parameters.AddWithValue("@KgProducidos", datos.KgProducidos);
                cmd.Parameters.AddWithValue("@ConsumoActualKW", datos.ConsumoActualKW);
                cmd.Parameters.AddWithValue("@DatosJson", 
                    System.Text.Json.JsonSerializer.Serialize(datos));
                
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error actualizando datos actuales: {ex.Message}");
                return false;
            }
        }
    }
}