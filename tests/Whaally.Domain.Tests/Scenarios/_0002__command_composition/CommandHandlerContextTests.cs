using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

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
                .AddDomain()
                .BuildServiceProvider(),
            "")
        {
        };
    
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
        new TestCommandHandler().Evaluate(Context, new TestCommand());
        
        new Result()
            .WithReasons(Context.Result.Reasons)
            .IsSuccess
            .Should().BeTrue();
    }
}
