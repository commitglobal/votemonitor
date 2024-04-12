
//namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

//public class RemoveObserverEndpointTests
//{
//    [Fact]
//    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
//    {
//        // Arrange
//        var repository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
//        var observersRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

//        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository, observersRepository);

//        // Act
//        var request = new RemoveObserver.Request { ElectionRoundId = Guid.NewGuid(), ObserverId = Guid.NewGuid() };
//        var result = await endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
//            .Which
//            .Result.Should().BeOfType<NotFound<string>>()
//            .Which.Value.Should().Be("Monitoring NGO not found");
//    }

//    [Fact]
//    public async Task ShouldReturnNotFound_WhenObserverNotMonitoringElectionRound()
//    {
//        // Arrange
//        var electionRoundId = Guid.NewGuid();
//        var ngo = new NgoAggregateFaker().Generate();
//        var monitoringNgo = new MonitoringNgoAggregateFaker(ngo: ngo).Generate();

//        var repository = Substitute.For<IRepository<MonitoringNgoAggregate>>();

//        repository
//            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoWithObserversSpecification>())
//            .Returns(monitoringNgo);

//        var observersRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

//        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository, observersRepository);

//        // Act
//        var request = new RemoveObserver.Request { ElectionRoundId = electionRoundId, ObserverId = Guid.NewGuid() };
//        var result = await endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
//            .Which
//            .Result.Should().BeOfType<NotFound<string>>()
//            .Which.Value.Should().Be("Observer not found");
//    }

//    [Fact]
//    public async Task ShouldReturnNoContent_AndRemoveMonitoringObserver()
//    {
//        // Arrange
//        var electionRoundId = Guid.NewGuid();
//        var observerId = Guid.NewGuid();
//        var observer = new ObserverAggregateFaker(id: observerId).Generate();
//        var ngo = new NgoAggregateFaker().Generate();
//        var monitoringNgo = new MonitoringNgoAggregateFaker(ngo: ngo).Generate();
//        monitoringNgo.AddMonitoringObserver(observer);

//        var repository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
//        repository
//            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoWithObserversSpecification>())
//            .Returns(monitoringNgo);

//        var observersRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
//        observersRepository.GetByIdAsync(observerId).Returns(observer);

//        var endpoint = Factory.Create<RemoveObserver.Endpoint>(repository, observersRepository);

//        // Act
//        var request = new RemoveObserver.Request { ElectionRoundId = electionRoundId, ObserverId = observerId };
//        var result = await endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<NoContent, NotFound<string>>>()
//            .Which
//            .Result.Should().BeOfType<NoContent>();

//       await repository
//           .Received(1)
//           .UpdateAsync(Arg.Is<MonitoringNgoAggregate>(x => x.MonitoringObservers.Count == 0));
//    }
//}
