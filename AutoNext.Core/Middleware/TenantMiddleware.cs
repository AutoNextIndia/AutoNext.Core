using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AutoNext.Core.Middleware
{
    /// <summary>
    /// Extracts and sets tenant context from request
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? tenantId = null;

            // Try to get tenant from header
            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenantId))
            {
                tenantId = headerTenantId.ToString();
            }
            // Try from subdomain
            else if (context.Request.Host.Host?.Contains('.') == true)
            {
                var subdomain = context.Request.Host.Host.Split('.')[0];
                if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
                {
                    tenantId = subdomain;
                }
            }

            if (!string.IsNullOrEmpty(tenantId))
            {
                context.Items["TenantId"] = tenantId;
                using (Serilog.Context.LogContext.PushProperty("TenantId", tenantId))
                {
                    _logger.LogDebug("Tenant context set: {TenantId}", tenantId);
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }

}
