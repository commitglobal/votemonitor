namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class UnarchiveEndpointTests
{
    [Fact]
    public async Task Should_Unarchive_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        var endpoint = Factory.Create<Unarchive.Endpoint>(repository);

        // Act
        var request = new Unarchive.Request { Id = electionRound.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        electionRound
            .Received(1)
            .Unarchive();

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<Unarchive.Endpoint>(repository);

        // Act
        var request = new Unarchive.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
