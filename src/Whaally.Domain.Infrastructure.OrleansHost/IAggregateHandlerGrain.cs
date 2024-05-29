using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public interface IAggregateHandlerGrain<TAggregate> : IGrainWithGuidKey,
    IAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{

}