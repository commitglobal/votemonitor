using System.Text.Json;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Vote.Monitor.Domain.UnitTests.Entities.IncidentReportAggregate;

public class IncidentReportLocationTypeTests
{
    private record Holder(IncidentReportLocationType LocationType);

    [Theory]
    [InlineData("PollingStation", "PollingStation")]
    [InlineData("OtherPollingStation", "OtherPollingStation")]
    [InlineData("NotRelatedToAPollingStation", "NotRelatedToAPollingStation")]
    [InlineData("OtherLocation", "NotRelatedToAPollingStation")]
    public void Deserialize_ShouldReadValueAndLegacyOtherLocation(string json, string expected)
    {
        var holder = JsonSerializer.Deserialize<Holder>($$"""{ "LocationType": "{{json}}" }""");

        holder!.LocationType.Value.Should().Be(expected);
    }

    [Fact]
    public void Serialize_ShouldWriteNewValue()
    {
        var json = JsonSerializer.Serialize(new Holder(IncidentReportLocationType.NotRelatedToAPollingStation));

        json.Should().Be("""{"LocationType":"NotRelatedToAPollingStation"}""");
    }

    [Fact]
    public void Deserialize_ShouldFail_ForUnknownValue()
    {
        var act = () => JsonSerializer.Deserialize<Holder>("""{ "LocationType": "Somewhere" }""");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void TryParse_ShouldMapLegacyOtherLocation()
    {
        IncidentReportLocationType.TryParse("OtherLocation", out var result).Should().BeTrue();

        result.Should().Be(IncidentReportLocationType.NotRelatedToAPollingStation);
    }
}
