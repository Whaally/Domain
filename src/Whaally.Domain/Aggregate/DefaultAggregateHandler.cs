using System.Diagnostics;
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
    
    public Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope)
    {
        // ToDo: Check whether the commands have in fact been intended for the present aggregate
        
        var events = new List<IEvent>();
        var results = new List<IResultBase>();

        TAggregate intermediateState = _aggregate;

        foreach (var cmd in commandEnvelope.Messages)
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
            
            ICommand command = cmd;

            if (string.IsNullOrWhiteSpace(commandEnvelope.Metadata.AggregateId)) commandEnvelope.Metadata.AggregateId = Id;
            // ToDo: Assert we are not executing commands not meant for this instance.

            // ToDo: provide more helpful exception methods
            var commandHandler = (ICommandHandler)_services.GetRequiredService(
                _domainContext.CommandHandlers
                    .Single(q => q.AggregateType == typeof(TAggregate) 
                                 && q.CommandType == command.GetType())
                    .HandlerType);

            var commandContext = _contextFactory.CreateCommandHandlerContext(
                intermediateState,
                commandEnvelope.Metadata);
            
            results.Add(commandHandler.Evaluate(commandContext, command));

            var intermediateEvents = commandContext.Events.ToList();

            foreach (var intermediateEvent in intermediateEvents)
            {
                IEvent @event = intermediateEvent;

                var eventContext = _contextFactory.CreateEventHandlerContext(
                    intermediateState,
                    new EventMetadata
                    {
                        AggregateId = Id
                    });

                intermediateState = _domainContext
                    .GetEventHandler(@event.GetType())
                    .Apply(eventContext, @event);

                events.Add(@event);
            }
        }
        
        var result = Result.Ok().WithReasons(results.SelectMany(result => result.Reasons));
        
        return Task.FromResult<IResult<IEventEnvelope>>(
            result.IsSuccess
                ? result.ToResult(new EventEnvelope(
                    new EventMetadata
                    {
                        Attributes = commandEnvelope.Metadata.Attributes,
                        AggregateId = commandEnvelope.Metadata.AggregateId,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    events))
                : result);
    }
    
    public async Task<IResultBase> Apply(IEventEnvelope eventEnvelope)
    {
        if (!eventEnvelope.Messages.Any()) return Result.Ok();

        TAggregate intermediateState = _aggregate;

        foreach (var @event in eventEnvelope.Messages)
        {
            // ToDo: Assert whether the events are intended to be applied to this aggregate instance.

            var eventHandler = _domainContext.GetEventHandler(@event.GetType());
            
            intermediateState = eventHandler.Apply(
                _contextFactory.CreateEventHandlerContext(
                    intermediateState,
                    eventEnvelope.Metadata),
                @event);
        }

        _aggregate = intermediateState;

        // Implicitly continue the operations
        await _evaluationAgent.Continue(eventEnvelope);
        
        return Result.Ok();
    }

    public Task Abort(ActivityContext context)
    {
        throw new NotImplementedException();
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
