using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ISaga
{
    public IAsyncEnumerable<IReason> Evaluate(ISagaContext context, IEvent @event);
}

public interface ISaga<TEvent> : ISaga
    where TEvent : class, IEvent
{
    IAsyncEnumerable<IReason> ISaga.Evaluate(ISagaContext context, IEvent @event)
        => Evaluate(context, (TEvent)@event);

    public IAsyncEnumerable<IReason> Evaluate(ISagaContext context, TEvent @event);
}
