using ConsoleSample.Domain.TodoItem;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace ConsoleSample.Domain;

public class TodoItemAddedSaga : ISaga<TodoItemCreated>
{
    public async Task<ISagaResult> Evaluate(ISagaContext context, TodoItemCreated @event) 
        => new SagaResult().Invoke(new NestedService());
}
