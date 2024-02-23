using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class AddObserverEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = Guid.NewGuid() };
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

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = electionRoundId, InviterNgoId = Guid.NewGuid() };
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
        var inviterNgoId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(NgoStatus.Activated);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = electionRoundId, InviterNgoId = inviterNgoId };
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
        var ngoId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(NgoStatus.Deactivated);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = electionRoundId, InviterNgoId = ngoId };
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
        var ngoId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        repository.GetByIdAsync(electionRoundId).Returns(new ElectionRoundAggregateFaker(id: electionRoundId).Generate());

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(NgoStatus.Activated);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
        observerRepository.SingleOrDefaultAsync(Arg.Any<GetObserverStatusSpecification>()).Returns(UserStatus.Deactivated);

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = electionRoundId, InviterNgoId = ngoId };
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
        var electionRoundId = Guid.NewGuid();
        var ngoId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(NgoStatus.Activated);

        var observerRepository = Substitute.For<IReadRepository<ObserverAggregate>>();
        observerRepository.SingleOrDefaultAsync(Arg.Any<GetObserverStatusSpecification>()).Returns(UserStatus.Active);

        var endpoint = Factory.Create<AddObserver.Endpoint>(repository, ngoRepository, observerRepository);

        // Act
        var request = new AddObserver.Request { Id = electionRoundId, InviterNgoId = ngoId, ObserverId = observerId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        electionRound.Received(1).AddMonitoringObserver(observerId, ngoId);
    }
}
