using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

// ReSharper disable InvertIf
namespace Whaally.Domain;

/*
 * DEVELOPMENT NOTE:
 * - Do not inject dependencies to this class (IAggregateHandlerFactory & IUnitOfWork) as concrete instances.
 *   Doing so causes an infinite loop as this class is often injected as dependency to those.
 */
public class DomainContext(
    IServiceProvider services,
    IEnumerable<Type>? commandHandlerTypes = null,
    IEnumerable<Type>? eventHandlerTypes = null,
    IEnumerable<Type>? serviceHandlerTypes = null,
    IEnumerable<Type>? sagaTypes = null,
    IEnumerable<Type>? snapshotFactoryTypes = null)
{
    internal static ActivitySource ActivitySource = new("Whaally.Domain");

    private IUnitOfWork NewUnitOfWork() => services.GetRequiredService<IUnitOfWork>();
    private Activity? NewActivity() => ActivitySource.StartActivity(
        ActivityKind.Internal,
        name: nameof(DomainContext),
        tags: new Dictionary<string, object?>());

    #region handler metadata
    /// <summary>
    ///     Metadata about the command handlers registered with this domain instance.
    /// </summary>
    public IReadOnlyList<CommandHandlerMeta> CommandHandlers { get; } = (commandHandlerTypes ?? [])
        .Select(CommandHandlerMeta.From)
        .ToList()
        .AsReadOnly();

    /// <summary>
    ///     Metadata about the event handlers registered with this domain.
    /// </summary>
    public IReadOnlyList<EventHandlerMeta> EventHandlers { get; } = (eventHandlerTypes ?? [])
        .Select(EventHandlerMeta.From)
        .ToList()
        .AsReadOnly();

    /// <summary>
    ///     Metadata about the service handlers registered with this domain.
    /// </summary>
    public IReadOnlyList<ServiceHandlerMeta> ServiceHandlers { get; } = (serviceHandlerTypes ?? [])
        .Select(ServiceHandlerMeta.From)
        .ToList()
        .AsReadOnly();

    /// <summary>
    ///     Metadata about the sagas registered with this domain.
    /// </summary>
    public IReadOnlyList<SagaMeta> Sagas { get; } = (sagaTypes ?? [])
        .Select(SagaMeta.From)
        .ToList()
        .AsReadOnly();

    /// <summary>
    ///     Metadata about the snapshot factories registered with this domain.
    /// </summary>
    public IReadOnlyList<SnapshotFactoryMeta> SnapshotFactories { get; } = (snapshotFactoryTypes ?? [])
        .Select(SnapshotFactoryMeta.From)
        .ToList()
        .AsReadOnly();

    #endregion
    
    #region Aggregate handlers
    private async Task<IAggregateHandler> GetAggregate(Type type, Guid id)
    {
        var aggregateHandlerFactory = services.GetRequiredService<IAggregateHandlerFactory>();
        
        if (type.IsAssignableTo(typeof(IAggregate)))
        {
            return await aggregateHandlerFactory.Instantiate(type, id);
        }
        
        Type? aggregateType = null;
        
        if (type.IsAssignableTo(typeof(ICommand)))
            aggregateType = CommandHandlers
                .SingleOrDefault(q => q.CommandType == type)
                ?.AggregateType;
        else if (type.IsAssignableTo(typeof(IEvent)))
            aggregateType = EventHandlers
                .SingleOrDefault(q => q.EventType == type)
                ?.AggregateType;
        
        if (aggregateType == null) 
            throw new Exception($"Aggregate type could not be resolved for {type.FullName}");
            
        var handler = await aggregateHandlerFactory.Instantiate(
            aggregateType,
            id);
        
        if (handler == null)
            throw new Exception($"Command handler could not be resolved for {type.FullName}");

        return handler;
    }
    
    public virtual IAggregateHandler<TAggregate> GetAggregate<TAggregate>(Guid id)
        where TAggregate : class, IAggregate 
        => (IAggregateHandler<TAggregate>)GetAggregate(typeof(TAggregate), id);
    #endregion
    
    public virtual Task<IResult<EventEnvelope[]>> Evaluate<TCommand>(
        Guid aggregateId,
        TCommand command)
        where TCommand : class, ICommand
    {
        return Evaluate(
            new CommandMetadata
            {
                AggregateId = aggregateId,
                AggregateType = this.GetCommonAggregateType([ command ]),
                CreatedAt = DateTimeOffset.UtcNow,
                TransactionId = Guid.NewGuid().ToString()
            },
            command);
    }
    
    public virtual async Task<IResult<EventEnvelope[]>> Evaluate(
        Guid aggregateId,
        params ICommand[] commands)
    {
        using var unitOfWork = NewUnitOfWork();
        
        return await unitOfWork.Evaluate(
            new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    AggregateType = this.GetCommonAggregateType(commands),
                    CreatedAt = DateTimeOffset.UtcNow,
                    TransactionId = Guid.NewGuid().ToString()
                },
                commands));
    }

    public virtual async Task<IResult<EventEnvelope[]>> Evaluate(
        CommandMetadata metadata,
        params ICommand[] commands)
    {
        using var unitOfWork = NewUnitOfWork();
        
        return await unitOfWork
            .Evaluate(new CommandEnvelope(metadata, commands));
    }

    public virtual async Task<IResult<EventEnvelope[]>> Invoke(
        CommandMetadata metadata,
        params ICommand[] commands)
    {
        using var unitOfWork = NewUnitOfWork();
        
        return await unitOfWork.Invoke(
            new CommandEnvelope(
                metadata,
                commands));
    }
    
    public virtual async Task<IResult<EventEnvelope[]>> Invoke(
        Guid aggregateId,
        params ICommand[] commands)
    {
        using var activity = NewActivity();
        using var unitOfWork = NewUnitOfWork();

        return await unitOfWork.Invoke(
            new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    AggregateType = this.GetCommonAggregateType(commands),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ParentContext = activity?.Context,
                    TransactionId = Guid.NewGuid().ToString()
                },
                commands));
    }
    
    public virtual async Task<IResult<EventEnvelope[]>> Invoke(
        IService service,
        ServiceMetadata? metadata = null)
    {
        using var activity = NewActivity();
        using var unitOfWork = NewUnitOfWork();
        
        return await unitOfWork.Invoke(
            new ServiceEnvelope(
                metadata ?? new ServiceMetadata
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    ParentContext = activity?.Context,
                    TransactionId = Guid.NewGuid().ToString()
                }, service));
    }
    
    public IServiceHandler GetServiceHandler(Type serviceType) => 
        (IServiceHandler)services.GetRequiredService(
            ServiceHandlers
                .Single(q => q.ServiceType == serviceType)
                .HandlerType);
    
    public IEnumerable<ISaga> GetSaga(Type eventType) =>
        Sagas.Where(q => q.EventType == eventType)
            .Select(q => (ISaga)services.GetRequiredService(q.HandlerType));
    
    public IEventHandler GetEventHandler(Type eventType) =>
        (IEventHandler)services.GetRequiredService(
            EventHandlers
                .Single(q => q.EventType == eventType)
                .HandlerType);
    
    // TODO: See if we can also require the aggregate type as argument to validate that we're executing the correct handlers
    //       Same goes for the event handlers though.
    public ICommandHandler GetCommandHandler(Type commandType) =>
        (ICommandHandler)services.GetRequiredService(
            CommandHandlers
                .Single(q => q.CommandType == commandType)
                .HandlerType);
}
