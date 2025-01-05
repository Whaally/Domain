using FluentAssertions;
using JasperFx.Core;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class ServiceTests
{
    readonly IServiceProvider _services = DependencyContainer.Create();

    [Fact]
    public void ServiceCanBeEvaluated()
    {
        var context = new ServiceHandlerContext(_services, new ServiceMetadata());
        var service = new TestService
        {
            Id = Guid.NewGuid().ToString()
        };

        var result = new TestServiceHandler()
            .Invoke(context, service)
            .ToBlockingEnumerable();

        result.Should().BeEmpty();
        Assert.Equal(service.Id, context.Commands.Single().Metadata.AggregateId);
    }

    [Fact]
    public void ServiceCanInvokeOtherServices()
    {
        var context = new ServiceHandlerContext(_services, new ServiceMetadata());
        var service = new TestParentService
        {
            Id1 = Guid.NewGuid().ToString(),
            Id2 = Guid.NewGuid().ToString()
        };

        var result = new TestParentServiceHandler()
            .Invoke(context, service)
            .ToBlockingEnumerable();

        result.Should().BeEmpty();
        Assert.Equal(service.Id1, context.Commands.First().Metadata.AggregateId);
        Assert.Equal(service.Id2, context.Commands.Last().Metadata.AggregateId);
    }
}
