namespace Feature.ElectionRounds.UnitTests.Endpoints;

public class UpdateEndpointTests
{
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
        var electionRound = new ElectionRoundAggregateFaker().Generate();

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
