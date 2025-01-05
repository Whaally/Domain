using ConsoleSample.Domain.TodoItem;
using FluentResults;
using Whaally.Domain.Abstractions;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace ConsoleSample.Domain;

public class TodoItemAddedSaga : ISaga<TodoItemCreated>
{
    public async IAsyncEnumerable<IReason> Evaluate(ISagaContext context, TodoItemCreated @event)
    {
        await context.EvaluateService(new NestedService());

        yield break;
    }
}
