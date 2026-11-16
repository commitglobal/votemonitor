namespace Feature.ElectionRounds.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    [Theory]
    [MemberData(nameof(NotArchivedStatuses))]
    public async Task ShouldUpdate_WhenStartDateInThePastAndElectionRoundNotArchived(string status)
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker(status: ElectionRoundStatus.FromValue(status)).Generate();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository);
        var startDate = new DateOnly(2023, 06, 01);

        // Act
        var result = await endpoint.ExecuteAsync(new Update.Request
        {
            Id = electionRound.Id,
            Title = "a fixed title",
            CountryId = electionRound.CountryId,
            StartDate = startDate
        }, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NoContent>();
        electionRound.StartDate.Should().Be(startDate);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenElectionRoundArchived()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker(status: ElectionRoundStatus.Archived).Generate();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var result = await endpoint.ExecuteAsync(new Update.Request
        {
            Id = electionRound.Id,
            Title = "a fixed title",
            CountryId = electionRound.CountryId,
            StartDate = new DateOnly(2030, 01, 01)
        }, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<Conflict<ProblemDetails>>();
        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    public static IEnumerable<object[]> NotArchivedStatuses =>
    [
        [ElectionRoundStatus.NotStarted.Value],
        [ElectionRoundStatus.Started.Value]
    ];

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var request = new Update.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenElectionRoundWithSameTitleExists()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker(status: ElectionRoundStatus.NotStarted).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        repository
            .AnyAsync(Arg.Any<GetActiveElectionRoundSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var request = new Update.Request { Id = electionRound.Id, Title = "ExistingTitle" };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldStoreNoEnglishTitle_WhenEnglishTitleIsMissing()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(Arg.Any<Guid>())
            .Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository);
        var countryId = Guid.NewGuid();
        var startDate = new DateOnly(2024, 01, 02);

        // Act
        await endpoint.ExecuteAsync(new Update.Request
        {
            Id = Guid.NewGuid(),
            Title = "Alegeri parlamentare 2026",
            EnglishTitle = null,
            CountryId = countryId,
            StartDate = startDate
        }, CancellationToken.None);

        // Assert
        electionRound.Received(1).UpdateDetails(countryId, "Alegeri parlamentare 2026", null, startDate);
    }

    [Fact] public async Task ShouldNoContent_AfterUpdatingElectionRoundDetails()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(Arg.Any<Guid>())
            .Returns(electionRound);

        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var updatedTitle = "updatedTitle";
        var updatedEnglishTitle = "updatedEnglishTitle";
        var updatedCountry = Guid.NewGuid();
        var updatedStartDate = new DateOnly(2024, 01, 02);

        var request = new Update.Request
        {
            Id = Guid.NewGuid(),
            Title = updatedTitle,
            EnglishTitle = updatedEnglishTitle,
            CountryId = updatedCountry,
            StartDate = updatedStartDate
        };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        electionRound.Received(1).UpdateDetails(updatedCountry, updatedTitle, updatedEnglishTitle, updatedStartDate);
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }
}
