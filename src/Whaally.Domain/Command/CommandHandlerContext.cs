using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class CommandHandlerContext<TAggregate> : ICommandHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    private TAggregate _aggregate = null!;
    
    [Obsolete]
    private List<IEvent> _events = [];
    
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IContextFactory _contextFactory;
    private readonly Activity? _activity;
    
    public CommandHandlerContext(
        IServiceProvider services,
        string aggregateId,
        Activity? activity = null)
    {
        _services = services;
        _domainContext = services.GetRequiredService<DomainContext>();
        _contextFactory = services.GetRequiredService<IContextFactory>();
        _activity = activity;
        
        AggregateId = aggregateId;
        
        // Stuff like this would require me to rethink what I am doing.
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        _aggregate ??= services
            .GetService<IAggregateFactory>()
            ?.Instantiate<TAggregate>() ?? null!;
    }

    public string AggregateId { get; init; }
    public string? TransactionId { get; init; }
    public IResultBase Result { get; init; } = new Result();
    
    [Obsolete]
    public void WithResult(IResultBase result)
    {
        Result.Reasons.AddRange(result.Reasons);

        // Ensure no events are returned in case of an error
        if (Result.IsFailed)
            _events.Clear();
        
        _activity?.AddEvent(new ActivityEvent($"Evaluation failed"));
    }

    public ActivityContext? ParentContext { get; init; }
    public IReadOnlyDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
    [Obsolete]
    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
    
    public TAggregate Aggregate
    {
        get => _aggregate; 
        init => _aggregate = value;
    }
    
    [Obsolete]
    public virtual void StageEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent
    {
        // Early return.
        if (Result.IsFailed) return;
        
        _activity?.AddEvent(new ActivityEvent($"Stage {typeof(TEvent).Name}"));
        
        _aggregate = _domainContext
            .GetEventHandler(@event.GetType())
            .Apply(
                _contextFactory.CreateEventHandlerContext(
                    Aggregate,
                    new EventMetadata
                    {
                        AggregateId = AggregateId,
                        AggregateType = Aggregate.GetType(),
                        CreatedAt = DateTimeOffset.UtcNow,
                        Attributes = new Dictionary<string, object>(Attributes),
                        ParentContext = ParentContext,
                        TransactionId = TransactionId
                    }),
                @event);
        
        _events.Add(@event);
    }
    
    [Obsolete]
    public virtual void EvaluateCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        _activity?.AddEvent(new ActivityEvent($"Evaluate {typeof(TCommand).Name}"));
        
        // Note that we're explicitly isolating the invocation of this command such that there is no mixup between
        // staged events, or there is otherwise a trace of this command being called by another command.
        var context = _contextFactory
            .CreateCommandHandlerContext(
                _aggregate, 
                new CommandMetadata
                {
                    AggregateId = AggregateId,
                    AggregateType = _aggregate.GetType(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    Attributes = new Dictionary<string, object>(Attributes),
                    ParentContext = ParentContext,
                    TransactionId = TransactionId
                },
                _activity);

        _domainContext
            .GetCommandHandler(command.GetType())
            .Evaluate(context, command);
        
        // ToDo: we have the input (command), and the output (context.result). Can we map the results in such way that it is clear what had happened?
        WithResult(context.Result);
        
        foreach (var @event in context.Events)
        {
            _aggregate = _domainContext
                .GetEventHandler(@event.GetType())
                .Apply(
                    _contextFactory.CreateEventHandlerContext(
                        Aggregate,
                        new EventMetadata
                        {
                            AggregateId = AggregateId,
                            AggregateType = Aggregate.GetType(),
                            CreatedAt = DateTimeOffset.UtcNow,
                            Attributes = new Dictionary<string, object>(Attributes),
                            ParentContext = ParentContext,
                            TransactionId = TransactionId
                        }),
                    @event);
            
            // Only add events in case evaluation is successful
            // Otherwise merely continue evaluation to collect the failures
            // Doing so could be potentially dangerous as information exposure
            if (Result.IsSuccess)
                _events.Add(@event);
        }
    }
}
