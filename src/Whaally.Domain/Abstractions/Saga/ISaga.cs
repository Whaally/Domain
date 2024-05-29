using FluentResults;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;

namespace Whaally.Domain.Abstractions.Saga;

public interface ISaga
{
    public Task<IResultBase> Evaluate(ISagaContext context, IEvent @event);
}

public interface ISaga<TEvent> : ISaga
    where TEvent : class, IEvent
{
    Task<IResultBase> ISaga.Evaluate(ISagaContext context, IEvent @event)
        => Evaluate(context, (TEvent)@event);

    public Task<IResultBase> Evaluate(ISagaContext context, TEvent @event);
}