using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ControlplastPLCService.Models;
using System;
using System.IO;
using System.Text.Json;

namespace ControlplastPLCService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configurar Serilog para logs en consola y archivo
            var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "service-.log");
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            try
            {
                Log.Information("==============================================");
                Log.Information("Iniciando Servicio de Monitoreo PLC");
                Log.Information("==============================================");
                
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error crítico al iniciar el servicio");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "ControlplastPLC";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Cargar configuración
                    var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    
                    if (!File.Exists(configPath))
                    {
                        throw new FileNotFoundException($"No se encontró el archivo de configuración: {configPath}");
                    }
                    
                    var configJson = File.ReadAllText(configPath);
                    
                    // Primero parseamos el JSON completo
                    var jsonDocument = JsonDocument.Parse(configJson);
                    
                    // Extraemos el nodo "ConfiguracionSistema"
                    if (!jsonDocument.RootElement.TryGetProperty("ConfiguracionSistema", out JsonElement configElement))
                    {
                        throw new InvalidOperationException("No se encontró la sección 'ConfiguracionSistema' en appsettings.json");
                    }
                    
                    // Deserializamos desde ese nodo
                    var config = JsonSerializer.Deserialize<ConfiguracionSistema>(
                        configElement.GetRawText(), 
                        new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                            AllowTrailingCommas = true
                        });
                    
                    if (config == null)
                    {
                        throw new InvalidOperationException("No se pudo cargar la configuración");
                    }
                    
                    // Log para debug
                    Log.Information("Configuración cargada: {MaquinasCount} máquina(s) definidas", 
                        config.Maquinas?.Count ?? 0);
                    
                    if (config.Maquinas != null)
                    {
                        foreach (var maq in config.Maquinas)
                        {
                            Log.Information("  - {Nombre} ({IP}:{Puerto}) - Habilitada: {Habilitada}",
                                maq.Nombre, 
                                maq.Configuracion?.Ip ?? "N/A", 
                                maq.Configuracion?.Puerto ?? 0,
                                maq.Habilitada);
                        }
                    }
                    
                    // Registrar configuración como singleton
                    services.AddSingleton(config);
                    
                    // Registrar Worker (servicio principal de monitoreo)
                    services.AddHostedService<PLCMonitorWorker>();
                })
                .UseSerilog();
    }
}