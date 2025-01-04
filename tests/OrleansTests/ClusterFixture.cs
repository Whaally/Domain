using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.TestingHost;
using Whaally.Domain;
using Whaally.Domain.Infrastructure.OrleansHost;

namespace Whaally.WebData.Tests.Integration.Cluster;

public sealed class ClusterFixture : IDisposable
{
    public TestCluster Cluster { get; } = new TestClusterBuilder()
        .AddSiloBuilderConfigurator<TestSiloConfigurations>()
        .Build();

    public ClusterFixture() => Cluster.Deploy();

    void IDisposable.Dispose() => Cluster.StopAllSilos();
}

file sealed class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {        
        siloBuilder.AddCustomStorageBasedLogConsistencyProvider();
        
        // siloBuilder.Configure<MessagingOptions>(opts =>
        // {
        //     opts.ResponseTimeoutWithDebugger = TimeSpan.FromMilliseconds(1000);
        //     opts.ResponseTimeout = TimeSpan.FromMilliseconds(1000);
        // });
        
        siloBuilder.ConfigureServices(static services =>
        {
            services.AddDomain(c =>
            {
                c.AggregateHandlerFactory = services 
                    => new OrleansAggregateHandlerFactory(
                        services.GetRequiredService<IClusterClient>());
            });
        });
    }
}
