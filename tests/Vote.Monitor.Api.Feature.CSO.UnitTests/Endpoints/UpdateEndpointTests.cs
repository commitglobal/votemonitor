namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    [Fact]
    public async Task ShouldReturnNotFound_WhenCSONotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<CSOAggregate>>();
        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var request = new Update.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenCSOWithSameNameExists()
    {
        // Arrange
        var cso = new CSOAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(cso.Id)
            .Returns(cso);

        repository
            .AnyAsync(Arg.Any<GetCSOByNameSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var request = new Update.Request { Id = cso.Id, Name = "ExistingName" };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldNoContent_AfterUpdatingCSODetails()
    {
        // Arrange
        var cso = Substitute.For<CSOAggregate>();

        var repository = Substitute.For<IRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(Arg.Any<Guid>())
            .Returns(cso);

        var endpoint = Factory.Create<Update.Endpoint>(repository);

        // Act
        var request = new Update.Request { Id = Guid.NewGuid(), Name = "updatedName" };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        cso.Received(1).UpdateDetails("updatedName");
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }
}
