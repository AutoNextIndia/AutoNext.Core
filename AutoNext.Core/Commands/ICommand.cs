using AutoNext.Core.Abstractions;

namespace AutoNext.Core.Commands
{
    /// <summary>
    /// Command marker interface (CQRS)
    /// </summary>
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }

    /// <summary>
    /// Command without response
    /// </summary>
    public interface ICommand : IRequest<Unit>
    {
    }

    /// <summary>
    /// Command that returns Result pattern
    /// </summary>
    public interface ICommandResult<TResponse> : IRequest<AutoNext.Core.Models.Result<TResponse>>
    {
    }
   
}
