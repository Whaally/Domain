using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Aggregate;

internal class DefaultAggregateHandlerFactory(
    IServiceProvider serviceProvider,
    IAggregateFactory aggregateFactory)
    : IAggregateHandlerFactory
{
    private readonly Dictionary<string, IAggregateHandler> _dictionary = new();

    public IAggregateHandler<TAggregate> Instantiate<TAggregate>(string id)
        where TAggregate : class, IAggregate
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        
        if (_dictionary.TryGetValue(id, out var handler)) 
            return (IAggregateHandler<TAggregate>)handler;
        
        handler = new DefaultAggregateHandler<TAggregate>(serviceProvider, id)
        {
            Aggregate = aggregateFactory.Instantiate<TAggregate>()
        };

        _dictionary.Add(id, handler);

        return (IAggregateHandler<TAggregate>)handler;
    }
}
