using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public class OrleansAggregateHandlerFactory(IClusterClient clusterClient) : IAggregateHandlerFactory
{
    // Bug: Implementing the interface like this does not work!
    // IAggregateHandler<TAggregate> IAggregateHandlerFactory.Instantiate<TAggregate>(string id)
    
    public Task<IAggregateHandler<TAggregate>> Instantiate<TAggregate>(Guid id) where TAggregate : class, IAggregate
        => Task.FromResult<IAggregateHandler<TAggregate>>(clusterClient.GetGrain<IAggregateHandlerGrain<TAggregate>>(id));
}
