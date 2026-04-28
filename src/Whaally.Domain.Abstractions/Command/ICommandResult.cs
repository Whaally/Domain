namespace Whaally.Domain.Abstractions;

public interface ICommandResult : IResult
{
    public IEnumerable<IMessageEnvelope> Operations { get; } 
    
    // todo: consider renaming this evaluate
    public ICommandResult Invoke(ICommand command);
    public ICommandResult Stage(IEvent @event);

    public ICommandResult WithError(string error, params string[] fields);
}
