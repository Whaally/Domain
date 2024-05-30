using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Tests;

public abstract class DomainTest : IDisposable
{
    public DomainTest() { }

    public DomainTest(Action<Whaally.Domain.Domain>? domainInitializer)
    {
        domainInitializer?.Invoke(Domain);
    }
    
    public IServiceProvider Services = new ServiceCollection()
        .AddDomain()
        .BuildServiceProvider();

    public Whaally.Domain.Domain Domain => Services.GetRequiredService<Whaally.Domain.Domain>();
    public IAggregateHandlerFactory Factory => Services.GetRequiredService<IAggregateHandlerFactory>();

    public void Dispose()
    {
            
    }
}
