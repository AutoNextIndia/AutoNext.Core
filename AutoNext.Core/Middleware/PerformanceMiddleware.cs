using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AutoNext.Core.Middleware
{
    /// <summary>
    /// Monitors and logs slow requests
    /// </summary>
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private readonly int _thresholdMs;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger, int thresholdMs = 500)
        {
            _next = next;
            _logger = logger;
            _thresholdMs = thresholdMs;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > _thresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {Elapsed}ms (threshold: {Threshold}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    _thresholdMs);
            }
        }
    }
}
