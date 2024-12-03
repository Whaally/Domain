using FluentResults;

namespace Whaally.Domain.Abstractions;

/// <summary>
///     Central component providing behaviour for the high-level interaction between different domain components.
/// </summary>
public interface IEvaluationAgent
{
    public Task<IResult<ICommandEnvelope[]>> Run<TService>(IServiceEnvelope<TService> service)
        where TService : class, IService;
    
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands);
    
    public Task<IResultBase> Apply(params IEventEnvelope[] events);
    
    public Task<IResultBase> Continue(IEventEnvelope @event);
    
    
    public async Task<IResult<IEventEnvelope[]>> Trigger(params ICommandEnvelope[] commands)
    {
        // ToDo: Check if there is only a single aggregate involved. If so, directly run the Trigger on the aggregate handler for performance benefits.
        
        var commandResult = await Evaluate(commands);

        if (commandResult.IsFailed)
            return Result.Fail<IEventEnvelope[]>(commandResult.Errors);

        var eventResult = await Apply(commandResult.Value);

        if (eventResult.IsFailed)
            return Result.Fail<IEventEnvelope[]>(eventResult.Errors);

        foreach (var @event in commandResult.Value)
        {
            await Continue(@event);
        }

        return commandResult;
    }
    
    public async Task<IResult<IEventEnvelope[]>> Trigger<TService>(IServiceEnvelope<TService> service)
        where TService : class, IService
    {
        var serviceResult = await Run(service);

        if (serviceResult.IsFailed)
            return Result.Fail<IEventEnvelope[]>(serviceResult.Errors);

        return await Trigger(serviceResult.Value);
    }
}
