namespace Whaally.Domain.Abstractions;

public interface IAggregateHandlerFactory
{
    public async Task<IAggregateHandler> Instantiate(Type aggregateType, Guid id)
    {
        var i = (Task)GetType()
            .GetMethod(nameof(Instantiate))!
            .MakeGenericMethod(aggregateType)
            .Invoke(this, [id])!;

        return (IAggregateHandler)i.GetType().GetProperty("Result")!.GetValue(i)!;
    }

    public Task<IAggregateHandler<TAggregate>> Instantiate<TAggregate>(Guid id)
        where TAggregate : class, IAggregate;
}
