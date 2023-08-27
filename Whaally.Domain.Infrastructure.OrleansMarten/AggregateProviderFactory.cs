using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Infrastructure.OrleansMarten;

namespace ResaleRadar.Infrastructure
{
    public class AggregateHandlerFactory : IAggregateHandlerFactory
    {
        private readonly IClusterClient _clusterClient;

        public AggregateHandlerFactory(
            IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        IAggregateHandler<TAggregate> IAggregateHandlerFactory.Instantiate<TAggregate>(string id)
            => _clusterClient.GetGrain<IAggregateHandlerGrain<TAggregate>>(Guid.Parse(id));
    }
}
