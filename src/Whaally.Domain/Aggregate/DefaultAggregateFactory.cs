using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Aggregate;

public class DefaultAggregateFactory : IAggregateFactory
{
    public T Instantiate<T>()
        where T : class
        => (T)ObjectHelpers.CreateCtor(typeof(T))();
}
