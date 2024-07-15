using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Testcontainers.Azurite;

namespace Skyhop.Domain.AzureFunctions.Tests;

public class AzureFunctionsTestContainerFixture : IAsyncLifetime
{
    private readonly IFutureDockerImage _azureFunctionsDockerImage;
    
    public AzuriteContainer? AzuriteContainerInstance { get; private set; }
    public IContainer? AzureFunctionsContainerInstance { get; private set; }
    
    public AzureFunctionsTestContainerFixture()
    {
        _azureFunctionsDockerImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "")
            .WithDockerfile("samples/Skyhop.Domain.AzureFunctions/Dockerfile")
            .WithBuildArgument(
                "RESOURCE_REAPER_SESSION_ID",
                ResourceReaper.DefaultSessionId.ToString("D"))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _azureFunctionsDockerImage.CreateAsync();
        
        AzuriteContainerInstance = new AzuriteBuilder().Build();
        await AzuriteContainerInstance.StartAsync();
        
        AzureFunctionsContainerInstance = new ContainerBuilder()
            .WithImage(_azureFunctionsDockerImage)
            .WithPortBinding(80, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(80)))
            .WithEnvironment("AzureWebJobsStorage", AzuriteContainerInstance!.GetConnectionString())
            .Build();
        await AzureFunctionsContainerInstance.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await AzuriteContainerInstance!.DisposeAsync();
        await AzureFunctionsContainerInstance!.DisposeAsync();

        await _azureFunctionsDockerImage.DisposeAsync();
    }
}