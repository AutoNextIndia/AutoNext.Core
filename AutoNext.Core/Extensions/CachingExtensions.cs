using AutoNext.Core.Abstractions;
using AutoNext.Core.Services.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;

namespace AutoNext.Core.Extensions
{
    public static class CachingExtensions
    {
        /// <summary>
        /// Adds in-memory caching (development only)
        /// </summary>
        public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return services;
        }

        /// <summary>
        /// Adds Redis caching with validation and health checks
        /// </summary>
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetRedisConnectionString(configuration);
            ValidateRedisConnection(connectionString);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = "AutoNext:";
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddHealthChecks()
                .AddRedis(connectionString, name: "redis", failureStatus: HealthStatus.Unhealthy, tags: new[] { "cache", "redis" });

            return services;
        }

        /// <summary>
        /// Adds Redis cache with direct connection string
        /// </summary>
        public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            ValidateRedisConnection(connectionString);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = "AutoNext:";
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddHealthChecks()
                .AddRedis(connectionString, name: "redis", failureStatus: HealthStatus.Unhealthy, tags: new[] { "cache", "redis" });

            return services;
        }

        private static string GetRedisConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("Redis")
                ?? configuration["Redis:ConnectionString"]
                ?? configuration["ConnectionStrings:Redis"]
                ?? throw new InvalidOperationException("Redis connection string not found in configuration");
        }

        private static void ValidateRedisConnection(string connectionString)
        {
            try
            {
                using var redis = ConnectionMultiplexer.Connect(connectionString);
                redis.GetDatabase().Ping();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot connect to Redis: {ex.Message}", ex);
            }
        }
    }
}