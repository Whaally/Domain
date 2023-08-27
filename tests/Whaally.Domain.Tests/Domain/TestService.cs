using FluentResults;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain.Tests.Domain
{
    internal class TestService : IService
    {
        public string? Id { get; init; }
    }

    internal class TestServiceHandler : IServiceHandler<TestService>
    {
        public Task<IResultBase> Handle(IServiceHandlerContext context, TestService service)
        {
            context.StageCommand(
                service.Id!,
                new TestCommand());

            return Task.FromResult<IResultBase>(Result.Ok());
        }
    }
}
