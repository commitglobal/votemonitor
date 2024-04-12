using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.Specifications;

public class GetPollingStationSpecificationTests
{
    [Fact]
    public void GetPollingStationSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation = new PollingStationAggregateFaker(electionRound: electionRound).Generate();

        var testCollection = new PollingStationAggregateFaker(electionRound: electionRound)
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker(electionRound: electionRound).Generate(500))
            .ToList();

        var spec = new GetPollingStationSpecification(electionRound.Id, pollingStation.Address, new Dictionary<string, string>());

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
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation = new PollingStationAggregateFaker(electionRound).Generate();

        var testCollection = new PollingStationAggregateFaker(electionRound)
            .Generate(500)
            .Union(new[] { pollingStation })
            .Union(new PollingStationAggregateFaker(electionRound).Generate(500))
            .ToList();

        var spec = new GetPollingStationSpecification(electionRound.Id, pollingStation.Address[..(pollingStation.Address.Length / 2)], new Dictionary<string, string>());

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }
}
