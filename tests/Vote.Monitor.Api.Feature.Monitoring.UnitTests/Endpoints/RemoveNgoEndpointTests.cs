namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class RemoveNgoEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();

        var endpoint = Factory.Create<RemoveNgo.Endpoint>(repository);

        // Act
        var request = new RemoveNgo.Request { Id = Guid.NewGuid(), NgoId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Election round not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoNotMonitoringElectionRound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());


        var endpoint = Factory.Create<RemoveNgo.Endpoint>(repository);

        // Act
        var request = new RemoveNgo.Request { Id = electionRoundId, NgoId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Requested NGO does not monitor requested election round");
    }

    [Fact]
    public async Task ShouldReturnNoContent_AndRemoveMonitoringNgo()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngoId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        electionRound.MonitoringNgos.Returns([new MonitoringNGO(Guid.NewGuid(), ngoId)]);
        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var endpoint = Factory.Create<RemoveNgo.Endpoint>(repository);

        // Act
        var request = new RemoveNgo.Request { Id = electionRoundId, NgoId = ngoId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        electionRound.Received(1).RemoveMonitoringNgo(ngoId);
    }
}
