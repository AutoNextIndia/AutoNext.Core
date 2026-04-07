namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Notification marker interface
    /// </summary>
    public interface INotification
    {
    }

    /// <summary>
    /// Notification handler interface
    /// </summary>
    /// <typeparam name="TNotification">Notification type</typeparam>
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles the notification
        /// </summary>
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Multiple notification handler interface
    /// </summary>
    public interface INotificationHandler<in TNotification, TResponse>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles the notification and returns a response
        /// </summary>
        Task<TResponse> Handle(TNotification notification, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Core Mediator pattern abstraction for CQRS
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a request and returns response
        /// </summary>
        Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends dynamic request
        /// </summary>
        Task<object?> Send(
            object request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a typed notification to multiple handlers
        /// </summary>
        Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification;

        /// <summary>
        /// Publishes dynamic notification
        /// </summary>
        Task Publish(
            object notification,
            CancellationToken cancellationToken = default);
    }
}