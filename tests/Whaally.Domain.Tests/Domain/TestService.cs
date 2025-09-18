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
    public async Task<IServiceResult> Invoke(IServiceHandlerContext context, TestService service)
    {
        return Service.Ok().Stage(
            service.Id,
            new TestCommand());
    }
}
