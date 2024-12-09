using ConsoleSample.Domain.TodoItem;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public class TodoItemAddedSaga : ISaga<TodoItemCreated>
{
    public Task<IResultBase> Evaluate(ISagaContext context, TodoItemCreated @event)
    {
        return Task.FromResult<IResultBase>(Result.Ok());
    }
}
