using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ICommandResult : IResultBase
{
    public IEnumerable<IMessageEnvelope> Operations { get; } 
    
    // todo: consider renaming this evaluate
    public ICommandResult Invoke(ICommand command);
    public ICommandResult Stage(IEvent @event);
}
