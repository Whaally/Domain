using FluentResults;
using Whaally.Domain.Abstractions;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Whaally.Domain.Tests.Domain;

public class TestParentService : IService
{
    public Guid Id1 { get; init; } = Guid.NewGuid();
    public Guid Id2 { get; init; } = Guid.NewGuid();
}

public class TestParentServiceHandler : IServiceHandler<TestParentService>
{
    public async Task<IServiceResult> Invoke(IServiceHandlerContext context, TestParentService service)
    {
        return new Service()
            .Invoke(new TestService { Id = service.Id1 })
            .Invoke(new TestService { Id = service.Id2 });
    }
}