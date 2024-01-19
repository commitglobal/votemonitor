namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class ActivateEndpointTests
{
    [Fact]
    public async Task Should_ActivateCSO_And_ReturnNoContent_WhenCSOExists()
    {
        // Arrange
        var cso = Substitute.For<CSOAggregate>();
        var repository = Substitute.For<IRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(cso.Id)
            .Returns(cso);

        var endpoint = Factory.Create<Activate.Endpoint>(repository);

        // Act
        var request = new Activate.Request { Id = cso.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        cso
            .Received(1)
            .Activate();

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCSONotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<CSOAggregate>>();
        var endpoint = Factory.Create<Activate.Endpoint>(repository);

        // Act
        var request = new Activate.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
