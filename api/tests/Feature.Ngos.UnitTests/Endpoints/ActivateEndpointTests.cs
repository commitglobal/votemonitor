using Feature.Ngos.Activate;
using Endpoint = Feature.Ngos.Activate.Endpoint;

namespace Feature.Ngos.UnitTests.Endpoints;

public class ActivateEndpointTests
{
    [Fact]
    public async Task Should_ActivateNgo_And_ReturnNoContent_WhenNgoExists()
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
            .Activate();

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoDoesNotExist()
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
