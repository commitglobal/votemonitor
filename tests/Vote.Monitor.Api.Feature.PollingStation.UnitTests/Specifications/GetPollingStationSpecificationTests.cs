namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.Specifications;

public class GetPollingStationSpecificationTests
{
    [Fact]
    public void GetPollingStationSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var pollingStation = new PollingStationAggregateFaker().Generate();

        var testCollection = new PollingStationAggregateFaker()
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetPollingStationSpecification(pollingStation.Address, new Dictionary<string, string>());
        
        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }

    [Fact]
    public void GetPollingStationSpecification_AppliesCorrectFilters_WhenPartialAddress()
    {
        // Arrange
        var pollingStation = new PollingStationAggregateFaker().Generate();

        var testCollection = new PollingStationAggregateFaker()
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetPollingStationSpecification(pollingStation.Address[..(pollingStation.Address.Length / 2)], new Dictionary<string, string>());
        
        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }
}
