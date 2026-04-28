using ConsoleSample.Domain.TodoList.Events;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain.TodoList.Commands;

public record AddItem(Guid id) : ICommand;

public class AddItemHandler : ICommandHandler<TodoListAggregate, AddItem>
{
    public ICommandResult Evaluate(ICommandHandlerContext<TodoListAggregate> context,
        AddItem command) =>
        Command.Stage(new ItemAdded(command.id));
}
