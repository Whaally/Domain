using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

/// <summary>
///     Object responsible for instantiating new aggregate handler instances. As the handler can assume the role of
///         either a concrete instance or a proxy to one this may be the place to link in the placement strategy.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="aggregateFactory"></param>
internal class AggregateHandlerFactory(
    IServiceProvider serviceProvider,
    IAggregateFactory aggregateFactory)
    : IAggregateHandlerFactory
{
    private readonly Dictionary<Guid, IAggregateHandler> _dictionary = new();

    public Task<IAggregateHandler<TAggregate>> Instantiate<TAggregate>(Guid id)
        where TAggregate : class, IAggregate
    {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        
        if (_dictionary.TryGetValue(id, out var handler)) 
            return Task.FromResult((IAggregateHandler<TAggregate>)handler);
        
        handler = new AggregateHandler<TAggregate>(serviceProvider, id)
        {
            Aggregate = aggregateFactory.Instantiate<TAggregate>()
        };
        
        _dictionary.Add(id, handler);
        
        return Task.FromResult((IAggregateHandler<TAggregate>)handler);
    }
}
