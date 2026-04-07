using AutoNext.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoNext.Core.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Add all AutoNext middlewares
        /// </summary>
        public static IApplicationBuilder UseAutoNextMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<PerformanceMiddleware>();
            app.UseMiddleware<TenantMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            return app;
        }

        /// <summary>
        /// Add exception handling middleware only
        /// </summary>
        public static IApplicationBuilder UseAutoNextExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }

        /// <summary>
        /// Add request logging middleware
        /// </summary>
        public static IApplicationBuilder UseAutoNextRequestLogging(this IApplicationBuilder app,
            bool logRequestBody = false,
            bool logResponseBody = false,
            int maxBodyLength = 1000)
        {
            app.UseMiddleware<RequestLoggingMiddleware>(logRequestBody, logResponseBody, maxBodyLength);
            return app;
        }

        /// <summary>
        /// Add performance monitoring middleware
        /// </summary>
        public static IApplicationBuilder UseAutoNextPerformanceMonitoring(this IApplicationBuilder app, int thresholdMs = 500)
        {
            app.UseMiddleware<PerformanceMiddleware>(thresholdMs);
            return app;
        }

        /// <summary>
        /// Add correlation ID middleware
        /// </summary>
        public static IApplicationBuilder UseAutoNextCorrelationId(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }

        /// <summary>
        /// Add tenant middleware
        /// </summary>
        public static IApplicationBuilder UseAutoNextTenant(this IApplicationBuilder app)
        {
            app.UseMiddleware<TenantMiddleware>();
            return app;
        }
    }
}
