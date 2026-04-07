using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace AutoNext.Core.Middleware
{
    /// <summary>
    /// Adds correlation ID to each request for tracing
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get or create correlation ID
            string correlationId;

            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerCorrelationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            else
            {
                correlationId = headerCorrelationId.ToString();
            }

            context.Request.Headers[CorrelationIdHeader] = correlationId;
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // Add to logging context
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            using (Serilog.Context.LogContext.PushProperty("RequestPath", context.Request.Path))
            using (Serilog.Context.LogContext.PushProperty("RequestMethod", context.Request.Method))
            {
                _logger.LogDebug("Request started: {Method} {Path} with CorrelationId: {CorrelationId}",
                    context.Request.Method, context.Request.Path, correlationId);

                await _next(context);

                _logger.LogDebug("Request completed: {Method} {Path} with StatusCode: {StatusCode}",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
        }
    }
}
