using FluentAssertions;
using JasperFx.Core;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class ServiceTests
{
    readonly IServiceProvider _services = DependencyContainer.Create();

    [Fact]
    public async Task ServiceCanBeEvaluated()
    {
        var context = new ServiceHandlerContext(_services, new ServiceMetadata());
        var service = new TestService
        {
            Id = Guid.NewGuid()
        };

        var result = await new TestServiceHandler()
            .Invoke(context, service);

        result.Errors.Should().BeEmpty();
        Assert.Equal(service.Id, ((CommandEnvelope)result.Operations.Single()).Metadata.AggregateId);
    }

    [Fact]
    public async Task ServiceCanInvokeOtherServices()
    {
        var context = new ServiceHandlerContext(_services, new ServiceMetadata());
        var service = new TestParentService
        {
            Id1 = Guid.NewGuid(),
            Id2 = Guid.NewGuid()
        };

        var result = await new TestParentServiceHandler()
            .Invoke(context, service);
        
        result.Errors.Should().BeEmpty();
        
        // for evaluation into commands, invoke the service through the evaluation agent
        // Assert.Equal(service.Id1, ((CommandEnvelope)result.Operations.First()).Metadata.AggregateId);
        // Assert.Equal(service.Id2, ((CommandEnvelope)result.Operations.Last()).Metadata.AggregateId);
    }
}
