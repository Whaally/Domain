using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultAggregateHandler<TAggregate> : IAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IEvaluationAgent _evaluationAgent;
    private readonly IContextFactory _contextFactory;
    
    private TAggregate _aggregate;
    public TAggregate Aggregate
    {
        get => _aggregate;
        init => _aggregate = value;
    }
    
    public string Id { get; init; }

    // ToDo: Use an options pattern to supply mandatory/optional parameters
    public DefaultAggregateHandler(IServiceProvider services, string id)
    {
        _services = services;
        _domainContext = _services.GetRequiredService<DomainContext>();
        _evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();
        _contextFactory = _services.GetRequiredService<IContextFactory>();
        
        Id = id;
        
        _aggregate ??= services
            .GetRequiredService<IAggregateFactory>()
            .Instantiate<TAggregate>();
    }
    
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands)
    {
        // ToDo: Check whether the commands have in fact been intended for the present aggregate
        
        var events = new List<IEventEnvelope>(commands.Length);
        var results = new List<IResultBase>();

        TAggregate intermediateState = _aggregate;

        foreach (var cmd in commands)
        {
            // ToDo: Extract the command handler instantiation to some other component
           /*
            * The following things happen:
            * 1. The aggregate ID is set on the command (must be refactored to remove dependency)
            * 2. Command handler is retrieved, and an appropriate command context instance is created for evaluation
            * 3. The command is evaluated
            * 4. Results are extracted from the context and evaluated against a temporary state of the aggregate.
            * 
            */
            
            ICommandEnvelope command = cmd;

            if (string.IsNullOrWhiteSpace(command.Metadata.AggregateId)) command.Metadata.AggregateId = Id;
            // ToDo: Assert we are not executing commands not meant for this instance.

            // ToDo: provide more helpful exception methods
            var commandHandler = (ICommandHandler)_services.GetRequiredService(
                _domainContext.CommandHandlers
                    .Single(q => q.AggregateType == typeof(TAggregate) 
                                 && q.CommandType == command.Message.GetType())
                    .HandlerType);

            var commandContext = _contextFactory.CreateCommandHandlerContext(
                intermediateState,
                command.Metadata);
            
            results.Add(commandHandler.Evaluate(commandContext, command.Message));

            var intermediateEvents = commandContext.Events.ToList();

            foreach (var intermediateEvent in intermediateEvents)
            {
                IEventEnvelope @event = intermediateEvent;

                var eventHandler = (IEventHandler)_services.GetRequiredService(
                    _domainContext.EventHandlers
                        .Single(q => q.AggregateType == typeof(TAggregate)
                                     && q.EventType == @event.Message.GetType())
                        .HandlerType);
                
                var eventContext = new EventHandlerContext<TAggregate>(
                    !string.IsNullOrWhiteSpace(command.Metadata.AggregateId)
                        ? command.Metadata.AggregateId
                        : Id)
                {
                    Aggregate = intermediateState
                };

                intermediateState = eventHandler
                    .Apply(eventContext, @event.Message);

                events.Add(@event);
            }
        }
        
        var result = Result.Ok().WithReasons(results.SelectMany(result => result.Reasons));
        
        return Task.FromResult<IResult<IEventEnvelope[]>>(result.IsSuccess
                ? result.ToResult(events.ToArray())
                : result);
    }
    
    public async Task<IResultBase> Apply(params IEventEnvelope[] events)
    {
        if (events == null) return Result.Ok();

        TAggregate intermediateState = _aggregate;

        foreach (var @event in events)
        {
            // ToDo: Assert whether the events are intended to be applied to this aggregate instance.
            
            var eventHandler = (IEventHandler)_services.GetRequiredService(
                _domainContext.EventHandlers
                    .Single(q => q.AggregateType == typeof(TAggregate)
                                 && q.EventType == @event.Message.GetType())
                    .HandlerType);

            var eventContext = _contextFactory.CreateEventHandlerContext(
                intermediateState,
                @event.Metadata);
            
            intermediateState = eventHandler.Apply(
                eventContext,
                @event.Message);
        }

        _aggregate = intermediateState;

        // Implicitly continue the operations
        foreach (var @event in events)
            await _evaluationAgent.Continue(@event);
        
        return Result.Ok();
    }

    public Task<TSnapshot> Snapshot<TSnapshot>()
        where TSnapshot : ISnapshot =>
        Task.FromResult(
            ((ISnapshotFactory<TAggregate, TSnapshot>)_services.GetRequiredService(
                _domainContext
                    .SnapshotFactories
                    .Single(q => q.AggregateType == typeof(TAggregate)
                                 && q.SnapshotType == typeof(TSnapshot))
                    .FactoryType))
            .Instantiate(_aggregate));
}
