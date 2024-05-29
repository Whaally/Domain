using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;

namespace Whaally.Domain.Tests;

public abstract class CommandTest<TAggregate, TCommand> : DomainTest
    where TAggregate : class, IAggregate, new()
    where TCommand : class, ICommand
{
    public ICommandHandler<TAggregate, TCommand> Handler { get; } 
    public TAggregate Aggregate { get; }
    public TCommand Command { get; }
    
    public CommandHandlerContext<TAggregate> Context { get; }
    public Result Result { get; private init; }
    public IEnumerable<IEvent> Events { get; private init; }
    
    public CommandTest(
        ICommandHandler<TAggregate, TCommand> handler,
        TAggregate aggregate,
        TCommand command)
    {
        Handler = handler;
        Aggregate = aggregate;
        Command = command;
        
        var id = Guid.NewGuid();
        Context = new CommandHandlerContext<TAggregate>(id.ToString())
        {
            Activity = default,
            Aggregate = Aggregate
        };

        var output = Handler.Evaluate(Context, Command);

        if (output.GetType().GenericTypeArguments.Any())
            Result = new Result().WithReasons(output.Reasons);
        else
            Result = (Result)output;
        
        Events = Context.Events.Select(q => q.Message);
    }

    [Fact]
    public void UponFailureAssertNoEvents()
    {
        if (Result.IsFailed)
        {
            Events.Should().BeEmpty();
        }
    }

    [Fact]
    public void AssertResultDoesNotContainObject()
    {
        Result.GetType().Should().NotBeOfType(typeof(IResult<>));
    }
}
