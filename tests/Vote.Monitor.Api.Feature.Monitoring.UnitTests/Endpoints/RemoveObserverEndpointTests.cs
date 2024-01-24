namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class RemoveObserverEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();

        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository);

        // Act
        var request = new RemoveObserver.Request { Id = Guid.NewGuid(), ObserverId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Election round not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenObserverNotMonitoringElectionRound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());


        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository);

        // Act
        var request = new RemoveObserver.Request { Id = electionRoundId, ObserverId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Requested observer does not monitor requested election round");
    }

    [Fact]
    public async Task ShouldReturnNoContent_AndRemoveMonitoringObserver()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        electionRound.MonitoringObservers.Returns([new MonitoringObserver(Guid.NewGuid(), Guid.NewGuid(), observerId)]);
        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository);

        // Act
        var request = new RemoveObserver.Request { Id = electionRoundId, ObserverId = observerId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        electionRound.Received(1).RemoveMonitoringObserver(observerId);
    }
}
