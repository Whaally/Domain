using FluentResults;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain.Tests.Domain
{
    public class TestParentService : IService
    {
        public string? Id1 { get; init; }
        public string? Id2 { get; init; }
    }

    public class TestParentServiceHandler : IServiceHandler<TestParentService>
    {
        public Task<IResultBase> Handle(IServiceHandlerContext context, TestParentService service)
        {
            context.EvaluateService(new TestService { Id = service.Id1 });
            context.EvaluateService(new TestService { Id = service.Id2 });

            return Task.FromResult<IResultBase>(Result.Ok());
        }
    }
}
