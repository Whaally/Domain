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
public class DomainContext
{
    internal static ActivitySource ActivitySource = new("Whaally.Domain");
    
    private readonly IServiceProvider _services;
    private readonly Activity? _activity;

        });
    
    public DomainContext(
        IServiceProvider services,
        IEnumerable<Type>? commandHandlerTypes = null,
        IEnumerable<Type>? eventHandlerTypes = null,
        IEnumerable<Type>? serviceHandlerTypes = null,
        IEnumerable<Type>? sagaTypes = null,
        IEnumerable<Type>? snapshotFactoryTypes = null)
    {
        _services = services;

        CommandHandlers = (commandHandlerTypes ?? [])
            .Select(CommandHandlerMeta.From)
            .ToList()
            .AsReadOnly();
        
        EventHandlers = (eventHandlerTypes ?? [])
            .Select(EventHandlerMeta.From)
            .ToList()
            .AsReadOnly();
        
        ServiceHandlers = (serviceHandlerTypes ?? [])
            .Select(ServiceHandlerMeta.From)
            .ToList()
            .AsReadOnly();
        
        Sagas = (sagaTypes ?? [])
            .Select(SagaMeta.From)
            .ToList()
            .AsReadOnly();
        
        SnapshotFactories = (snapshotFactoryTypes ?? [])
            .Select(SnapshotFactoryMeta.From)
            .ToList()
            .AsReadOnly();
    }
    
    #region handler metadata
    /// <summary>
    ///     Metadata about the command handlers registered with this domain instance.
    /// </summary>
    public IReadOnlyList<CommandHandlerMeta> CommandHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the event handlers registered with this domain.
    /// </summary>
    public IReadOnlyList<EventHandlerMeta> EventHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the service handlers registered with this domain.
    /// </summary>
    public IReadOnlyList<ServiceHandlerMeta> ServiceHandlers { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the sagas registered with this domain.
    /// </summary>
    public IReadOnlyList<SagaMeta> Sagas { get; private init; } = [];
    
    /// <summary>
    ///     Metadata about the snapshot factories registered with this domain.
    /// </summary>
    public IReadOnlyList<SnapshotFactoryMeta> SnapshotFactories { get; private init; } = [];
    #endregion
    
    private IEvaluationAgent _evaluationAgent => _services.GetRequiredService<IEvaluationAgent>();
    
    private IAggregateHandler GetAggregate(Type type, string id)
    {
        var aggregateHandlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();
        
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
                CreatedAt = DateTimeOffset.UtcNow,
                ParentContext = _activity?.Context
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
                    AggregateId = aggregateId,
                    ParentContext = _activity?.Context
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
                    CreatedAt = DateTimeOffset.UtcNow,
                    ParentContext = _activity?.Context
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
                    CreatedAt = DateTimeOffset.UtcNow,
                    ParentContext = _activity?.Context
                }, service));
    }
    
    public IServiceHandler GetServiceHandler(Type serviceType) => 
        (IServiceHandler)_services.GetRequiredService(
            ServiceHandlers
                .Single(q => q.ServiceType == serviceType)
                .HandlerType);
    
    public IEnumerable<ISaga> GetSaga(Type eventType) =>
        Sagas.Where(q => q.EventType == eventType)
            .Select(q => (ISaga)_services.GetRequiredService(q.HandlerType));

    public IEventHandler GetEventHandler(Type eventType) =>
        (IEventHandler)_services.GetRequiredService(
            EventHandlers
                .Single(q => q.EventType == eventType)
                .HandlerType);

    // TODO: See if we can also require the aggregate type as argument to validate that we're executing the correct handlers
    //       Same goes for the event handlers though.
    public ICommandHandler GetCommandHandler(Type commandType) =>
        (ICommandHandler)_services.GetRequiredService(
            CommandHandlers
                .Single(q => q.CommandType == commandType)
                .HandlerType);
}
