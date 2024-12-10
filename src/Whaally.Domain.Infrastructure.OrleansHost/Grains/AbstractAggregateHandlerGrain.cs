using FluentResults;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost.Grains;

public abstract class AbstractAggregateHandlerGrain<TAggregate> :
    JournaledGrain<TAggregate, IEventEnvelope>,
    ICustomStorageInterface<TAggregate, IEventEnvelope>,
    IAggregateHandlerGrain<TAggregate>
    where TAggregate : class, IAggregate, new()
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AbstractAggregateHandlerGrain<TAggregate>> _logger;

    protected IAggregateHandler<TAggregate> AggregateHandler;
    protected TAggregate Aggregate = new();

    public AbstractAggregateHandlerGrain(
        IServiceProvider services,
        ILogger<AbstractAggregateHandlerGrain<TAggregate>> logger)
    {
        _services = services;
        _logger = logger;

        Aggregate = new();
        AggregateHandler = new DefaultAggregateHandler<TAggregate>(_services, this.GetPrimaryKey().ToString())
        {
            Aggregate = Aggregate
        };
    }

    public override async Task OnActivateAsync(CancellationToken token)
    {
        await RefreshNow();

        await base.OnActivateAsync(token);
    }
    
    protected override void TransitionState(
        TAggregate state,
        IEventEnvelope eventEnvelope)
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
        AggregateHandler.Apply(eventEnvelope);
    }

    public async Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope)
    {
        var result = await AggregateHandler.Evaluate(
            commandEnvelope.Messages.ToArray());

        if (result.IsSuccess)
            _logger.LogTrace("Evaluation succesful\r\n\tCommands: {@commands}", commandEnvelope.Messages);
        else
            _logger.LogTrace("Evaluation failed\r\n\tCommands: {@command}\r\n\tReasons: {@reasons}", commandEnvelope.Messages,
                result.Reasons);

        return result;
    }

    public async Task<IResultBase> Apply(IEventEnvelope eventEnvelope)
    {
        if (eventEnvelope.Messages?.Count() == 0) return Result.Ok();

        RaiseEvent(eventEnvelope);

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

        _logger.LogTrace("Events applied: {@events}", eventEnvelope.Messages);
        
        return Result.Ok();
    }

    [ReadOnly]
    public Task<TSnapshot> Snapshot<TSnapshot>() where TSnapshot : ISnapshot
    {
        return AggregateHandler.Snapshot<TSnapshot>();
    }

    /// <summary>
    ///     Retrieve the current aggregate version from storage
    /// </summary>
    /// <returns>
    ///     A key-value pair, with the key being the version number, and the value being the corresponding instance of
    ///     the aggregate.
    /// </returns>
    public abstract Task<KeyValuePair<int, TAggregate>> ReadStateFromStorage();

    /// <summary>
    ///     Persists changes
    /// </summary>
    /// <param name="updates">The events reflecting the changes</param>
    /// <param name="expectedversion">The expected version after the events had been applied</param>
    /// <returns>A boolean value indicating success state</returns>
    public abstract Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IEventEnvelope> updates, int expectedversion);
}