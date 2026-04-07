using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AutoNext.Core.Services.HealthChecks
{
    public class DatabaseHealthCheck<TContext> : IHealthCheck where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<DatabaseHealthCheck<TContext>> _logger;

        public DatabaseHealthCheck(TContext context, ILogger<DatabaseHealthCheck<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                if (canConnect)
                {
                    // Optional: Run a simple query
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                    return HealthCheckResult.Healthy("Database connection is healthy");
                }

                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }
}
