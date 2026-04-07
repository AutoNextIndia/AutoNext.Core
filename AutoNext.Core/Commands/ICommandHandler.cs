using AutoNext.Core.Abstractions;

namespace AutoNext.Core.Commands
{
    /// <summary>
    /// Command handler for commands with response
    /// </summary>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }

    /// <summary>
    /// Command handler for commands without response
    /// </summary>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand
    {
    }
}
