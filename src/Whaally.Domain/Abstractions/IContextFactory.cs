namespace Whaally.Domain.Abstractions;

public interface IContextFactory
{
    public ISagaContext CreateSagaContext(IEventMetadata metadata);

    public IServiceHandlerContext CreateServiceHandlerContext(IServiceMetadata metadata);

    public ICommandHandlerContext<TAggregate> CreateCommandHandlerContext<TAggregate>(
        TAggregate aggregate,
        ICommandMetadata metadata)
        where TAggregate : class, IAggregate;

    public IEventHandlerContext<TAggregate> CreateEventHandlerContext<TAggregate>(
        TAggregate aggregate, 
        IEventMetadata metadata)
        where TAggregate : class, IAggregate;
}
