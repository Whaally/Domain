using ConsoleSample.Domain.TodoItem;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public class TodoItemAddedSaga : ISaga<TodoItemCreated>
{
    public Task<IResultBase> Evaluate(ISagaContext context, TodoItemCreated @event)
    {
        context.EvaluateService(new NestedService());
        
        return Task.FromResult<IResultBase>(Result.Ok());
    }
}
