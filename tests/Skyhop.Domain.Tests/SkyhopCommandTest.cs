using FluentAssertions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Tests;

namespace Skyhop.Domain.Tests;

public abstract class SkyhopCommandTest<TAggregate, TCommand> : CommandTest<TAggregate, TCommand>
    where TAggregate : class, IAggregate, new()
    where TCommand : class, ICommand
{
    public SkyhopCommandTest(
        ICommandHandler<TAggregate, TCommand> handler, 
        TAggregate aggregate, 
        TCommand command) : base(handler, aggregate, command) { }
    
    [Fact]
    public void AssertCommandSerializerIsGenerated()
    {
        typeof(TCommand).Should().BeDecoratedWith<GenerateSerializerAttribute>();
    }

    [Fact]
    public void AssertCommandIsMarkedImmutable()
    {
        typeof(TCommand).Should().BeDecoratedWith<ImmutableAttribute>();
    }
}