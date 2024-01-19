namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    [Fact]
    public async Task Should_DeleteCSO_And_ReturnNoContent_WhenCSOExists()
    {
        // Arrange
        var cso = new CSOAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(cso.Id)
            .Returns(cso);

        var endpoint = Factory.Create<Delete.Endpoint>(repository);

        // Act
        var request = new Delete.Request { Id = cso.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository.Received(1).DeleteAsync(cso);

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCSONotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<CSOAggregate>>();
        var endpoint = Factory.Create<Delete.Endpoint>(repository);

        // Act
        var request = new Delete.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<CSOAggregate>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
