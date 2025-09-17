using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Saga : Result<Saga>, ISagaResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => _operations.AsReadOnly();

    public ISagaResult Invoke(IService service)
    {
        _operations.Add(
            new ServiceEnvelope(
                new ServiceMetadata(),
                service));
        
        return this;
    }

    public ISagaResult Stage(string aggregateId, params ICommand[] command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata()
                {
                    AggregateId = aggregateId
                }
                , command));
        
        return this;
    }
}
