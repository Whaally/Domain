using Marten;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Grains;

namespace Whaally.Domain.Infrastructure.OrleansHost.MartenPersistence;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AggregateHandlerGrain<TAggregate> 
    : AbstractAggregateHandlerGrain<TAggregate> 
    where TAggregate : class, IAggregate, new()
{
    private readonly IDocumentStore _store;
    
    public AggregateHandlerGrain(
        IServiceProvider services,
        IDocumentStore store) 
        : base(services)
    {
        _store = store;
    }

    public override async Task<KeyValuePair<int, TAggregate>> ReadStateFromStorage()
    {
        await using var session = _store.LightweightSession();

        var events = await session.Events.FetchStreamAsync(this.GetPrimaryKey());

        if (!events.Any())
            return new KeyValuePair<int, TAggregate>(0, Aggregate);

        foreach (var @event in events)
        {
            await AggregateHandler.Apply(new EventEnvelope(
                new EventMetadata
                {
                    AggregateId = @event.StreamId.ToString(),
                    CreatedAt = @event.Timestamp.DateTime
                },
                (IEvent)@event.Data));
        }

        return new KeyValuePair<int, TAggregate>(
            (int)events.Max(q => q.Version),
            Aggregate);
    }

    public override async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IEventEnvelope> updates, int expectedversion)
    {
        // ToDo: Add version check
        // ToDo: Add tracing information to each event individually

        await using var session = _store.LightweightSession();

        var action = session.Events.Append(
            this.GetPrimaryKey(),
            updates.SelectMany(q => q.Messages));

        await session.SaveChangesAsync();
        return true;
    }
}
