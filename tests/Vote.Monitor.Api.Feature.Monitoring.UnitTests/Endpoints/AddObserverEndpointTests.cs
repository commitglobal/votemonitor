using Vote.Monitor.Api.Feature.Monitoring.AddObserver;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class AddObserverEndpointTests
{
    private readonly ITimeProvider _timeProvider = Substitute.For<ITimeProvider>();
    private readonly IRepository<ElectionRoundAggregate> _repository;
    private readonly IRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly IReadRepository<Observer> _observerRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public AddObserverEndpointTests()
    {
        _repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        _monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgoAggregate>>();
        _observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _monitoringNgoRepository,
            _observerRepository,
            _monitoringObserverRepository,
            _timeProvider);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Election round not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenInvitingNgoNotFound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        _repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        // Act
        var request = new Request { ElectionRoundId = electionRoundId, MonitoringNgoId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Monitoring NGO not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenObserverNotFound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(status: MonitoringNgoStatus.Active, ngo: ngo).Generate();

        _repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        _monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);
        
        // Act
        var request = new Request { ElectionRoundId = electionRoundId, MonitoringNgoId = ngo.Id };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
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

        _repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        _monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);
        
        // Act
        var request = new Request { ElectionRoundId = electionRoundId, MonitoringNgoId = ngo.Id };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<ValidationProblem>();

        var errors = result.Result.As<ValidationProblem>().ProblemDetails.Errors.Values;
        errors.Should().HaveCount(1);
        errors.First().Should().Contain(["Only active monitoring NGOs can add monitoring observers"]);
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenObserverNotActive()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(status: MonitoringNgoStatus.Active, ngo: ngo).Generate();
        var observer = new ObserverAggregateFaker(status: UserStatus.Deactivated).Generate();

        _repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        _monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        _observerRepository.GetByIdAsync(observer.Id).Returns(observer);

        // Act
        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            MonitoringNgoId = ngo.Id,
            ObserverId = observer.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
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

        _repository.GetByIdAsync(electionRound.Id).Returns(electionRound);

        _monitoringNgoRepository.SingleOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>()).Returns(monitoringNgo);

        _observerRepository.GetByIdAsync(observer.Id).Returns(observer);

        // Act
        var request = new Request
        {
            ElectionRoundId = electionRound.Id,
            MonitoringNgoId = monitoringNgo.Id,
            ObserverId = observer.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<MonitoringObserverModel>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<Ok<MonitoringObserverModel>>();

        await _monitoringObserverRepository
            .Received(1)
             .AddAsync(Arg.Is<MonitoringObserver>(x => x.ObserverId == observer.Id));
    }
}
