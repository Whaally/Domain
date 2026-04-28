namespace Whaally.Domain.Abstractions;

public interface ISagaResult : IResult
{
    public IEnumerable<IMessageEnvelope> Operations { get; }
    
    public ISagaResult Invoke(IService service);
    public ISagaResult Stage(Guid aggregateId, params ICommand[] command);
}
