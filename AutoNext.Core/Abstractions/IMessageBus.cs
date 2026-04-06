namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Message bus interface for event publishing
    /// </summary>
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
        Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class;
        Task PublishDelayedAsync<T>(T message, TimeSpan delay, CancellationToken cancellationToken = default) where T : class;
    }
}
