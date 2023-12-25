using FluentResults;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;

namespace Whaally.Domain.Abstractions.Aggregate;

public interface IAggregateHandler
{
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommand[] commands)
        => Evaluate(commands
            .Select(q => new CommandEnvelope(
                q,
                new CommandMetadata
                {
                    Timestamp = DateTime.UtcNow
                }))
            .ToArray());

    /// <summary>
    ///     Evaluate the provided commands against the current state.
    /// </summary>
    /// <param name="commands">The commands to evaluate</param>
    /// <returns>async result containing resulting events if successful</returns>
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands);

    /// <summary>
    ///     Apply the provided events to the current state.
    /// </summary>
    /// <param name="events">The events to apply</param>
    /// <returns>async Task</returns>
    public Task<IResultBase> Apply(params IEventEnvelope[] events);

    [Obsolete("The confirm method had been renamed to `Continue`. This method will be removed in a future version.")]
    public Task<IResultBase> Confirm(params IEventEnvelope[] events)
        => Continue(events);
    
    public Task<IResultBase> Continue(params IEventEnvelope[] events);

    public Task<TSnapshot> Snapshot<TSnapshot>()
        where TSnapshot : ISnapshot;
}

/// <summary>
/// This interface exists because we want to retrieve actual implementations for the relevant handler
/// from the DI container.
/// </summary>
/// <typeparam name="TAggregate">The aggregate type for which this aggregate handler exists</typeparam>
public interface IAggregateHandler<TAggregate> : IAggregateHandler
    where TAggregate : class, IAggregate
{
}