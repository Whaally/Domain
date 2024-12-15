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
    private readonly IContextFactory _contextFactory;
    private readonly IEvaluationAgent _evaluationAgent;

    private Activity? _activity = null;
    
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
        _contextFactory = _services.GetRequiredService<IContextFactory>();
        _evaluationAgent = _services.GetRequiredService<IEvaluationAgent>();
        
        Id = id;
        
        _aggregate ??= services
            .GetRequiredService<IAggregateFactory>()
            .Instantiate<TAggregate>();
    }

    public virtual Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope) =>
        Evaluate(commandEnvelope, null);
    
    public virtual Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope, CancellationToken? cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(commandEnvelope.Metadata.AggregateId)
            && commandEnvelope.Metadata.AggregateId != Id)
            throw new Exception("The provided commands seem intended for a different aggregate instance");
        else if (cancellationToken?.IsCancellationRequested ?? false)
            return Task.FromResult<IResult<IEventEnvelope>>(
                Result.Fail<IEventEnvelope>("Operation was cancelled"));
        
        // TODO: Allow concurrent uses, though queue subsequent operations
        if (_activity != null
            && _activity.ParentSpanId != commandEnvelope.Metadata.ParentContext?.SpanId)
            throw new Exception("Aggregate is locked");

        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"Aggregate {Aggregate.GetType().Name}",
            parentContext: commandEnvelope.Metadata.ParentContext ?? default,
            tags: new Dictionary<string, object?>
            {

            });

        var events = new List<IEvent>();
        var results = new List<IResultBase>();

        TAggregate intermediateState = _aggregate;

        foreach (var cmd in commandEnvelope.Messages)
        {
           /*
            * The following things happen:
            * 1. The aggregate ID is set on the command (must be refactored to remove dependency)
            * 2. Command handler is retrieved, and an appropriate command context instance is created for evaluation
            * 3. The command is evaluated
            * 4. Results are extracted from the context and evaluated against a temporary state of the aggregate.
            * 
            */
            
            var command = cmd;

            _activity?.AddEvent(new ActivityEvent($"Evaluate {command.GetType().Name}"));
            
            if (string.IsNullOrWhiteSpace(commandEnvelope.Metadata.AggregateId)) commandEnvelope.Metadata.AggregateId = Id;

            var commandContext = _contextFactory.CreateCommandHandlerContext(
                intermediateState,
                commandEnvelope.Metadata,
                _activity);
            
            results.Add(_domainContext
                .GetCommandHandler(command.GetType())
                .Evaluate(commandContext, command));

            var intermediateEvents = commandContext.Events.ToList();

            foreach (var intermediateEvent in intermediateEvents)
            {
                if (cancellationToken?.IsCancellationRequested ?? false)
                    return Task.FromResult<IResult<IEventEnvelope>>(
                        Result.Fail<IEventEnvelope>("Operation was cancelled"));
                
                var @event = intermediateEvent;

                var eventContext = _contextFactory.CreateEventHandlerContext(
                    intermediateState,
                    new EventMetadata
                    {
                        AggregateId = Id,
                        AggregateType = Aggregate.GetType(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        Attributes = commandEnvelope.Metadata.Attributes,
                        ParentContext = _activity?.Context
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
                        AggregateId = commandEnvelope.Metadata.AggregateId,
                        AggregateType = Aggregate.GetType(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        Attributes = commandEnvelope.Metadata.Attributes,
                        ParentContext = _activity?.Context
                    },
                    events))
                : result);
    }

    public virtual Task<IResultBase> Apply(IEventEnvelope eventEnvelope) => Apply(eventEnvelope, null);
    
    public virtual async Task<IResultBase> Apply(IEventEnvelope eventEnvelope, CancellationToken? cancellationToken)
    {
        if (!eventEnvelope.Messages.Any())
        {
            _activity?.Dispose();
            _activity = null;
            
            return Result.Ok();
        }
        else if (cancellationToken?.IsCancellationRequested ?? false)
            return Result.Fail("Operation was cancelled");

        TAggregate intermediateState = _aggregate;

        foreach (var @event in eventEnvelope.Messages)
        {
            _activity?.AddEvent(new ActivityEvent($"Apply {@event.GetType().Name}"));
            
            var eventHandler = _domainContext.GetEventHandler(@event.GetType());
            
            intermediateState = eventHandler.Apply(
                _contextFactory.CreateEventHandlerContext(
                    intermediateState,
                    eventEnvelope.Metadata),
                @event);
            
            if (cancellationToken?.IsCancellationRequested ?? false)
                return Result.Fail("Operation was cancelled");
        }

        _aggregate = intermediateState;

        // Implicitly continue the operations
        await _evaluationAgent.Continue(eventEnvelope);

        _activity?.Dispose();
        _activity = null;
        
        return Result.Ok();
    }

    public virtual Task Abort(IMessageMetadata metadata)
    {
        _activity?.AddEvent(new ActivityEvent("Abort"));
        _activity?.Dispose();
        _activity = null;
        
        return Task.CompletedTask;
    }

    public virtual Task<TSnapshot> Snapshot<TSnapshot>()
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
