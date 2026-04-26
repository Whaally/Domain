using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

internal class AggregateHandlerFactory(
    IServiceProvider serviceProvider,
    IAggregateFactory aggregateFactory)
    : IAggregateHandlerFactory
{
    private readonly Dictionary<Guid, IAggregateHandler> _dictionary = new();

    public IAggregateHandler<TAggregate> Instantiate<TAggregate>(Guid id)
        where TAggregate : class, IAggregate
    {
        if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
        
        if (_dictionary.TryGetValue(id, out var handler)) 
            return (IAggregateHandler<TAggregate>)handler;
        
        handler = new AggregateHandler<TAggregate>(serviceProvider, id)
        {
            Aggregate = aggregateFactory.Instantiate<TAggregate>()
        };

        _dictionary.Add(id, handler);

        return (IAggregateHandler<TAggregate>)handler;
    }
}
