using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Tests.Fixtures;

namespace Whaally.Domain.Tests.Scenarios._0001__orleans_serialization.Tests;

[Collection(ClusterCollection.Name)]
public class SerializationTests(ClusterFixture fixture)
{
    private Task<IResult<IEventEnvelope[]>> _evaluation 
        => new DomainContext(fixture.Cluster.Client.ServiceProvider)
            .EvaluateCommand(Guid.NewGuid().ToString(), new TestCommand());

    [Fact(Skip = "_")]
    public async Task CanInvokeCommand()
        => (await _evaluation).IsSuccess.Should().BeTrue();

    [Fact(Skip = "_")]
    public async Task ReceivesSerializedEventInResponse()
        => (await _evaluation).Value.Should().ContainSingle();

    [Fact(Skip = "_")]
    public async Task CanInspectEvent()
        => (await _evaluation).Value[0].Should().BeAssignableTo<IEventEnvelope<TestEvent>>();

    [Fact(Skip = "_")]
    public async Task CanAccessEventData()
        => (await _evaluation).Value[0].Message.Should().BeAssignableTo<TestEvent>();

    [Fact(Skip = "_")]
    public async Task FlagIsTrue()
        => ((TestEvent)(await _evaluation).Value[0].Message).Flag.Should().BeTrue();
}
