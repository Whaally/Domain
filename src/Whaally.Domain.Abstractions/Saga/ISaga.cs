namespace Whaally.Domain.Abstractions;

public interface ISaga
{
    public Task<ISagaResult> Evaluate(ISagaContext context, IEvent @event);
}

public interface ISaga<TEvent> : ISaga
    where TEvent : class, IEvent
{
    Task<ISagaResult> ISaga.Evaluate(ISagaContext context, IEvent @event)
        => Evaluate(context, (TEvent)@event);

    public Task<ISagaResult> Evaluate(ISagaContext context, TEvent @event);
}
