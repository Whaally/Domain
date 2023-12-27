using Whaally.Domain.Service;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class ServiceTests
{
    readonly IServiceProvider _services = DependencyContainer.Create();

    [Fact]
    public async Task ServiceCanBeEvaluated()
    {
        var context = new ServiceHandlerContext(_services);
        var service = new TestService
        {
            Id = Guid.NewGuid().ToString()
        };

        var result = await new TestServiceHandler()
            .Handle(context, service);

        Assert.True(result.IsSuccess);
        Assert.Equal(service.Id, context.Commands.Single().Metadata.AggregateId);
    }

    [Fact]
    public async Task ServiceCanInvokeOtherServices()
    {
        var context = new ServiceHandlerContext(_services);
        var service = new TestParentService()
        {
            Id1 = Guid.NewGuid().ToString(),
            Id2 = Guid.NewGuid().ToString()
        };

        var result = await new TestParentServiceHandler()
            .Handle(context, service);

        Assert.True(result.IsSuccess);
        Assert.Equal(service.Id1, context.Commands.First().Metadata.AggregateId);
        Assert.Equal(service.Id2, context.Commands.Last().Metadata.AggregateId);
    }
}