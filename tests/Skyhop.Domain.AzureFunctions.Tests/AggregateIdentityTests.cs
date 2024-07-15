using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using FluentAssertions;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;

namespace Skyhop.Domain.AzureFunctions.Tests;

public class AggregateIdentityTests
{
    [Fact]
    public void AggregateIdentityIsSerializable()
    {
        var id = Guid.Parse("3c1995b0-b02b-40ae-9e25-b71e7170cbb0");

        JsonSerializer.Serialize(new AggregateIdentity("Aircraft", id))
            .Should()
            .Be("{\"Type\":\"Aircraft\",\"Id\":\"3c1995b0-b02b-40ae-9e25-b71e7170cbb0\"}");
    }

    [Fact]
    public void GenericAggregateIdentityIsSerializable()
    {
        var id = Guid.Parse("3c1995b0-b02b-40ae-9e25-b71e7170cbb0");

        JsonSerializer.Serialize(new AggregateIdentity<Aircraft>(id))
            .Should()
            .Be("{\"Type\":\"Aircraft\",\"Id\":\"3c1995b0-b02b-40ae-9e25-b71e7170cbb0\"}");
    }

    [Fact]
    public void StringIsDeserializableToGeneric()
    {
        var id = Guid.Parse("3c1995b0-b02b-40ae-9e25-b71e7170cbb0");
        var str = "{\"Type\":\"Aircraft\",\"Id\":\"3c1995b0-b02b-40ae-9e25-b71e7170cbb0\"}";

        JsonSerializer.Deserialize<AggregateIdentity>(str)
            .Should()
            .BeEquivalentTo(new AggregateIdentity<Aircraft>(id));
    }
}