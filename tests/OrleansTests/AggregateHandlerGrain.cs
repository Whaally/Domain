using Orleans.Concurrency;
using Orleans.Providers;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Grains;

namespace OrleansTests;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AggregateHandlerGrain<TAggregate>(IServiceProvider services)
    : AbstractAggregateHandlerGrain<TAggregate>(services)
    where TAggregate : class, IAggregate, new()
{
    public override Task<KeyValuePair<int, TAggregate>> ReadStateFromStorage() =>
        Task.FromResult(new KeyValuePair<int, TAggregate>(
            0,
            Aggregate));
    
    public override Task<bool> ApplyUpdatesToStorage(
        IReadOnlyList<EventEnvelope> updates, 
        int expectedversion) => 
        Task.FromResult(true);
}
