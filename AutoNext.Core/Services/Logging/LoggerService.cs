using AutoNext.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoNext.Core.Services.Logging
{
    /// <summary>
    /// Generic logger service wrapper
    /// </summary>
    public class LoggerService<T> : ILoggerService<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerService(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args)
            => _logger.LogDebug(message, args);

        public void LogInformation(string message, params object[] args)
            => _logger.LogInformation(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.LogWarning(message, args);

        public void LogError(Exception exception, string message, params object[] args)
            => _logger.LogError(exception, message, args);

        public void LogError(string message, params object[] args)
            => _logger.LogError(message, args);

        public void LogCritical(Exception exception, string message, params object[] args)
            => _logger.LogCritical(exception, message, args);

        public void LogWithContext(string level, string message, Dictionary<string, object> context)
        {
            using var scope = _logger.BeginScope(context);

            switch (level.ToLower())
            {
                case "debug":
                    _logger.LogDebug(message);
                    break;
                case "warning":
                    _logger.LogWarning(message);
                    break;
                case "error":
                    _logger.LogError(message);
                    break;
                default:
                    _logger.LogInformation(message);
                    break;
            }
        }
    }


    /// <summary>
    /// Non-generic logger service implementation
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args)
            => _logger.LogDebug(message, args);

        public void LogInformation(string message, params object[] args)
            => _logger.LogInformation(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.LogWarning(message, args);

        public void LogError(Exception exception, string message, params object[] args)
            => _logger.LogError(exception, message, args);

        public void LogError(string message, params object[] args)
            => _logger.LogError(message, args);

        public void LogCritical(Exception exception, string message, params object[] args)
            => _logger.LogCritical(exception, message, args);

        public IDisposable BeginScope<TState>(TState state)
            => _logger.BeginScope(state);
    }
}
