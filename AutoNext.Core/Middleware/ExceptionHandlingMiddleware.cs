using AutoNext.Core.Exceptions;
using AutoNext.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoNext.Core.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = validationEx.StatusCode;
                    errorResponse.Code = validationEx.Code;
                    errorResponse.Message = validationEx.Message;
                    errorResponse.Errors = validationEx.Errors;
                    _logger.LogWarning(exception, "Validation error: {Message}", exception.Message);
                    break;

                case NotFoundException notFoundEx:
                    response.StatusCode = notFoundEx.StatusCode;
                    errorResponse.Code = notFoundEx.Code;
                    errorResponse.Message = notFoundEx.Message;
                    errorResponse.Details = $"Entity: {notFoundEx.EntityType}, Id: {notFoundEx.EntityId}";
                    _logger.LogInformation(exception, "Resource not found: {Message}", exception.Message);
                    break;

                case UnauthorizedException unauthorizedEx:
                    response.StatusCode = unauthorizedEx.StatusCode;
                    errorResponse.Code = unauthorizedEx.Code;
                    errorResponse.Message = unauthorizedEx.Message;
                    _logger.LogWarning(exception, "Unauthorized access attempt");
                    break;

                case ForbiddenException forbiddenEx:
                    response.StatusCode = forbiddenEx.StatusCode;
                    errorResponse.Code = forbiddenEx.Code;
                    errorResponse.Message = forbiddenEx.Message;
                    _logger.LogWarning(exception, "Forbidden access: {Message}", exception.Message);
                    break;

                case ConflictException conflictEx:
                    response.StatusCode = conflictEx.StatusCode;
                    errorResponse.Code = conflictEx.Code;
                    errorResponse.Message = conflictEx.Message;
                    _logger.LogWarning(exception, "Conflict: {Message}", exception.Message);
                    break;

                case BusinessRuleException businessEx:
                    response.StatusCode = businessEx.StatusCode;
                    errorResponse.Code = businessEx.Code;
                    errorResponse.Message = businessEx.Message;
                    errorResponse.Details = $"Rule: {businessEx.RuleName}";
                    _logger.LogWarning(exception, "Business rule violation: {Message}", exception.Message);
                    break;

                case FluentValidation.ValidationException fluentEx:
                    response.StatusCode = 400;
                    errorResponse.Code = "VALIDATION_ERROR";
                    errorResponse.Message = "One or more validation errors occurred";
                    errorResponse.Errors = fluentEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    _logger.LogWarning(exception, "Fluent validation error");
                    break;

                default:
                    response.StatusCode = 500;
                    errorResponse.Code = "INTERNAL_SERVER_ERROR";
                    errorResponse.Message = "An unexpected error occurred. Please try again later.";
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await response.WriteAsync(json);
        }
    }

   
}
