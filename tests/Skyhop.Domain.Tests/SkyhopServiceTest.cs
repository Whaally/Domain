using FluentAssertions;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Tests;

namespace Skyhop.Domain.Tests;

public abstract class SkyhopServiceTest<TService> : ServiceTest<TService>
    where TService : class, IService
{
    public SkyhopServiceTest(
        IServiceHandler<TService> handler, 
        TService service) : base(handler, service) { }
    
    [Fact]
    public void AssertCommandSerializerIsGenerated()
    {
        typeof(TService).Should().BeDecoratedWith<GenerateSerializerAttribute>();
    }

    [Fact]
    public void AssertCommandIsMarkedImmutable()
    {
        typeof(TService).Should().BeDecoratedWith<ImmutableAttribute>();
    }
}
