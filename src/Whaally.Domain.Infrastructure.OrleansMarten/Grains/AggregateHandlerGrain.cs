using FluentResults;
using Marten;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Providers;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Aggregate;
using Whaally.Domain.Event;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Grains
{
    [LogConsistencyProvider(ProviderName = "LogStorage")]
    public class AggregateHandlerGrain<TAggregate> :
        JournaledGrain<TAggregate, IEventEnvelope>,
        ICustomStorageInterface<TAggregate, IEventEnvelope>,
        IAggregateHandlerGrain<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
        private readonly IServiceProvider _services;
        private readonly IDocumentStore _store;
        private readonly ILogger<AggregateHandlerGrain<TAggregate>> _logger;

        private IAggregateHandler<TAggregate> _aggregateHandler;
        private TAggregate _aggregate = new();

        public AggregateHandlerGrain(
            IServiceProvider services,
            IDocumentStore documentStore,
            ILogger<AggregateHandlerGrain<TAggregate>> logger)
        {
            _services = services;
            _logger = logger;

            _aggregate = new();
            _store = documentStore;
            _aggregateHandler = new DefaultAggregateHandler<TAggregate>(_services, this.GetPrimaryKey().ToString())
            {
                Aggregate = _aggregate
            };
        }

        public override async Task OnActivateAsync(CancellationToken token)
        {
            await RefreshNow();

            await base.OnActivateAsync(token);
        }

        public async Task<IResultBase> Apply(params IEventEnvelope[] events)
        {
            if (events == null) return Result.Ok();

            RaiseEvents(events);

            /*
             * Whether or not events are confirmed within the apply method had a significant impact on performance.
             * On my local machine (with spotty internet connection to Azure):
             * - With confirmation: +-300/400ms
             * - Without immediate confirmation (delayed confirmation): +-30/40ms
             * The tradeoff to make here is whether or not to have certainty that the operations are applied.
             * When the actor crashes between events are confirmed these would be lost.
             * 
             * Without confirmation multiple events can be sent after one another while and be applied against stale
             * local state. We can probably work our way around that while delaying the application to the database,
             * which is the behaviour we want. This gets rid of the significant confirmation delay while also ensuring
             * events are applied against the latest state.
             */
            await ConfirmEvents();

            _logger.LogInformation("Events applied: {@events}", events);

            return Result.Ok();
        }

        public async Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands)
        {
            var result = await _aggregateHandler.Evaluate(commands);

            if (result.IsSuccess)
                _logger.LogInformation("Evaluation succesful\r\n\tCommands: {@commands}", commands);
            else
                _logger.LogWarning("Evaluation failed\r\n\tCommands: {@command}\r\n\tReasons: {@reasons}", commands, result.Reasons);

            return result;
        }

        protected override void TransitionState(
            TAggregate state,
            IEventEnvelope @event)
        {
            /* In this case, with the DefaultAggregateHandler, there are no async operations 
             * to wait on. Perhaps we should implement a more solid approach though.
             * 
             * Given the event handler is synchronous, we can:
             * 1. Create a new event handler context with the current aggregate, and metadata defined on the event envelope
             * 2. Retrieve the appropriate event handler for the supplied event
             * 3. Invoke the event handler with the context and the event
             * 
             * As is outlined above is analogous to the current behaviour of the DefaultAggregateHandler.
             */
            _aggregateHandler.Apply(@event);
        }

        public Task<IResultBase> Confirm(params IEventEnvelope[] events)
        {
            return _aggregateHandler.Confirm();
        }

        [ReadOnly]
        public Task<TSnapshot> Snapshot<TSnapshot>() where TSnapshot : ISnapshot
        {
            return _aggregateHandler.Snapshot<TSnapshot>();
        }

        public async Task<KeyValuePair<int, TAggregate>> ReadStateFromStorage()
        {
            await using var session = _store.LightweightSession();

            var events = await session.Events.FetchStreamAsync(this.GetPrimaryKey());

            if (!events.Any())
                return new KeyValuePair<int, TAggregate>(0, _aggregate);

            foreach (var @event in events)
            {
                await _aggregateHandler.Apply(new EventEnvelope(
                    (IEvent)@event.Data,
                    new EventMetadata(@event.StreamId.ToString())
                    {
                        Timestamp = @event.Timestamp.DateTime
                    }));
            }

            return new KeyValuePair<int, TAggregate>(
                (int)events.Max(q => q.Version),
                _aggregate);
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IEventEnvelope> updates, int expectedversion)
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
}
