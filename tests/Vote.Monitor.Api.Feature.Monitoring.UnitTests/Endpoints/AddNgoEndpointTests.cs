namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Endpoints;

public class AddNgoEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();

        var endpoint = Factory.Create<AddNgo.Endpoint>(repository, ngoRepository);

        // Act
        var request = new AddNgo.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
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

        var endpoint = Factory.Create<AddNgo.Endpoint>(repository, ngoRepository);

        // Act
        var request = new AddNgo.Request { Id = electionRoundId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("NGO not found");
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
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(CSOStatus.Deactivated);

        var endpoint = Factory.Create<AddNgo.Endpoint>(repository, ngoRepository);

        // Act
        var request = new AddNgo.Request { Id = electionRoundId, NgoId = ngoId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<ValidationProblem>();

        var errors = result.Result.As<ValidationProblem>().ProblemDetails.Errors.Values;
        errors.Should().HaveCount(1);
        errors.First().Should().Contain(["Only active ngos can monitor elections"]);
    }

    [Fact]
    public async Task ShouldReturnNoContent_AndAddToMonitoringNgos()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngoId = Guid.NewGuid();

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var electionRound = Substitute.For<ElectionRoundAggregate>();
        repository.GetByIdAsync(electionRoundId).Returns(electionRound);

        var ngoRepository = Substitute.For<IReadRepository<NgoAggregate>>();
        ngoRepository.SingleOrDefaultAsync(Arg.Any<GetNgoStatusSpecification>()).Returns(CSOStatus.Activated);

        var endpoint = Factory.Create<AddNgo.Endpoint>(repository, ngoRepository);

        // Act
        var request = new AddNgo.Request { Id = electionRoundId, NgoId = ngoId };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound<string>, ValidationProblem>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        electionRound.Received(1).AddMonitoringNgo(ngoId);
    }
}
