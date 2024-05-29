namespace Skyhop.Hosting.Tests.Integration;

public class FlightControllerTests : IntegrationTest
{
    public FlightControllerTests(ApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Create_New_Flight()
    {
        var client = Factory.CreateClient();

        var result = await client.PostAsync("/flight/new", null);
        
        Assert.True(result.IsSuccessStatusCode);
        
    }
}
