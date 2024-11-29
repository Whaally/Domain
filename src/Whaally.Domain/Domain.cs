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

[Obsolete("Use the `DomainContext` instead")]
public class Domain : DomainContext
{
    public Domain(IServiceProvider services) : base(services)
    {
    }
}

public class DomainContext
{
    readonly IServiceProvider _services;
    
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
    
    public DomainContext(IServiceProvider services)
    {
        _services = services;
    }

    /*
     * Operations:
     *
     * - GetAggregate => AggregateHandler
     * - Evaluate(Command)
     * - Evaluate(Service)
     * - Preview(Command)
     * - Preview(Service)
     */

    // ReSharper disable once UnusedMember.Global
    public Task<IAggregateHandler<TAggregate>> GetAggregate<TAggregate>(string id)
        where TAggregate : class, IAggregate, new()
    {
        var factory = _services.GetRequiredService<IAggregateHandlerFactory>();
        
        return Task.FromResult(factory.Instantiate<TAggregate>(id));
    }

    public async Task<IResult<IEventEnvelope[]>> EvaluateCommand<TCommand>(string aggregateId, TCommand command)
        where TCommand : class, ICommand
    {
        var factory = _services.GetRequiredService<IAggregateHandlerFactory>();

        var aggregateType = CommandHandlers.Single(q => q.CommandType == command.GetType())
            .AggregateType;
        
        if (aggregateType == null) throw new Exception($"Aggregate type could not be resolved for command {command.GetType().FullName}");

        var handler = factory.Instantiate(
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
        var evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();

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