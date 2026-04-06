namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Message consumer interface
    /// </summary>
    public interface IMessageConsumer<T> where T : class
    {
        Task HandleAsync(T message, CancellationToken cancellationToken = default);
    }
}
