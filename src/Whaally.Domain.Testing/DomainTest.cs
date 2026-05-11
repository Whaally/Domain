using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Testing;

public abstract class DomainTest
{
    public virtual IServiceProvider Services { get; init; } 
        = new ServiceCollection()
            .AddDomain()
            .BuildServiceProvider();

    public DomainContext Domain => Services.GetRequiredService<DomainContext>();
    public IAggregateHandlerFactory Factory => Services.GetRequiredService<IAggregateHandlerFactory>();
}
