using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.Tests;

public abstract class DomainTest
{
    protected readonly ServiceProvider Services;
    protected readonly IAggregateHandlerFactory AggregateFactory;

    public DomainTest()
    {
        Services = new ServiceCollection()
            .AddDomain("Skyhop.Domain")
            .BuildServiceProvider();

        AggregateFactory = Services.GetRequiredService<IAggregateHandlerFactory>();
    }    
}