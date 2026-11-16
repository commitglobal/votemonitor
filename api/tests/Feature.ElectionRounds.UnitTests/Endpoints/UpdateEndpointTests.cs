namespace Feature.ElectionRounds.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    private static readonly ITimeProvider TimeProvider =
        new FreezeTimeProvider(new DateTime(2024, 01, 10, 03, 04, 05, DateTimeKind.Utc));

    [Theory]
    [MemberData(nameof(NotArchived))]
    public async Task ShouldUpdate_WhenStartDateInThePastAndElectionRoundNotArchived(Action<ElectionRoundAggregate> setStatus)
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        setStatus(electionRound);
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);
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
    public async Task ShouldFail_WhenStartDateInThePastAndElectionRoundArchived()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        electionRound.Archive();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);

        // Act
        var act = () => endpoint.ExecuteAsync(new Update.Request
        {
            Id = electionRound.Id,
            Title = "a fixed title",
            CountryId = electionRound.CountryId,
            StartDate = new DateOnly(2024, 01, 08)
        }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationFailureException>();
        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldUpdate_WhenStartDateIsTodayWestOfUtcAndElectionRoundArchived()
    {
        // Arrange: the UTC date is 2024-01-10, still 2024-01-09 west of UTC
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        electionRound.Archive();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);
        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);

        // Act
        var result = await endpoint.ExecuteAsync(new Update.Request
        {
            Id = electionRound.Id,
            Title = "a fixed title",
            CountryId = electionRound.CountryId,
            StartDate = new DateOnly(2024, 01, 09)
        }, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NoContent>();
    }

    public static IEnumerable<object[]> NotArchived =>
    [
        [(Action<ElectionRoundAggregate>)(_ => { })],
        [(Action<ElectionRoundAggregate>)(x => x.Start())]
    ];

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);

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
        var electionRound = new ElectionRoundAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        repository
            .AnyAsync(Arg.Any<GetActiveElectionRoundSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);

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
        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);
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

        var endpoint = Factory.Create<Update.Endpoint>(repository, TimeProvider);

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
