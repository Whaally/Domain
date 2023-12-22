using Marten;
using Microsoft.Extensions.Logging;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;
using Whaally.Domain.Infrastructure.OrleansHost.Grains;

namespace Whaally.Domain.Infrastructure.OrleansHost.MartenPersistence;

public class AggregateHandlerGrain<TAggregate> 
    : AbstractAggregateHandlerGrain<TAggregate> 
    where TAggregate : class, IAggregate, new()
{
    private readonly IDocumentStore _store;
    
    public AggregateHandlerGrain(
        IServiceProvider services,
        ILogger<AbstractAggregateHandlerGrain<TAggregate>> logger,
        IDocumentStore store) 
        : base(services, logger)
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
                (IEvent)@event.Data,
                new EventMetadata(@event.StreamId.ToString())
                {
                    Timestamp = @event.Timestamp.DateTime
                }));
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
            updates.Select(q => q.Message));

        await session.SaveChangesAsync();
        return true;
    }
}
