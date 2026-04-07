using AutoNext.Core.Extensions;

namespace AutoNext.Core.Configurations
{
    public class AppSettings
    {
        public DatabaseSettings Database { get; set; } = new();
        public RabbitMqSettings RabbitMQ { get; set; } = new();
        public JwtSettings Jwt { get; set; } = new();
        public CacheSettings Cache { get; set; } = new();
        public SerilogSettings Serilog { get; set; } = new();
        public CorsSettings Cors { get; set; } = new();
        public HealthCheckSettings HealthChecks { get; set; } = new();
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public int MaxRetryCount { get; set; } = 5;
        public bool EnableSensitiveDataLogging { get; set; } = false;
        public bool EnableDetailedErrors { get; set; } = false;
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenLifetimeMinutes { get; set; } = 15;
        public int RefreshTokenLifetimeDays { get; set; } = 7;
    }

    public class CacheSettings
    {
        public string Provider { get; set; } = "Memory"; // Memory, Redis
        public string? RedisConnectionString { get; set; }
        public int DefaultSlidingExpirationSeconds { get; set; } = 600;
        public int DefaultAbsoluteExpirationSeconds { get; set; } = 3600;
    }

    public class CorsSettings
    {
        public List<string> AllowedOrigins { get; set; } = new();
        public List<string> AllowedMethods { get; set; } = new() { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        public List<string> AllowedHeaders { get; set; } = new() { "Content-Type", "Authorization", "X-Correlation-Id" };
        public bool AllowCredentials { get; set; } = true;
    }

    public class HealthCheckSettings
    {
        public bool EnableDatabaseCheck { get; set; } = true;
        public bool EnableRabbitMQCheck { get; set; } = false;
        public bool EnableRedisCheck { get; set; } = false;
        public string HealthEndpoint { get; set; } = "/health";
        public string ReadinessEndpoint { get; set; } = "/ready";
        public string LivenessEndpoint { get; set; } = "/live";
    }
}
