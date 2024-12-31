using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public static class IAggregateHandlerExtensions
{
    public static Task<IResult<IEventEnvelope>> Trigger(this IAggregateHandler aggregateHandler, params ICommand[] commands)
        => aggregateHandler.Trigger(new CommandEnvelope(
            new CommandMetadata(),
            commands));
    
    public static Task<IResult<IEventEnvelope>> Evaluate(this IAggregateHandler aggregateHandler, params ICommand[] commands)
        => aggregateHandler.Evaluate(new CommandEnvelope(
            new CommandMetadata(),
            commands));
    
    public static Task<IResultBase> Apply(this IAggregateHandler aggregateHandler, params IEvent[] events)
        => aggregateHandler.Apply(new EventEnvelope(
            new EventMetadata(),
            events));
}
