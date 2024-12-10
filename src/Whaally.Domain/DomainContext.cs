using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

// ReSharper disable InvertIf

namespace Whaally.Domain;

/*
 * DEVELOPMENT NOTE:
 * - Do not inject dependencies to this class (IAggregateHandlerFactory & IEvaluationAgent) as concrete instances.
 *   Doing so causes an infinite loop as this class is often injected as dependency to those.
 */
public class DomainContext(IServiceProvider services)
{
    internal static ActivitySource ActivitySource = new("Whaally.Domain");
    
    #region handler metadata
    /// <summary>
    ///     Metadata about the command handlers registered with this domain instance.
    ///
    ///     Provide command handler types during instantiation of the domain through the <see cref="CommandHandlerTypes"/> property.
    /// </summary>
    public IReadOnlyList<CommandHandlerMeta> CommandHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the event handlers registered with this domain.
    ///
    ///     Provide event handler types during instantiation of the domain through the <see cref="EventHandlerTypes"/> property.
    /// </summary>
    public IReadOnlyList<EventHandlerMeta> EventHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the service handlers registered with this domain.
    ///
    ///     Provide service handler types during instantiation of the domain through the <see cref="ServiceHandlerTypes"/> property.
    /// </summary>
    public IReadOnlyList<ServiceHandlerMeta> ServiceHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the sagas registered with this domain.
    ///
    ///     Provide saga types during instantiation of the domain through the <see cref="SagaTypes"/> property.
    /// </summary>
    public IReadOnlyList<SagaMeta> Sagas { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the snapshot factories registered with this domain.
    ///
    ///     Provide snapshot factory types during instantiation of the domain through the <see cref="SnapshotFactoryTypes"/> property.
    /// </summary>
    public IReadOnlyList<SnapshotFactoryMeta> SnapshotFactories { get; private init; } = [];
    
    /// <summary>
    ///     Provide the types of command handlers to register with this domain instance.
    ///
    ///     Access related metadata through the <see cref="CommandHandlers"/> property.
    /// </summary>
    public IEnumerable<Type> CommandHandlerTypes
    {
        init => CommandHandlers = value.Select(CommandHandlerMeta.From).ToList().AsReadOnly();
    }
    
    /// <summary>
    ///     Provide the types of event handlers to register with this domain instance.
    ///
    ///     Access related metadata through the <see cref="EventHandlers"/> property.
    /// </summary>
    public IEnumerable<Type> EventHandlerTypes
    {
        init => EventHandlers = value.Select(EventHandlerMeta.From).ToList().AsReadOnly();
    } 
    
    /// <summary>
    ///     Provide the types of service handlers to register with this domain instance.
    ///
    ///     Access related metadata through the <see cref="ServiceHandlers"/> property.
    /// </summary>
    public IEnumerable<Type> ServiceHandlerTypes
    {
        init => ServiceHandlers = value.Select(ServiceHandlerMeta.From).ToList().AsReadOnly();
    } 

    /// <summary>
    ///     Provide the types of sagas registered with this domain instance.
    ///
    ///     Access related metadata through the <see cref="Sagas"/> property.
    /// </summary>
    public IEnumerable<Type> SagaTypes
    {
        init => Sagas = value.Select(SagaMeta.From).ToList().AsReadOnly();
    } 
    
    /// <summary>
    ///     Provide the types of snapshot factories registered with this domain instance.
    ///
    ///     Access related metadata through the <see cref="SnapshotFactories"/> property.
    /// </summary>
    public IEnumerable<Type> SnapshotFactoryTypes
    {
        init => SnapshotFactories = value.Select(SnapshotFactoryMeta.From).ToList().AsReadOnly();
    } 
    #endregion

    private IEvaluationAgent _evaluationAgent => services.GetRequiredService<IEvaluationAgent>();

    private IAggregateHandler GetAggregate(Type type, string id)
    {
        var aggregateHandlerFactory = services.GetRequiredService<IAggregateHandlerFactory>();
        
        if (type.IsAssignableTo(typeof(IAggregate)))
        {
            return aggregateHandlerFactory.Instantiate(type, id);
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
            
        var handler = aggregateHandlerFactory.Instantiate(
            aggregateType,
            id);
        
        if (handler == null)
            throw new Exception($"Command handler could not be resolved for {type.FullName}");

        return handler;
    }
    
    public virtual IAggregateHandler<TAggregate> GetAggregate<TAggregate>(string id)
        where TAggregate : class, IAggregate 
        => (IAggregateHandler<TAggregate>)GetAggregate(typeof(TAggregate), id);
    
    public virtual Task<IResult<IEventEnvelope[]>> Evaluate<TCommand>(
        string aggregateId, 
        TCommand command)
        where TCommand : class, ICommand
    {
        return Trigger(
            new CommandMetadata
            {
                AggregateId = aggregateId,
                CreatedAt = DateTimeOffset.UtcNow
            },
            command);
    }
    
    public virtual Task<IResult<IEventEnvelope[]>> Evaluate(
        string aggregateId,
        params ICommand[] commands)
    {
        return _evaluationAgent.Evaluate(
            new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId
                },
                commands));
    }
    
    public virtual Task<IResult<IEventEnvelope[]>> Trigger(
        ICommandMetadata metadata,
        params ICommand[] commands)
    {
        using var evaluationAgent = _evaluationAgent;
        
        return evaluationAgent.Invoke(
            new CommandEnvelope(
                metadata,
                commands));
    }
    
    public virtual Task<IResult<IEventEnvelope[]>> Trigger(
        string aggregateId,
        params ICommand[] commands)
    {
        using var evaluationAgent = _evaluationAgent;

        return evaluationAgent.Invoke(
            new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                commands));
    }
    
    public virtual Task<IResult<IEventEnvelope[]>> Trigger(
        IService service,
        IServiceMetadata? metadata = null)
    {
        using var evaluationAgent = _evaluationAgent;

        return evaluationAgent.Invoke(
            new ServiceEnvelope(
                metadata ?? new ServiceMetadata
                {
                    CreatedAt = DateTimeOffset.UtcNow
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
