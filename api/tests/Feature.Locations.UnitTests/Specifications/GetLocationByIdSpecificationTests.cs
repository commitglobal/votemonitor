using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Locations.UnitTests.Specifications;

public class GetLocationByIdSpecificationTests
{
    [Fact]
    public void GetLocationByIdSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var location = new LocationAggregateFaker(id: Guid.NewGuid(), electionRound: electionRound).Generate();

        var testCollection = new LocationAggregateFaker(electionRound: electionRound)
            .Generate(500)
            .Union(new[] { location })
            .Union(new LocationAggregateFaker(id: location.Id).Generate(500))
            .ToList();

        var spec = new GetLocationByIdSpecification(location.ElectionRoundId, location.Id);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(location);
    }
}
