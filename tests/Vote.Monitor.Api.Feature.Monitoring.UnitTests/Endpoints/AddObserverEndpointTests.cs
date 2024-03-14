using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class AddObserverEndpointTests
{
    private readonly ITimeProvider _timeProvider = Substitute.For<ITimeProvider>();

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request { ElectionRoundId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Election round not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenInvitingNgoNotFound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request { ElectionRoundId = electionRoundId, NgoId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("NGO not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenObserverNotFound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(status: MonitoringNgoStatus.Active, ngo: ngo).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Observer not found");
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenNgoNotActive()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Deactivated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(status: MonitoringNgoStatus.Active, ngo: ngo).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<ValidationProblem>();

        var errors = result.Result.As<ValidationProblem>().ProblemDetails.Errors.Values;
        errors.Should().HaveCount(1);
        errors.First().Should().Contain(["Only active ngos can add monitoring observers"]);
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenObserverNotActive()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(status: MonitoringNgoStatus.Active, ngo: ngo).Generate();
        var observer = new ObserverAggregateFaker(status: UserStatus.Deactivated).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
        observerRepository.GetByIdAsync(observer.Id).Returns(observer);

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request
        {
            ElectionRoundId = electionRoundId,
            NgoId = ngo.Id,
            ObserverId = observer.Id
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<ValidationProblem>();

        var errors = result.Result.As<ValidationProblem>().ProblemDetails.Errors.Values;
        errors.Should().HaveCount(1);
        errors.First().Should().Contain(["Only active observers can monitor elections"]);
    }

    [Fact]
    public async Task ShouldReturnNoContent_AndAddToMonitoringObservers()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = electionRound.AddMonitoringNgo(ngo, _timeProvider);
        var observer = new ObserverAggregateFaker(status: UserStatus.Active).Generate();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRound.Id).Returns(electionRound);

        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
        observerRepository.GetByIdAsync(observer.Id).Returns(observer);

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, monitoringNgoRepository, observerRepository, _timeProvider);

        // Act
        var request = new AddObserver.Request
        {
            ElectionRoundId = electionRound.Id,
            NgoId = monitoringNgo.Id,
            ObserverId = observer.Id
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        await monitoringNgoRepository
            .Received(1)
             .UpdateAsync(Arg.Is<MonitoringNgoAggregate>(x => x.MonitoringObservers.Any(o => o.ObserverId == observer.Id)));
    }
}
