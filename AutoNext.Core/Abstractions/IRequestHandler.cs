namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Handler for requests that return a response
    /// </summary>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Handler for requests without response
    /// </summary>
    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }
}
