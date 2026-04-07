using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AutoNext.Core.Middleware
{
    /// <summary>
    /// Logs request and response details
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly bool _logRequestBody;
        private readonly bool _logResponseBody;
        private readonly int _maxBodyLength;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger,
            bool logRequestBody = false,
            bool logResponseBody = false,
            int maxBodyLength = 1000)
        {
            _next = next;
            _logger = logger;
            _logRequestBody = logRequestBody;
            _logResponseBody = logResponseBody;
            _maxBodyLength = maxBodyLength;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // Log request
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var requestBody = _logRequestBody ? await ReadRequestBody(request) : "";
                _logger.LogInformation("HTTP {Method} {Path} started - Query: {QueryString} - Body: {Body}",
                    request.Method,
                    request.Path,
                    request.QueryString.ToString(),
                    Truncate(requestBody));
            }

            // Capture response body if needed
            Stream? originalBodyStream = null;
            MemoryStream? responseBodyStream = null;

            if (_logResponseBody)
            {
                originalBodyStream = context.Response.Body;
                responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;
            }

            try
            {
                await _next(context);

                stopwatch.Stop();

                // Log response
                var responseBody = _logResponseBody ? await ReadResponseBody(context.Response) : "";
                _logger.LogInformation(
                    "HTTP {Method} {Path} completed - StatusCode: {StatusCode} - Duration: {Duration}ms - Response: {Body}",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    Truncate(responseBody));

                // Copy response back
                if (_logResponseBody && responseBodyStream != null && originalBodyStream != null)
                {
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                    context.Response.Body = originalBodyStream;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "HTTP {Method} {Path} failed after {Duration}ms",
                    request.Method, request.Path, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private string Truncate(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Length > _maxBodyLength ? text[.._maxBodyLength] + "..." : text;
        }
    }
}
