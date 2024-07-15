using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public class OrleansAggregateHandlerFactory : IAggregateHandlerFactory
{
    private readonly IClusterClient _clusterClient;

    public OrleansAggregateHandlerFactory(
        IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    IAggregateHandler IAggregateHandlerFactory.Instantiate(Type aggregateType, string id)
        => (GetType()
            .GetMethod(nameof(IAggregateHandlerFactory.Instantiate))!
            .MakeGenericMethod(aggregateType)
            .Invoke(this, [id]) as IAggregateHandler)!;

    IAggregateHandler<TAggregate> IAggregateHandlerFactory.Instantiate<TAggregate>(string id)
        => _clusterClient.GetGrain<IAggregateHandlerGrain<TAggregate>>(Guid.Parse(id));
}