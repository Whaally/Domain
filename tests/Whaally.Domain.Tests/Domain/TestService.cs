using FluentResults;
using Whaally.Domain.Abstractions;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Whaally.Domain.Tests.Domain;

internal class TestService : IService
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
}

internal class TestServiceHandler : IServiceHandler<TestService>
{
    public async Task Invoke(IServiceHandlerContext context, TestService service)
    {
        context.StageCommands(
            service.Id,
            new TestCommand());
    }
}
