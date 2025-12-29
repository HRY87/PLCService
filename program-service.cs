using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ControlplastPLCService.Services;
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
            // Configurar Serilog
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
                Log.Information("Iniciando Servicio de Windows - ControlplastPLC");
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
                    var configPath = Path.Combine(AppContext.BaseDirectory, "appsetting.json");
                    
                    if (!File.Exists(configPath))
                    {
                        throw new FileNotFoundException($"No se encontró el archivo de configuración: {configPath}");
                    }
                    
                    var configJson = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<ConfiguracionSistema>(configJson, 
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
                    
                    // Registrar servicios
                    services.AddSingleton(config);
                    
                    // Servicio de encriptación
                    services.AddSingleton<EncryptionService>(sp => 
                        new EncryptionService("ControlplastMasterKey2024!"));
                    
                    // Base de datos local
                    services.AddSingleton<IDatabaseService>(sp =>
                    {
                        var encryption = sp.GetRequiredService<EncryptionService>();
                        return new SqlServerDatabaseService(config.DatabaseLocal, encryption);
                    });
                    
                    // Base de datos nube (opcional)
                    if (config.DatabaseNube != null && !string.IsNullOrEmpty(config.DatabaseNube.Host))
                    {
                        services.AddSingleton<IDatabaseService>(sp =>
                        {
                            var encryption = sp.GetRequiredService<EncryptionService>();
                            return new SqlServerDatabaseService(config.DatabaseNube, encryption);
                        });
                    }
                    
                    // Manager de máquinas
                    services.AddSingleton<MaquinaManagerService>();
                    
                    // Registrar Worker (el servicio principal)
                    services.AddHostedService<PLCMonitorWorker>();
                })
                .UseSerilog();
    }
}