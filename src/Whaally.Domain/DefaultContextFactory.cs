using System.Collections.ObjectModel;
using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultContextFactory(IServiceProvider services) : IContextFactory
{
    public ISagaContext CreateSagaContext(IEventMetadata metadata, Activity? activity = null)
        => new SagaContext(
            services, 
            metadata)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };

    public IServiceHandlerContext CreateServiceHandlerContext(IServiceMetadata metadata, Activity? activity = null)
        => new ServiceHandlerContext(services, metadata)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
        };

    public ICommandHandlerContext<TAggregate> CreateCommandHandlerContext<TAggregate>(
        TAggregate aggregate,
        ICommandMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate =>
        new CommandHandlerContext<TAggregate>(services, metadata.AggregateId, activity)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };

    public IEventHandlerContext<TAggregate> CreateEventHandlerContext<TAggregate>(
        TAggregate aggregate,
        IEventMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate => 
        new EventHandlerContext<TAggregate>(metadata.AggregateId)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };
}
