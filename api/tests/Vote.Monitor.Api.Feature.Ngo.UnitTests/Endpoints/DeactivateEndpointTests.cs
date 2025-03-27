using Vote.Monitor.Api.Feature.Ngo.Deactivate;
using Endpoint = Vote.Monitor.Api.Feature.Ngo.Deactivate.Endpoint;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.Endpoints;

public class DeactivateEndpointTests
{
    [Fact]
    public async Task Should_DeactivateNgo_And_Return_NoContent_WhenNgoExists()
    {
        // Arrange
        var ngo = Substitute.For<NgoAggregate>();

        var repository = Substitute.For<IRepository<NgoAggregate>>();
        repository
            .GetByIdAsync(ngo.Id)
            .Returns(ngo);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        ngo
            .Received(1)
            .Deactivate();

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoDoesNotExists()
    {
        // Arrange
        var repository = Substitute.For<IRepository<NgoAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
