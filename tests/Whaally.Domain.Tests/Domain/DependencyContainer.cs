using Microsoft.Extensions.DependencyInjection;

namespace Whaally.Domain.Tests.Domain
{
    internal static class DependencyContainer
    {
        public static IServiceCollection Services => new ServiceCollection()
            .AddDomain();

        public static IServiceProvider Create()
        {
            return Services.BuildServiceProvider();
        }
    }
}
