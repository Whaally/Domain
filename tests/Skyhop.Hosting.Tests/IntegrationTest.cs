using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace Skyhop.Hosting.Tests;

public abstract class IntegrationTest : IClassFixture<ApplicationFactory<Program>>
{
    protected readonly ApplicationFactory<Program> Factory;
    protected IDocumentStore DocumentStore => Factory.Services.GetRequiredService<IDocumentStore>();

    protected IntegrationTest(ApplicationFactory<Program> factory)
    {
        Factory = factory;
    }

    /// <summary>
    /// 1. Start generation of projections
    /// 2. Wait for projections to be projected
    /// </summary>
    protected async Task GenerateProjectionsAsync()
    {
        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        await daemon.WaitForNonStaleData(10.Seconds());
    }

    protected IDocumentSession OpenSession() => DocumentStore.LightweightSession();
}
