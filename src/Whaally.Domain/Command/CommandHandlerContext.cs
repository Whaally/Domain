using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class CommandHandlerContext<TAggregate> : ICommandHandlerContext<TAggregate>
    where TAggregate : class, IAggregate
{
    private TAggregate _aggregate = null!;
    private List<IEvent> _events = [];
    
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IContextFactory _contextFactory;
    
    public CommandHandlerContext(
        IServiceProvider services,
        string aggregateId)
    {
        _services = services;
        _domainContext = services.GetRequiredService<DomainContext>();
        _contextFactory = services.GetRequiredService<IContextFactory>();
        
        AggregateId = aggregateId;
        
        // Stuff like this would require me to rethink what I am doing.
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        _aggregate ??= services
            .GetService<IAggregateFactory>()
            ?.Instantiate<TAggregate>() ?? null!;
    }

    public string AggregateId { get; init; }
    public ActivityContext? ParentContext { get; init; }
    
    public IReadOnlyDictionary<string, object> Attributes { get; init; } 
        = new Dictionary<string, object>();
    
    public IReadOnlyCollection<IEvent> Events 
        => _events.AsReadOnly();
    
    public TAggregate Aggregate
    {
        get => _aggregate; 
        init => _aggregate = value;
    }
    
    public virtual void StageEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent => _events.Add(@event);
    
    public virtual IResultBase EvaluateCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {   
        // ToDo: Assert the aggregate types of the command and this context do match.

        // Note that we're explicitly isolating the invocation of this command such that there is no mixup between
        // staged events, or there is otherwise a trace of this command being called by another command.
        var context = _contextFactory
            .CreateCommandHandlerContext(
                _aggregate, 
                new CommandMetadata
                {
                    AggregateId = AggregateId,
                    CreatedAt = DateTimeOffset.UtcNow
                });
        
        var result = _domainContext
            .GetCommandHandler(command.GetType())
            .Evaluate(context, command);
        
        if (!result.IsSuccess) return result;
        
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
                            Attributes = new Dictionary<string, object>(Attributes),
                            ParentContext = ParentContext
                        }),
                    @event);
            
            _events.Add(@event);
        }
        
        _events.AddRange(context.Events);
        
        return result;
    }
}
