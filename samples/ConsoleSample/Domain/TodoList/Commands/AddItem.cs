using ConsoleSample.Domain.TodoList.Events;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain.TodoList.Commands;

public record AddItem(Guid id) : ICommand;

public class AddItemHandler : ICommandHandler<TodoListAggregate, AddItem>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TodoListAggregate> context,
        AddItem command) =>
        context.StageEvent(new ItemAdded(command.id));
}
