using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.Tests;

public abstract class DomainTest
{
    protected readonly ServiceProvider Services;
    protected readonly DomainContext Domain;
    protected readonly IAggregateHandlerFactory AggregateFactory;

    public DomainTest()
    {
        Services = new ServiceCollection()
            .AddDomain("Skyhop.Domain")
            .BuildServiceProvider();

        Domain = Services.GetRequiredService<DomainContext>();

        AggregateFactory = Services.GetRequiredService<IAggregateHandlerFactory>();
    }    
}