using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class CommandHandlerContext<TAggregate> : ICommandHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    public IReadOnlyCollection<IEventEnvelope> Events => _events.AsReadOnly();
    
    private List<IEventEnvelope> _events = [];
    
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;

    private readonly Activity? _activity;
    
    public CommandHandlerContext(
        IServiceProvider services, 
        string aggregateId)
    {
        _services = services;
        _domainContext = services.GetRequiredService<DomainContext>();
        AggregateId = aggregateId;
        
        // Stuff like this would require me to rethink what I am doing.
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        _aggregate ??= services
            .GetService<IAggregateFactory>()
            ?.Instantiate<TAggregate>() ?? null!;
        
        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"evaluate {typeof(TAggregate).Name}",
            parentContext: ParentContext ?? default,
            tags: new Dictionary<string, object?>
            {
                // { "messaging.batch.message_count", operation.commands.Length },
                // { "messaging.batch.types", $"[{string.Join(';', operation.commands.Select(q => q.Message.GetType().FullName))}]" },
                { "messaging.operation.name", "evaluate" },
                { "messaging.operation.type", "process" },
                // { "messaging.destination.id", operation.aggregateId },
                // { "messaging.destination.name", operation.aggregateType.FullName }
            });
    }

    private TAggregate _aggregate = null!;
    public TAggregate Aggregate
    {
        get => _aggregate; 
        init => _aggregate = value;
    }

    public IDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
    public ActivityContext? ParentContext { get; init; }
    public string AggregateId { get; init; }
    
    public virtual void StageEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent
    {
        var envelope = new EventEnvelope<TEvent>(
            @event,
            new EventMetadata
            {
                AggregateId = AggregateId
            });
        
        _events.Add(envelope);
    }
    
    public virtual IResultBase EvaluateCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        var commandHandler = (ICommandHandler) _services.GetRequiredService(
            _domainContext.CommandHandlers
                .Single(q => q.CommandType == typeof(TCommand))
                .HandlerType);
        
        // ToDo: Assert the aggregate types of the command and this context do match.

        // Note that we're explicitly isolating the invocation of this command such that there is no mixup between
        // staged events, or there is otherwise a trace of this command being called by another command.
        var context = _services.GetRequiredService<IContextFactory>()
            .CreateCommandHandlerContext(_aggregate, new CommandMetadata
            {
                AggregateId = AggregateId,
                CreatedAt = DateTimeOffset.UtcNow
            });
        
        var result = commandHandler.Evaluate(context, command);

        if (result.IsSuccess)
        {
            foreach (var eventEnvelope in context.Events)
            {
                var @event = eventEnvelope.Message;
                _aggregate =
                    ((IEventHandler)_services.GetRequiredService(
                        _domainContext.EventHandlers
                            .Single(q => q.EventType == @event.GetType())
                            .HandlerType))
                    .Apply(new EventHandlerContext<TAggregate>(AggregateId)
                    {
                        Aggregate = Aggregate,
                        ParentContext = ParentContext
                    }, @event);
                
                _events.Add(eventEnvelope);
            }
            
            _events.AddRange(context.Events);
        }

        return result;
    }
}
