using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class DomainTests
{
    private ServiceProvider _services = new ServiceCollection()
        .AddDomain()
        .BuildServiceProvider();
    
    [Fact]
    public async Task CanEvaluateService()
    {
        var domain = _services.GetRequiredService<DomainContext>();

        var result = await domain.Invoke(new TestService());
        
        Assert.True(result.IsSuccess);
    }
}