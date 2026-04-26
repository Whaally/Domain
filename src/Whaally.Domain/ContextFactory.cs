using System.Collections.ObjectModel;
using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class ContextFactory(IServiceProvider services) : IContextFactory
{
    public ISagaContext CreateSagaContext(EventMetadata metadata, Activity? activity = null)
        => new SagaContext(
            services, 
            metadata)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext,
            TransactionId = metadata.TransactionId
        };

    public IServiceHandlerContext CreateServiceHandlerContext(ServiceMetadata metadata, Activity? activity = null)
        => new ServiceHandlerContext(services, metadata)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            TransactionId = metadata.TransactionId
        };

    public ICommandHandlerContext<TAggregate> CreateCommandHandlerContext<TAggregate>(
        TAggregate aggregate,
        CommandMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate =>
        new CommandHandlerContext<TAggregate>(services, metadata.AggregateId, activity)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext,
            TransactionId = metadata.TransactionId
        };

    public IEventHandlerContext<TAggregate> CreateEventHandlerContext<TAggregate>(
        TAggregate aggregate,
        EventMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate => 
        new EventHandlerContext<TAggregate>(metadata.AggregateId)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext,
            TransactionId = metadata.TransactionId
        };
}
