namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Specifications;

public class GetActiveElectionRoundSpecificationTests
{
    [Theory]
    [MemberData(nameof(NonArchivedStatuses))]
    public void ShouldMatch_NonArchivedElectionRounds(ElectionRoundStatus status)
    {
        // Arrange
        var countryId = CountriesList.MD.Id;
        string title = "Election title";
        var electionRound = new ElectionRoundAggregateFaker(title: title, status: status, countryId: countryId).Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound,
            new ElectionRoundAggregateFaker(title: title, countryId: countryId, status: ElectionRoundStatus.Archived), // same title & country but archived
            new ElectionRoundAggregateFaker(countryId: countryId, status: status), // same country different title
            new ElectionRoundAggregateFaker(title: title, status: status) // same title different country
        ];

        // Act
        var spec = new GetActiveElectionRoundSpecification(countryId, title);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(electionRound);
    }


    public static IEnumerable<object[]> NonArchivedStatuses =>
        new List<object[]>
        {
            new object[] { ElectionRoundStatus.Started },
            new object[] { ElectionRoundStatus.NotStarted }
        };
}
