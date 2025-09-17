using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Service : Result<Service>, IServiceResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => _operations.AsReadOnly();

    public IServiceResult Invoke(IService service)
    {
        _operations.Add(
            new ServiceEnvelope(
                new ServiceMetadata(), 
                service));
        
        return this;
    }

    public IServiceResult Stage(string aggregateId, params ICommand[] command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata()
                {
                    AggregateId = aggregateId
                    // todo: add aggregate type for verbosity
                }, 
                command));
        
        return this;
    }
}
