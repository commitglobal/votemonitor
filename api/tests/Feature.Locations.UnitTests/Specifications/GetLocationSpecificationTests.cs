using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Locations.UnitTests.Specifications;

public class GetLocationSpecificationTests
{
    [Fact]
    public void GetLocationSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var location = new LocationAggregateFaker(electionRound: electionRound).Generate();

        var testCollection = new LocationAggregateFaker(electionRound: electionRound)
            .Generate(500)
            .Union(new[] { location })
            .Union(new LocationAggregateFaker(electionRound: electionRound).Generate(500))
            .ToList();

        var spec = new GetLocationSpecification(electionRound.Id, location.Level1, location.Level2, location.Level3,
            location.Level4, location.Level5);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(location);
    }
}
