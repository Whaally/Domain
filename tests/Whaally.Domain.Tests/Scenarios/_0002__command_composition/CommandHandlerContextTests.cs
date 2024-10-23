using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

public class CommandHandlerContextTests
{
    /*
     * This test is to assert a command handler can invoke another command inline.
     *
     * Doing so immediately invokes the specified command, and add its output to the output of the current handler.
     */
    
    public ICommandHandlerContext<Aggregate> Context 
        = new CommandHandlerContext<Aggregate>(
            new ServiceCollection()
                .AddSingleton<IAggregate, Aggregate>()
                // We're abusing the DI system a bit here.
                // ToDo: Build a different and more elegant mechanism to keep track of handler implementations etc.
                .AddTransient<ICommandHandler, TestCommandHandler>()
                .AddTransient<ICommandHandler, AnotherCommandHandler>()
                .AddTransient<ICommandHandler<Aggregate, TestCommand>, TestCommandHandler>()
                .AddTransient<ICommandHandler<Aggregate, AnotherCommand>, AnotherCommandHandler>()
                .AddTransient<IEventHandler, TestEventHandler>()
                .AddTransient<IEventHandler<Aggregate, TestEvent>, TestEventHandler>()
                .BuildServiceProvider(), 
            "");
    
    [Fact]
    public void CanInvokeCommand()
        => Context.EvaluateCommand(new TestCommand());
    
    [Fact]
    public void ContextContainsCommands()
    {
        Context.EvaluateCommand(new TestCommand());
        Context.Events.Should().NotBeEmpty();
    }

    [Fact]
    public void RunFromHandler()
    {
        new TestCommandHandler()
            .Evaluate(Context, new TestCommand())
            .IsSuccess.Should().BeTrue();
    }
}
