using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Command;
using Whaally.Domain.Service;

namespace Whaally.Domain;

/*
 * DEVELOPMENT NOTE:
 * - Do not inject dependencies to this class (IAggregateHandlerFactory & IEvaluationAgent) as concrete instances.
 *   Doing so causes an infinite loop as this class is often injected as dependency to those.
 */

// ToDo: in this class, create a mapping from operations to the evaluation agent. This way we can already involve it at early stage.
// This allows us to go from large scale to small scale structures. E.g. cluster -> service -> command -> event
public class DomainContext(IServiceProvider services)
{
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

    /*
     * Operations:
     *
     * - GetAggregate => AggregateHandler
     * - Evaluate(Command)
     * - Evaluate(Service)
     * - Preview(Command)
     * - Preview(Service)
     */

    internal IAggregateHandler GetAggregate(Type type, string id)
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

    public IAggregateHandler<TAggregate> GetAggregateHandler<TAggregate>(string id)
        where TAggregate : class, IAggregate 
        => (IAggregateHandler<TAggregate>)GetAggregate(typeof(TAggregate), id);

    public Task<IResult<IEventEnvelope[]>> Evaluate<TCommand>(string aggregateId, TCommand command)
        where TCommand : class, ICommand
        => GetAggregate(typeof(TCommand), aggregateId).Evaluate(command);
    
    
    
    public async Task<IResult<IEventEnvelope[]>> EvaluateCommand<TCommand>(string aggregateId, TCommand command)
        where TCommand : class, ICommand
    {
        var aggregateHandlerFactory = services.GetRequiredService<IAggregateHandlerFactory>();
        
        var aggregateType = CommandHandlers.Single(q => q.CommandType == command.GetType())
            .AggregateType;
        
        if (aggregateType == null) throw new Exception($"Aggregate type could not be resolved for command {command.GetType().FullName}");

        var handler = aggregateHandlerFactory.Instantiate(
            aggregateType,
            aggregateId);

        if (handler == null)
            throw new Exception($"Command handler could not be resolved from command {command.GetType().FullName}");
        
        var result = await handler.Evaluate(
            new CommandEnvelope(
                command, 
                new CommandMetadata
                {
                    AggregateId = aggregateId,
                    Timestamp = DateTime.UtcNow
                }));
        
        if (result.IsFailed) return result;

        await handler.Continue(result.Value);
        
        return result;
    }
    
    public async Task<IResult<IEventEnvelope[]>> EvaluateService<TService>(TService service)
        where TService : class, IService
    {
        var evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
        
        var evalResult = await evaluationAgent.EvaluateService(
            new ServiceEnvelope<TService>(
                service,
                new ServiceMetadata()));

        if (evalResult.IsFailed) 
            return Result.Fail<IEventEnvelope[]>(evalResult.Errors);

        var applyResult = await evaluationAgent.EvaluateCommands(evalResult.Value);

        return applyResult;
    }
}