using AutoNext.Core.Configurations;
using AutoNext.Core.Services.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AutoNext.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all AutoNext core services
        /// </summary>
        public static IServiceCollection AddAutoNextCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add configuration
            services.Configure<AppSettings>(configuration);
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<CacheSettings>(configuration.GetSection("Cache"));
            services.Configure<CorsSettings>(configuration.GetSection("Cors"));

            // Add logging
            services.AddLoggingServices();

            // Add HTTP context accessor
            services.AddHttpContextAccessor();

            // Add memory cache (default)
            services.AddInMemoryCache();

            // Add health checks
            services.AddHealthChecks();

            return services;
        }

        /// <summary>
        /// Add health checks with all configured services
        /// </summary>
        public static IServiceCollection AddAutoNextHealthChecks<TContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TContext : DbContext
        {
            var settings = configuration.GetSection("HealthChecks").Get<HealthCheckSettings>();

            var healthChecks = services.AddHealthChecks();

            if (settings?.EnableDatabaseCheck == true)
            {
                healthChecks.AddCheck<DatabaseHealthCheck<TContext>>(
                    "database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgresql" });
            }

            if (settings?.EnableRabbitMQCheck == true)
            {
                healthChecks.AddCheck<RabbitMqHealthCheck>(
                    "rabbitmq",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "messaging", "rabbitmq" });
            }

            return services;
        }

        /// <summary>
        /// Add CORS configuration
        /// </summary>
        public static IServiceCollection AddAutoNextCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection("Cors").Get<CorsSettings>();

            if (corsSettings?.AllowedOrigins?.Any() == true)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AutoNextCors", policy =>
                    {
                        policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
                              .WithMethods(corsSettings.AllowedMethods.ToArray())
                              .WithHeaders(corsSettings.AllowedHeaders.ToArray());

                        if (corsSettings.AllowCredentials)
                        {
                            policy.AllowCredentials();
                        }
                    });
                });
            }

            return services;
        }
    }
}
