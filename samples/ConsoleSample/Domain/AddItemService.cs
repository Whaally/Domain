using ConsoleSample.Domain.TodoItem;
using ConsoleSample.Domain.TodoList.Commands;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public class AddItemService : IService;

public class AddItemServiceHandler : IServiceHandler<AddItemService>
{
    public Task<IServiceResult> Invoke(IServiceHandlerContext context, AddItemService service)
    {
        var todoItem = Guid.NewGuid();

        return Task.FromResult(
            new ServiceResult()
                .Stage(
                    todoItem, 
                    new CreateTodoItem("do a thing"))
                .Invoke(new NestedService())
                .Stage(
                    Guid.NewGuid(),
                    new AddItem(todoItem)));
    }
}