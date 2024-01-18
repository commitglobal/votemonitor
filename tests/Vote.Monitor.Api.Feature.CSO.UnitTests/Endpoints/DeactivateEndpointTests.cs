namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class DeactivateEndpointTests
{
    [Fact]
    public async Task Should_DeactivateCSO_And_Return_NoContent_WhenCSOExists()
    {
        // Arrange
        var cso = Substitute.For<CSOAggregate>();

        var repository = Substitute.For<IRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(cso.Id)
            .Returns(cso);

        var endpoint = Factory.Create<Deactivate.Endpoint>(repository);

        // Act
        var request = new Deactivate.Request { Id = cso.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        cso
            .Received(1)
            .Deactivate();

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
        var endpoint = Factory.Create<Deactivate.Endpoint>(repository);

        // Act
        var request = new Deactivate.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
