using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain.TodoItem;

public record TodoItemAggregate : IAggregate
{
    public string? item { get; init; }
    public bool Completed { get; init; }
};

public record CreateTodoItem(string item) : ICommand;
public record SetCompletion(bool completed) : ICommand;

public class CreateTodoItemHandler : ICommandHandler<TodoItemAggregate, CreateTodoItem>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TodoItemAggregate> context, CreateTodoItem command)
    {
        return Command
            .Stage(new TodoItemCreated(command.item))
            .Invoke(new SetCompletion(false));
    }
}

public class SetCompletionHandler : ICommandHandler<TodoItemAggregate, SetCompletion>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TodoItemAggregate> context, SetCompletion command)
    {
        return Command.Stage(new CompletionSet(command.completed));
    }
}

public record TodoItemCreated(string Item) : IEvent;
public record CompletionSet(bool Completed) : IEvent;

public class TodoItemCreatedHandler : IEventHandler<TodoItemAggregate, TodoItemCreated>
{
    public TodoItemAggregate Apply(IEventHandlerContext<TodoItemAggregate> context, TodoItemCreated @event)
        => context.Aggregate with
        {
            item = @event.Item
        };
}

public class CompletionHandler : IEventHandler<TodoItemAggregate, CompletionSet>
{
    public TodoItemAggregate Apply(IEventHandlerContext<TodoItemAggregate> context, CompletionSet @event)
        => context.Aggregate with
        {
            Completed = @event.Completed
        };
}
