using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain.TodoList;

public record TodoListAggregate : IAggregate
{
    public List<Guid> Items { get; init; } = [];
}
