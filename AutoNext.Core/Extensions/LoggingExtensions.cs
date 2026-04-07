using AutoNext.Core.Abstractions;
using AutoNext.Core.Services.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace AutoNext.Core.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Add Serilog logging with configuration
        /// </summary>
        public static IHostBuilder UseAutoNextSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            {
                // Read from appsettings.json [Serilog] section and registered services
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

                // Console output
                loggerConfiguration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}");

                // File output (daily rolling)
                loggerConfiguration.WriteTo.File(
                    path: "logs/autonext-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}");

                // JSON structured log file (for log aggregation tools e.g. Seq, ELK)
                if (context.Configuration.GetValue<bool>("Serilog:JsonFormat", false))
                {
                    loggerConfiguration.WriteTo.File(
                        formatter: new JsonFormatter(),           // requires: using Serilog.Formatting.Json
                        path: "logs/autonext-structured-.json",
                        rollingInterval: RollingInterval.Day);
                }

                // Minimum log level — falls back to Information if missing or invalid
                var minLevelRaw = context.Configuration.GetValue<string>("Serilog:MinimumLevel:Default", "Information")!;
                if (Enum.TryParse<LogEventLevel>(minLevelRaw, ignoreCase: true, out var minLevel))
                {
                    loggerConfiguration.MinimumLevel.Is(minLevel);
                }

                // Suppress noisy framework namespaces
                loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
                loggerConfiguration.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("System", LogEventLevel.Warning);
            });
        }

        /// <summary>
        /// Add logger services to DI
        /// </summary>
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
            services.AddScoped<ILoggerService, LoggerService>();
            return services;
        }
    }
}