using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ISaga
{
    public Task Evaluate(ISagaContext context, IEvent @event);
}

public interface ISaga<TEvent> : ISaga
    where TEvent : class, IEvent
{
    Task ISaga.Evaluate(ISagaContext context, IEvent @event)
        => Evaluate(context, (TEvent)@event);

    public Task Evaluate(ISagaContext context, TEvent @event);
}
