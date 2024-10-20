using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public class OrleansAggregateHandlerFactory(IClusterClient clusterClient) : IAggregateHandlerFactory
{
    // Bug: Implementing the interface like this does not work!
    // IAggregateHandler<TAggregate> IAggregateHandlerFactory.Instantiate<TAggregate>(string id)
         
    public IAggregateHandler<TAggregate> Instantiate<TAggregate>(string id) where TAggregate : class, IAggregate, new()
        => clusterClient.GetGrain<IAggregateHandlerGrain<TAggregate>>(Guid.Parse(id));
}
