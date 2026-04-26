using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class AggregateFactory : IAggregateFactory
{
    public T Instantiate<T>()
        where T : class
        => (T)ObjectHelpers.CreateCtor(typeof(T))();
}
