using System.Diagnostics;
using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;

namespace Whaally.Domain.Command;

public class CommandHandlerContext<TAggregate>(IServiceProvider services, string aggregateId)
    : ICommandHandlerContext<TAggregate>
    where TAggregate : class, IAggregate, new()
{
    public IReadOnlyCollection<IEventEnvelope> Events => _events.AsReadOnly();
    
    private List<IEventEnvelope> _events = new List<IEventEnvelope>();

    public TAggregate Aggregate { get; init; } = new();
    public ActivityContext Activity { get; init; }
    public string AggregateId { get; init; } = aggregateId;

    public void StageEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent
    {
        var envelope = new EventEnvelope<TEvent>(
            @event,
            new EventMetadata(AggregateId)
            {
                SourceActivity = Activity
            });

        _events.Add(envelope);
    }

    public IResultBase EvaluateCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        var handler = services.GetCommandHandlerForCommand<TCommand>();

        // ToDo: Assert the aggregate types of the command and this context do match.

        // Note that we're explicitly isolating the invocation of this command such that there is no mixup between
        // staged events, or there is otherwise a trace of this command being called by another command.
        var context = new CommandHandlerContext<TAggregate>(services, AggregateId);
        
        var result = handler.Evaluate(context, command);
        
        if (result.IsSuccess)
            _events.AddRange(context.Events);
        
        return result;
    }
}
