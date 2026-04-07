using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AutoNext.Core.Services.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqHealthCheck> _logger;

        public RabbitMqHealthCheck(IConnection connection, ILogger<RabbitMqHealthCheck> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection.IsOpen)
                {
                    return await Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
                }

                return await Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is closed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ health check failed");
                return await Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ health check failed", ex));
            }
        }
    }
}
