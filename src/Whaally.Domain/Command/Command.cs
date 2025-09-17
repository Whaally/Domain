using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Command : Result<Command>, ICommandResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => this._operations;

    public ICommandResult Invoke(ICommand command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata(), 
                command));
        
        return this;
    }

    public ICommandResult Stage(IEvent @event)
    {
        _operations.Add(
            new EventEnvelope(
                new EventMetadata(), 
                @event));
        
        return this;
    }
}
