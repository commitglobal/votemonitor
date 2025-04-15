namespace Feature.ElectionRounds.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    [Fact]
    public async Task Should_DeleteElectionRound_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        var endpoint = Factory.Create<Delete.Endpoint>(repository);

        // Act
        var request = new Delete.Request { Id = electionRound.Id };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await repository.Received(1).DeleteAsync(electionRound);

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<Delete.Endpoint>(repository);

        // Act
        var request = new Delete.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<ElectionRoundAggregate>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
