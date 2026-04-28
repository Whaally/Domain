using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class CommandHandlerContext<TAggregate> : ICommandHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    private TAggregate _aggregate = null!;
    
    public CommandHandlerContext(
        IServiceProvider services,
        Guid aggregateId,
        Activity? activity = null)
    {
        AggregateId = aggregateId;
        
        // Stuff like this would require me to rethink what I am doing.
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        _aggregate ??= services
            .GetService<IAggregateFactory>()
            ?.Instantiate<TAggregate>() ?? null!;
    }

    public Guid AggregateId { get; init; }
    public string? TransactionId { get; init; }

    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
    
    public TAggregate Aggregate
    {
        get => _aggregate; 
        init => _aggregate = value;
    }
}
