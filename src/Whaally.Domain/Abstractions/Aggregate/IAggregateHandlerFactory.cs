namespace Whaally.Domain.Abstractions.Aggregate
{
    public interface IAggregateHandlerFactory
    {
        public IAggregateHandler Instantiate(Type aggregateType, string id)
            => (GetType()
                .GetMethod(nameof(Instantiate))!
                .MakeGenericMethod(aggregateType)
                .Invoke(this, new object[] { id }) as IAggregateHandler)!;

        public IAggregateHandler<TAggregate> Instantiate<TAggregate>(string id)
            where TAggregate : class, IAggregate, new();
    }
}
