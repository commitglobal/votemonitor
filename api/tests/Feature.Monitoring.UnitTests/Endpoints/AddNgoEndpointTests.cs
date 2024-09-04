using Vote.Monitor.Api.Feature.Monitoring.Add;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class AddNgoEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();
        var endpoint = Factory.Create<Endpoint>(repository, ngoRepository, monitoringNgoRepository);

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

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

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();

        var endpoint = Factory.Create<Endpoint>(repository, ngoRepository, monitoringNgoRepository);

        // Act
        var request = new Request { ElectionRoundId = electionRoundId };
        var result = await endpoint.ExecuteAsync(request, default);

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

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Deactivated).Generate();

        ngoRepository.GetByIdAsync(ngo.Id).Returns(ngo);
        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();

        var endpoint = Factory.Create<Endpoint>(repository, ngoRepository, monitoringNgoRepository);

        // Act
        var request = new Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, default);

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

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.GetByIdAsync(ngo.Id).Returns(ngo);
        var monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();

        var endpoint = Factory.Create<Endpoint>(repository, ngoRepository, monitoringNgoRepository);

        // Act
        var request = new Request { ElectionRoundId = electionRoundId, NgoId = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<Ok<Response>>();

       await monitoringNgoRepository.Received(1).AddAsync(Arg.Is<MonitoringNgoAggregate>(x=>x.NgoId == ngo.Id));
    }
}
