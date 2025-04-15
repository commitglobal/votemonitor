using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.PollingStations.UnitTests.Specifications;

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

        var spec = new GetPollingStationSpecification(electionRound.Id,
            pollingStation.Level1,
            pollingStation.Level2,
            pollingStation.Level3,
            pollingStation.Level4,
            pollingStation.Level5,
            pollingStation.Number,
            pollingStation.Address);
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

        var spec = new GetPollingStationSpecification(electionRound.Id,
            pollingStation.Level1,
            pollingStation.Level2,
            pollingStation.Level3,
            pollingStation.Level4,
            pollingStation.Level5,
            pollingStation.Number,
            pollingStation.Address);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(pollingStation);
    }
}
