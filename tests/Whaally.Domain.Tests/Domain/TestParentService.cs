using FluentResults;
using Whaally.Domain.Abstractions;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Whaally.Domain.Tests.Domain;

public class TestParentService : IService
{
    public string Id1 { get; init; } = Guid.NewGuid().ToString();
    public string Id2 { get; init; } = Guid.NewGuid().ToString();
}

public class TestParentServiceHandler : IServiceHandler<TestParentService>
{
    public async Task<IResult> Invoke(IServiceHandlerContext context, TestParentService service)
    {
        await context.InvokeService(new TestService { Id = service.Id1 });
        await context.InvokeService(new TestService { Id = service.Id2 });
    }
}