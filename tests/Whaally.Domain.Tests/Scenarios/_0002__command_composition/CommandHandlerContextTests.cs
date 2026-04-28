using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios._0002__command_composition;

[Obsolete] // todo: remove
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
            Guid.Empty)
        {
        };
}
