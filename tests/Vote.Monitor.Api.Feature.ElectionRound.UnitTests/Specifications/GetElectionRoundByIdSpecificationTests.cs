namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Specifications;

public class GetElectionRoundByIdSpecificationTests
{
    [Fact]
    public void ShouldMatch_NonArchivedElectionRounds()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var electionRound = new ElectionRoundAggregateFaker(id: electionRoundId).Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound,
            .. new ElectionRoundAggregateFaker().Generate(100)
        ];

        // Act
        var spec = new GetElectionRoundByIdSpecification(electionRoundId);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.Id == electionRoundId);
    }
}
