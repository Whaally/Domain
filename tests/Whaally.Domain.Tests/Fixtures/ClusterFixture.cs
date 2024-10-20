using Marten;
using Marten.Events.Daemon.Coordination;
using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Serialization;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Infrastructure.OrleansHost;

namespace Whaally.Domain.Tests.Fixtures;

public sealed class ClusterFixture : IDisposable
{
    public TestCluster Cluster { get; } = new TestClusterBuilder()
        .AddClientBuilderConfigurator<TestClientConfigurations>()
        .AddSiloBuilderConfigurator<TestSiloConfigurations>()
        .Build();
    
    public ClusterFixture() => Cluster.Deploy();

    void IDisposable.Dispose() => Cluster.StopAllSilos();
}

file sealed class TestClientConfigurations : IClientBuilderConfigurator
{
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder.Services.AddDomain("Whaally.Domain.Tests");
            
        clientBuilder.Services.Remove(clientBuilder.Services.Single(q => q.ServiceType.IsAssignableTo(typeof(IAggregateHandlerFactory))));
            
        clientBuilder.Services.AddSingleton<IAggregateHandlerFactory, OrleansAggregateHandlerFactory>();
    }
}

file sealed class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(static services =>
        {
            // ToDo: Find a way to configure this stuff in a more straightforward manner.
            services.AddDomain("Whaally.Domain.Tests");
            services.Remove(services.Single(q => q.ServiceType.IsAssignableTo(typeof(IAggregateHandlerFactory))));
            services.AddSingleton<IAggregateHandlerFactory, OrleansAggregateHandlerFactory>();
            services.AddMarten(config =>
            {
                config.Connection(
                    "Server=127.0.0.1;Port=5432;Userid=postgres;Password=postgres;Pooling=false;MinPoolSize=1;MaxPoolSize=20;Timeout=15;SslMode=Disable;Database=webdata");
            });

            services.AddSerializer(serializer => serializer.AddProtobufSerializer());
        }).AddCustomStorageBasedLogConsistencyProvider();
    }
}