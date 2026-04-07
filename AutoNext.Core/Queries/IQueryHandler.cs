using AutoNext.Core.Abstractions;


namespace AutoNext.Core.Queries
{
    /// <summary>
    /// Query handler for read operations
    /// </summary>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
}
