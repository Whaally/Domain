using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IAggregateHandler
{
    /// <summary>
    ///     Trigger a command to evaluate the command and applying all related side effects when applicable.
    /// </summary>
    /// <param name="commandEnvelope"></param>
    /// <param name="commands">The commands to trigger evaluation and application of side effects for</param>
    /// <returns>Events which had been applied to the aggregate</returns>
    public async Task<IResult<EventEnvelope>> Trigger(CommandEnvelope commandEnvelope)
    {
        var commandResult = await Evaluate(commandEnvelope);

        if (commandResult.IsFailed)
            return commandResult;

        var eventResult = await Apply(commandResult.Value);

        return eventResult.IsFailed 
            ? Result.Fail<EventEnvelope>(eventResult.Errors) 
            : commandResult;
    }
    
    /// <summary>
    ///     Evaluate the provided commands against the current state.
    /// </summary>
    /// <param name="commands">The commands to evaluate</param>
    /// <returns>async result containing events if successful</returns>
    public Task<IResult<EventEnvelope>> Evaluate(CommandEnvelope commandEnvelope);


    
    /// <summary>
    ///     Apply the provided events to the current state.
    /// </summary>
    /// <param name="events">The events to apply</param>
    /// <returns>async Task</returns>
    public Task<IResultBase> Apply(EventEnvelope eventEnvelope);

    /// <summary>
    ///     Allows abortion of a running transaction
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public Task Abort(IMessageMetadata metadata);
    
    public Task<TSnapshot> Snapshot<TSnapshot>()
        where TSnapshot : ISnapshot;
}

/// <summary>
///     This interface exists because we want to retrieve actual implementations for the relevant handler
///     from the DI container.
/// </summary>
/// <typeparam name="TAggregate">The aggregate type for which this aggregate handler exists</typeparam>
public interface IAggregateHandler<TAggregate> : IAggregateHandler
    where TAggregate : class, IAggregate;
