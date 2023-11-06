namespace Vote.Monitor.Feature.PollingStation.UnitTests.Specifications;

public class GetPollingStationByIdSpecificationTests
{
    [Fact]
    public void GetPollingStationByIdSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var pollingStation = new PollingStationAggregateFaker(Guid.NewGuid()).Generate();

        var testCollection = new PollingStationAggregateFaker()
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetPollingStationByIdSpecification(pollingStation.Id);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }
}
