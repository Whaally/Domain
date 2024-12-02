using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Tests.Fixtures;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization.Tests;

[Collection(ClusterCollection.Name)]
public class SerializationTests(ClusterFixture fixture)
{
    private Task<IResult<IEventEnvelope[]>> _evaluation 
        => fixture.Cluster.Client.ServiceProvider.GetRequiredService<DomainContext>()
            .EvaluateCommand(Guid.NewGuid().ToString(), new TestCommand());

    private const string _skipReason = "Fails when running on GitHub Actions";
    
    [Fact(Skip = _skipReason)]
    public async Task CanInvokeCommand()
        => (await _evaluation).IsSuccess.Should().BeTrue();

    [Fact(Skip = _skipReason)]
    public async Task ReceivesSerializedEventInResponse()
        => (await _evaluation).Value.Should().ContainSingle();

    [Fact(Skip = _skipReason)]
    public async Task CanInspectEvent()
        => (await _evaluation).Value[0].Should().BeAssignableTo<IEventEnvelope<TestEvent>>();

    [Fact(Skip = _skipReason)]
    public async Task CanAccessEventData()
        => (await _evaluation).Value[0].Message.Should().BeAssignableTo<TestEvent>();

    [Fact(Skip = _skipReason)]
    public async Task FlagIsTrue()
        => ((TestEvent)(await _evaluation).Value[0].Message).Flag.Should().BeTrue();
}
