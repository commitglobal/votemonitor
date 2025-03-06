using Feature.Monitoring.Add;
using Feature.Monitoring.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.Monitoring.UnitTests.Endpoints;

public class AddNgoEndpointTests
{
    private readonly IRepository<ElectionRoundAggregate> _repository =
        Substitute.For<IRepository<ElectionRoundAggregate>>();

    private readonly IReadRepository<NgoAggregate> _ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
    private readonly IRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();
    private readonly Endpoint _endpoint;

    public AddNgoEndpointTests()
    {
        _endpoint = Factory.Create<Endpoint>(_repository, _ngoRepository, _monitoringNgoRepository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange & Act 
        var request = new Request { ElectionRoundId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("Election round not found");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoNotFound()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        _repository.FirstOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>())
            .Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        // Act
        var request = new Request { ElectionRoundId = electionRoundId };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("NGO not found");
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenNgoNotActive()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        _repository.FirstOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>())
            .Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngo = new NgoAggregateFaker(status: NgoStatus.Deactivated).Generate();

        _ngoRepository.GetByIdAsync(ngo.Id).Returns(ngo);

        // Act
        var request = new Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<ValidationProblem>();

        var errors = result.Result.As<ValidationProblem>().ProblemDetails.Errors.Values;
        errors.Should().HaveCount(1);
        errors.First().Should().Contain(["Only active ngos can monitor elections"]);
    }

    [Fact]
    public async Task ShouldAddToMonitoringNgos()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();

        var electionRound = Substitute.For<ElectionRoundAggregate>();
        _repository.FirstOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>()).Returns(electionRound);
        var monitoringNgo = new MonitoringNgo(electionRound, ngo);
        electionRound.AddMonitoringNgo(ngo).Returns(monitoringNgo);
        _ngoRepository.GetByIdAsync(ngo.Id).Returns(ngo);

        // Act
        var request = new Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<Ok<Response>>();

        await _monitoringNgoRepository.Received(1).AddAsync(monitoringNgo);
    }
}
