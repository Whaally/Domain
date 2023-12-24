using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain;

namespace Skyhop.Domain.Tests;

public abstract class DomainTest
{
    protected ServiceProvider Services = new ServiceCollection()
        .AddDomain("Skyhop.Domain")
        .BuildServiceProvider();
}