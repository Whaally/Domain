using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Tests;

public abstract class DomainTest : IDisposable
{
    public DomainTest() { }

    public DomainTest(Action<DomainContext>? domainInitializer)
    {
        domainInitializer?.Invoke(Domain);
    }
    
    public IServiceProvider Services = new ServiceCollection()
        .AddDomain()
        .BuildServiceProvider();

    public DomainContext Domain => Services.GetRequiredService<DomainContext>();
    public IAggregateHandlerFactory Factory => Services.GetRequiredService<IAggregateHandlerFactory>();

    public void Dispose()
    {
            
    }
}
