using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MqttService.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System;
using System.IO;
using System.Reflection;

namespace MqttWebAppTest
{
    public class Program
    {
        private static IConfigurationRoot config;

        public static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        public static MqttConfiguration Configuration { get; set; } = new();

        public static AssemblyName ServiceName => Assembly.GetExecutingAssembly().GetName();

        public static void Main(string[] args)
        {
            LoggerConfiguration();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Reading The Configuration");
                ReadConfiguration();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();

            }

        }

        private static void LoggerConfiguration()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .WriteTo.Console();

            if (EnvironmentName != "Development")
            {
                loggerConfiguration
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Orleans", LogEventLevel.Information)
                    .MinimumLevel.Information();
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static void ReadConfiguration()
        {

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true);

            if (!string.IsNullOrWhiteSpace(EnvironmentName))
            {
                var appsettingsFileName = $"appsettings.{EnvironmentName}.json";

                if (File.Exists(appsettingsFileName))
                {
                    configurationBuilder.AddJsonFile(appsettingsFileName, false, true);
                }
            }

            config = configurationBuilder.Build();
            config.Bind(ServiceName.Name, Configuration);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
