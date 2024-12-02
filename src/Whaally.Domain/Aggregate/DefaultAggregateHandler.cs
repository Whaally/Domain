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
        _evaluationAgent = services.GetRequiredService<IEvaluationAgent>();
        
        Id = id;
        
        _aggregate ??= services
            .GetRequiredService<IAggregateFactory>()
            .Instantiate<TAggregate>();
    }
    
    public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands)
    {
        var events = new List<IEventEnvelope>(commands.Length);
        var results = new List<IResultBase>();

        TAggregate intermediateState = _aggregate;

        // ToDo: Check whether the commands have in fact been intended for the present aggregate

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

            // ToDo: provide more helpful exception methods
            var commandHandler = (ICommandHandler)_services.GetRequiredService(
                _domainContext.CommandHandlers
                    .Single(q => q.AggregateType == typeof(TAggregate) 
                                 && q.CommandType == command.Message.GetType())
                    .HandlerType);
            
            var commandContext = new CommandHandlerContext<TAggregate>(
                _services,
                !string.IsNullOrWhiteSpace(command.Metadata.AggregateId)
                    ? command.Metadata.AggregateId
                    : Id)
            {
                Aggregate = intermediateState
            };

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

        return Task.FromResult<IResult<IEventEnvelope[]>>(
            result.IsSuccess
                ? result.ToResult(events.ToArray())
                : result);
    }

    public async Task<IResultBase> Apply(params IEventEnvelope[] events)
    {
        if (events == null) return Result.Ok();

        TAggregate intermediateState = _aggregate;

        foreach (var @event in events)
        {
            var eventHandler = (IEventHandler)_services.GetRequiredService(
                _domainContext.EventHandlers
                    .Single(q => q.AggregateType == typeof(TAggregate)
                                 && q.EventType == @event.Message.GetType())
                    .HandlerType);

            var eventContext = new EventHandlerContext<TAggregate>(
                !string.IsNullOrWhiteSpace(@event.Metadata.AggregateId)
                    ? @event.Metadata.AggregateId
                    : Id)
            {
                Aggregate = intermediateState
            };

            intermediateState = eventHandler.Apply(
                eventContext,
                @event.Message);
        }

        _aggregate = intermediateState;

        await Continue(events);
        
        return Result.Ok();
    }
    
    /*
     * Note that in this method the continuation happens sequentially, and is awaited.
     * In production environments the confirm method should likely return after the changes had been applied
     * to free up the aggregate handler for other operations.
     */
    public async Task<IResultBase> Continue(params IEventEnvelope[] events)
    {
        foreach (var @event in events)
        {
            // ToDo: Ensure these sagas are properly evaluated in the background
            await Task.Run(async () =>
            {
                var commands = await _evaluationAgent.EvaluateSaga(@event);

                if (commands.IsFailed) return;

                var events = await _evaluationAgent.EvaluateCommands(commands.Value);

                if (events.IsFailed) return;

                await _evaluationAgent.EvaluateEvents(events.Value);
            });
        }

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