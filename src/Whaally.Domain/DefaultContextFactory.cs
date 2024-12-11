using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultContextFactory(IServiceProvider services) : IContextFactory
{
    public ISagaContext CreateSagaContext(IEventMetadata metadata)
        => new SagaContext(services, metadata)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };

    public IServiceHandlerContext CreateServiceHandlerContext(IServiceMetadata metadata)
        => new ServiceHandlerContext(services)
        {
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            ParentContext = metadata.ParentContext
        };

    public ICommandHandlerContext<TAggregate> CreateCommandHandlerContext<TAggregate>(
        TAggregate aggregate,
        ICommandMetadata metadata)
        where TAggregate : class, IAggregate =>
        new CommandHandlerContext<TAggregate>(services, metadata.AggregateId)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };

    public IEventHandlerContext<TAggregate> CreateEventHandlerContext<TAggregate>(
        TAggregate aggregate,
        IEventMetadata metadata)
        where TAggregate : class, IAggregate => 
        new EventHandlerContext<TAggregate>(metadata.AggregateId)
        {
            Aggregate = aggregate,
            Attributes = new ReadOnlyDictionary<string, object>(metadata.Attributes),
            AggregateId = metadata.AggregateId,
            ParentContext = metadata.ParentContext
        };
}
