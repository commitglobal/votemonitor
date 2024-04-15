namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class StartEndpointTests
{
    [Fact]
    public async Task Should_Start_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        var endpoint = Factory.Create<Start.Endpoint>(repository);

        // Act
        var request = new Start.Request { Id = electionRound.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        electionRound
            .Received(1)
            .Start();

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
        var endpoint = Factory.Create<Start.Endpoint>(repository);

        // Act
        var request = new Start.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
