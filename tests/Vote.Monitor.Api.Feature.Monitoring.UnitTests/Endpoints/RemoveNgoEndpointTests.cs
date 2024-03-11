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
        var request = new RemoveNgo.Request { ElectionRoundId = Guid.NewGuid(), NgoId = Guid.NewGuid() };
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
        repository
            .SingleOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>())
            .Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var endpoint = Factory.Create<RemoveNgo.Endpoint>(repository);

        // Act
        var request = new RemoveNgo.Request { ElectionRoundId = electionRoundId, NgoId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Ngo not found");
    }

    [Fact]
    public async Task ShouldReturnNoContent_AndRemoveMonitoringNgo()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngoId = Guid.NewGuid();

        var ngo = new NgoAggregateFaker(ngoId: ngoId).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(ngo: ngo).Generate();

        var electionRound = new ElectionRoundAggregateFaker(id: electionRoundId, monitoringNgos: [monitoringNgo]).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository
            .SingleOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>())
            .Returns(electionRound);

        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var endpoint = Factory.Create<RemoveNgo.Endpoint>(repository);

        // Act
        var request = new RemoveNgo.Request { ElectionRoundId = electionRoundId, NgoId = ngoId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        await repository.Received(1).UpdateAsync(Arg.Is<ElectionRoundAggregate>(x => x.MonitoringNgos.Count == 0));
    }
}
