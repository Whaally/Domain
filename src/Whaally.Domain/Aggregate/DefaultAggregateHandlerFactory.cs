using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Aggregate;

internal class DefaultAggregateHandlerFactory : IAggregateHandlerFactory
{
    private readonly Dictionary<string, IAggregateHandler> _dictionary = new();
    private readonly IServiceProvider _serviceProvider;

    public DefaultAggregateHandlerFactory(IServiceProvider serviceProvider)
    {
            _serviceProvider = serviceProvider;
        }

    public IAggregateHandler<TAggregate> Instantiate<TAggregate>(string id)
        where TAggregate : class, IAggregate, new()
    {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            if (_dictionary.TryGetValue(id, out var handler)) 
                return (IAggregateHandler<TAggregate>)handler;
            
            handler = new DefaultAggregateHandler<TAggregate>(_serviceProvider, id)
            {
                Aggregate = new TAggregate()
            };

            _dictionary.Add(id, handler);

            return (IAggregateHandler<TAggregate>)handler;
        }
}