using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultAggregateFactory : IAggregateFactory
{
    public T Instantiate<T>()
        where T : class
        => (T)ObjectHelpers.CreateCtor(typeof(T))();
}
