using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain.TodoList.Events;

public record ItemAdded(Guid id) : IEvent;

public class ItemAddedHandler : IEventHandler<TodoListAggregate, ItemAdded>
{
    public TodoListAggregate Apply(IEventHandlerContext<TodoListAggregate> context, ItemAdded @event)
    {
        var list = context.Aggregate.Items;
        
        list.Add(@event.id);
        
        return context.Aggregate with
        {
            Items = list
        };
    }
}
