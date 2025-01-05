using ConsoleSample.Domain.TodoItem;
using ConsoleSample.Domain.TodoList.Commands;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public class AddItemService : IService;

public class AddItemServiceHandler : IServiceHandler<AddItemService>
{
    public async Task Invoke(IServiceHandlerContext context, AddItemService service)
    {
        var todoItem = Guid.NewGuid();
        
        context.StageCommands(
            todoItem.ToString(), 
            new CreateTodoItem("do a thing"));

        await context.InvokeService(new NestedService());
        
        context.StageCommands(
            Guid.NewGuid().ToString(),
            new AddItem(todoItem));
    }
}