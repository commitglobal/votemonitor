namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithCSOModel_WhenNoConflict()
    {
        // Arrange
        var csoName = "UniqueName";
        var timeService = Substitute.For<ITimeService>();
        var repository = Substitute.For<IRepository<CSOAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetCSOByNameSpecification>())
            .Returns(false);

        var endpoint = Factory.Create<Create.Endpoint>(repository, timeService);

        // Act
        var request = new Create.Request { Name = csoName };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository
            .Received(1)
            .AddAsync(Arg.Is<CSOAggregate>(x => x.Name == csoName));

        result
            .Should().BeOfType<Results<Ok<CSOModel>, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Ok<CSOModel>>()
            .Which.Value.Name.Should().Be(csoName);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenCSOWithSameNameExists()
    {
        // Arrange
        var timeService = Substitute.For<ITimeService>();
        var repository = Substitute.For<IRepository<CSOAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetCSOByNameSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Create.Endpoint>(repository, timeService);

        // Act
        var request = new Create.Request { Name = "ExistingName" };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CSOModel>, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }
}
