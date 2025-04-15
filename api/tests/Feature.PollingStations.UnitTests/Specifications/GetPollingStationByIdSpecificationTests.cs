using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.PollingStations.UnitTests.Specifications;

public class GetPollingStationByIdSpecificationTests
{
    [Fact]
    public void GetPollingStationByIdSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation = new PollingStationAggregateFaker(id: Guid.NewGuid(), electionRound: electionRound).Generate();

        var testCollection = new PollingStationAggregateFaker(electionRound: electionRound)
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker(id: pollingStation.Id).Generate(500))
            .ToList();

        var spec = new GetPollingStationByIdSpecification(pollingStation.ElectionRoundId, pollingStation.Id);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }
}
