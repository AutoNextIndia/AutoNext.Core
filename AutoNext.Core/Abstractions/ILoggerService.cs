namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Generic logging abstraction
    /// </summary>
    public interface ILoggerService<T>
    {
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
        void LogWithContext(string level, string message, Dictionary<string, object> context);
    }

    /// <summary>
    /// Non-generic logging abstraction
    /// </summary>
    public interface ILoggerService
    {
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
        IDisposable BeginScope<TState>(TState state);
    }
}
