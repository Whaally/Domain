namespace Skyhop.Domain.AzureFunctions.Tests;

public abstract class AzureFunctionsTest : IClassFixture<AzureFunctionsTestContainerFixture>
{
    private readonly AzureFunctionsTestContainerFixture _fixture;

    public AzureFunctionsTest(AzureFunctionsTestContainerFixture fixture)
    {
        _fixture = fixture;
    }
}
