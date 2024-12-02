using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public interface IAggregateHandlerGrain<TAggregate> : IGrainWithGuidKey,
    IAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{

}