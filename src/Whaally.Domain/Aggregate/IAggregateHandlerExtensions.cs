using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public static class IAggregateHandlerExtensions
{
    public static Task<IResult<EventEnvelope>> Trigger(this IAggregateHandler aggregateHandler, params ICommand[] commands)
        => aggregateHandler.Trigger(
            new CommandEnvelope(
                new CommandMetadata(),
                commands));
    
    public static Task<IResult<EventEnvelope>> Evaluate(this IAggregateHandler aggregateHandler, params ICommand[] commands)
        => aggregateHandler.Evaluate(
            new CommandEnvelope(
                new CommandMetadata(),
                commands));
    
    public static Task<IResult> Apply(this IAggregateHandler aggregateHandler, params IEvent[] events)
        => aggregateHandler.Apply(
            new EventEnvelope(
                new EventMetadata(),
                events));
}
