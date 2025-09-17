using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ISagaResult : IResultBase
{
    public IEnumerable<IMessageEnvelope> Operations { get; }
    
    public ISagaResult Invoke(IService service);
    public ISagaResult Stage(string aggregateId, params ICommand[] command);
}
