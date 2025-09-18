namespace Whaally.Domain.Abstractions;

public interface IAggregateHandlerFactory
{
    public IAggregateHandler Instantiate(Type aggregateType, Guid id)
        => (GetType()
            .GetMethod(nameof(Instantiate))!
            .MakeGenericMethod(aggregateType)
            .Invoke(this, [ id ]) as IAggregateHandler)!;

    public IAggregateHandler<TAggregate> Instantiate<TAggregate>(Guid id)
        where TAggregate : class, IAggregate;
}
