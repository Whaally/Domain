using FluentResults;

namespace Whaally.Domain.Abstractions;

/// <summary>
///     Central component providing behaviour for the high-level interaction between different domain components.
/// </summary>
public interface IEvaluationAgent : IDisposable
{   
    /// <summary>
    ///     Invokes a saga, meaning it runs the saga, and consequently invokes all resulting events as well
    ///
    ///     Possibly incurs side effects
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="event"></param>
    /// <returns></returns>
    public Task<IResultBase> Invoke(ISaga saga, IEventEnvelope @event);
    
    /// <summary>
    ///     Evaluates a service, meaning it will run the service and collect its output as a number of commands, but not
    ///     continue eavaluating these commands.
    ///
    ///     Should be side effect free
    /// </summary>
    /// <param name="service"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public Task<IResult<ICommandEnvelope[]>> Evaluate(IServiceEnvelope service);
    
    /// <summary>
    ///     Evaluates a command, meaning it runs the command and collects its output as events, but does not apply these
    ///
    ///     Should be side effect free 
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands);
    
    /// <summary>
    ///     Applies several events, triggering a state change on corresponding aggregates
    ///
    ///     Possibly incurs side effects
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public Task<IResultBase> Apply(params IEventEnvelope[] events);
    
    /// <summary>
    ///     Continue from an event onwards. Finds relevant sagas and invokes these
    ///
    ///     Possibly incurs side effects
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public Task<IResultBase> Continue(IEventEnvelope events);

    public Task Abort(params ICommandMetadata[] metadata);
    
    /// <summary>
    ///     Invokes a number of commands, meaning they are ran, and the resulting events are applied to the
    ///     corresponding aggregates
    ///
    ///     Possibly incurs side effects
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    public async Task<IResult<IEventEnvelope[]>> Invoke(params ICommandEnvelope[] commandEnvelopes)
    {
        // Assign a transaction to the commands involved in this operation
        // If you start assigning transaction ids on your own; you are own your own.
        if (commandEnvelopes.All(q => string.IsNullOrEmpty(q.Metadata.TransactionId)))
        {
            var transactionId = Guid.NewGuid().ToString();
            
            foreach (var envelope in commandEnvelopes) 
                envelope.Metadata.TransactionId = transactionId;
        }
        
        // ToDo: Check if there is only a single aggregate involved. If so, directly run the Trigger on the aggregate handler for performance benefits.
        var commandResult = await Evaluate(commandEnvelopes);
        
        if (commandResult.IsFailed)
        {
            await Abort(commandEnvelopes
                .Select(q => q.Metadata)
                .ToArray());
            
            return Result.Fail<IEventEnvelope[]>(commandResult.Errors);
        }
        
        var eventResult = await Apply(commandResult.Value);

        if (eventResult.IsFailed)
        {
            await Abort(commandEnvelopes
                .Select(q => q.Metadata)
                .ToArray());
            
            return Result.Fail<IEventEnvelope[]>(eventResult.Errors);
        }
        
        return commandResult;
    }
    
    public async Task<IResult<IEventEnvelope[]>> Invoke(
        IServiceEnvelope serviceEnvelope)
    {
        if (string.IsNullOrEmpty(serviceEnvelope.Metadata.TransactionId))
            serviceEnvelope.Metadata.TransactionId = Guid.NewGuid().ToString();
        
        var serviceResult = await Evaluate(serviceEnvelope);
        
        if (serviceResult.IsFailed)
            return new Result<IEventEnvelope[]>()
                .WithReasons(serviceResult.Reasons);
        
        return await Invoke(serviceResult.Value);
    }
}
