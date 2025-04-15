namespace Feature.ElectionRounds.UnitTests.Endpoints;

public class UnstartEndpointTests
{
    [Fact]
    public async Task Should_Unstart_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .GetByIdAsync(electionRound.Id)
            .Returns(electionRound);

        var endpoint = Factory.Create<Unstart.Endpoint>(repository);

        // Act
        var request = new Unstart.Request { Id = electionRound.Id };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        electionRound
            .Received(1)
            .Unstart();

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
        var endpoint = Factory.Create<Unstart.Endpoint>(repository);

        // Act
        var request = new Unstart.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
