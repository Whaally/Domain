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
public class ConcurrencyTests(ClusterFixture fixture)
{
    private readonly TestCluster _cluster = fixture.Cluster;

    [Fact]
    public async Task ReleasesLock()
    {
        var id = Guid.NewGuid();
        var txId = Guid.NewGuid().ToString();
        
        var grain = _cluster.GrainFactory.GetGrain<
            IAggregateHandlerGrain<TestDomain.Aggregate>>(id);
        
        var meta1 = new CommandMetadata
        {
            Attributes = new Dictionary<string, object>(),
            AggregateId = id,
            AggregateType = typeof(TestDomain.Aggregate),
            CreatedAt = DateTimeOffset.UtcNow,
            ParentContext = new ActivityContext(),
            TransactionId = txId
        };
        var result1 = await grain.Evaluate(new CommandEnvelope(meta1));
        
        await grain.Abort(meta1);
        
        var result2 = await grain.Evaluate(new CommandEnvelope(
            new CommandMetadata
            {
                Attributes = new Dictionary<string, object>(),
                AggregateId = id,
                AggregateType = typeof(TestDomain.Aggregate),
                CreatedAt = DateTimeOffset.UtcNow,
                ParentContext = new ActivityContext(),
                TransactionId = Guid.NewGuid().ToString()
            }));


        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ReleaseAndContinue()
    {
        
        var id = Guid.NewGuid();
        var txId = Guid.NewGuid().ToString();
        
        var grain = _cluster.GrainFactory.GetGrain<
            IAggregateHandlerGrain<TestDomain.Aggregate>>(id);
        
        var meta1 = new CommandMetadata
        {
            TransactionId = txId
        };
        
        var result1 = await grain.Evaluate(new CommandEnvelope(meta1));
        
        var result2 = grain.Evaluate(new CommandEnvelope(
            new CommandMetadata
            {
                TransactionId = Guid.NewGuid().ToString()
            }));

        result1.IsSuccess.Should().BeTrue();
        
        await grain.Abort(meta1);
        
        (await result2).IsSuccess.Should().BeTrue();
    }
}
