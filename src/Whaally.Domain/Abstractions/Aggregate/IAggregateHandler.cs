using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IAggregateHandler
{
    public Task<IResult<IEventEnvelope[]>> Trigger(params ICommand[] commands)
        => Trigger(commands
            .Select(q => new CommandEnvelope(
                q,
                new CommandMetadata
                {
                    Timestamp = DateTime.UtcNow
                }))
            .ToArray());

    /// <summary>
    ///     Trigger a command to evaluate the command and applying all related side effects when applicable.
    /// </summary>
    /// <param name="commands">The commands to trigger evaluation and application of side effects for</param>
    /// <returns>Events which had been applied to the aggregate</returns>
    public async Task<IResult<IEventEnvelope[]>> Trigger(params ICommandEnvelope[] commands)
    {
        var commandResult = await Evaluate(commands);

        if (commandResult.IsFailed)
            return commandResult;

        var eventResult = await Apply(commandResult.Value);

        return eventResult.IsFailed 
            ? Result.Fail<IEventEnvelope[]>(eventResult.Errors) 
            : commandResult;
    }
    
    /// <summary>
    ///     Evaluate the provided commands against the current state.
    /// </summary>
    /// <param name="commands">The commands to evaluate</param>
    /// <returns>async result containing events if successful</returns>
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
    /// <returns>async result containing events if successful</returns>
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands);
    
    /// <summary>
    ///     Apply the provided events to the current state.
    /// </summary>
    /// <param name="events">The events to apply</param>
    /// <returns>async Task</returns>
    public Task<IResultBase> Apply(params IEventEnvelope[] events);
    
    public Task<TSnapshot> Snapshot<TSnapshot>()
        where TSnapshot : ISnapshot;
}

/// <summary>
///     This interface exists because we want to retrieve actual implementations for the relevant handler
///     from the DI container.
/// </summary>
/// <typeparam name="TAggregate">The aggregate type for which this aggregate handler exists</typeparam>
public interface IAggregateHandler<TAggregate> : IAggregateHandler
    where TAggregate : class, IAggregate
{
}
