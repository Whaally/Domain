using System.Diagnostics;
using FluentAssertions;
using Orleans.TestingHost;
using OrleansTests.TestDomain;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost;
using Whaally.WebData.Tests.Integration.Cluster;
using Task = System.Threading.Tasks.Task;

namespace Whaally.WebData.Tests.Integration.Spikes;


[Collection(ClusterCollection.Name)]
public class SendEnvelopes(ClusterFixture fixture)
{
    private readonly TestCluster _cluster = fixture.Cluster;

    [Fact]
    public Task CanGetGrainReference()
    {
        var grain = _cluster.GrainFactory.GetGrain<
                    IAggregateHandlerGrain<TestDomain.Aggregate>>(Guid.NewGuid());

        grain.Should().NotBeNull();
        
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CanAbort()
    {
        var grain = _cluster.GrainFactory.GetGrain<
            IAggregateHandlerGrain<TestDomain.Aggregate>>(Guid.NewGuid());

        await grain.Abort(new CommandMetadata());
    }
    
    /*
     * There is some issue with Orleans when using surrogates for the ICommandEnvelope and ICommandMetadata objects
     * causing an Orleans.Serialization.ReferenceNotFoundException.
     *
     * The workaround at this point in time is to start using concrete objects instead of interfaces. In this situation,
     * is defensible as these objects exist right on the boundaries between different physical systems. Therefore these
     * objects should be serializable, and I suspect the serialization of concrete objects is a bit less awkward than
     * the serialization of abstract interface types.
     *
     * At the same time we are still holding onto interface usage for the messages contained by these envelopes. In
     * practice this works a lot better given these types are held in projects from which Orleans can generate
     * serializers for their concrete implementations.
     *
     * Yadda yadda
     */
    [Fact]
    public async Task CanEvaluate()
    {
        var id = Guid.NewGuid();
        
        var grain = _cluster.GrainFactory.GetGrain<
            IAggregateHandlerGrain<TestDomain.Aggregate>>(id);
        
        var result = await grain.Evaluate(new CommandEnvelope(
            new CommandMetadata
            {
                Attributes = new Dictionary<string, object>(),
                AggregateId = id,
                AggregateType = typeof(TestDomain.Aggregate),
                CreatedAt = DateTimeOffset.UtcNow,
                ParentContext = new ActivityContext(),
                TransactionId = Guid.NewGuid().ToString()
            }));

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }
}
