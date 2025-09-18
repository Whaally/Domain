using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IServiceResult : IResultBase
{
    public IEnumerable<IMessageEnvelope> Operations { get; }
    
    public IServiceResult Invoke(IService service);
    public IServiceResult Stage(Guid aggregateId, params ICommand[] command);
}
