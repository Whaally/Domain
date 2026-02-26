using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Skyhop.Hosting.Tests;

public class ApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
#pragma warning disable CS0618 // Type or member is obsolete
    public PostgreSqlContainer Database = new PostgreSqlBuilder().Build();
#pragma warning restore CS0618 // Type or member is obsolete

    public override async ValueTask DisposeAsync()
    {
        await Database.StopAsync();
        await base.DisposeAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Database
            .StartAsync()
            .GetAwaiter()
            .GetResult();
        
        builder.ConfigureHostConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["ConnectionStrings:PostgreSQL"] = Database.GetConnectionString(),
                    ["Flags:DisableProjectionDaemon"] = "true"
                }!);
        });

        return base.CreateHost(builder);
    }
}
