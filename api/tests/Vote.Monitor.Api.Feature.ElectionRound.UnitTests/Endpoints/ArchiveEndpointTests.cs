namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class ArchiveEndpointTests
{
    [Fact]
    public async Task Should_Archive_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        var endpoint = Factory.Create<Archive.Endpoint>(repository);

        // Act
        var request = new Archive.Request { Id = electionRound.Id };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        electionRound
            .Received(1)
            .Archive();

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
        var endpoint = Factory.Create<Archive.Endpoint>(repository);

        // Act
        var request = new Archive.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
