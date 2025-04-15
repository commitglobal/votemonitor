using Feature.Ngos.Delete;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Endpoint = Feature.Ngos.Delete.Endpoint;

namespace Feature.Ngos.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    [Fact]
    public async Task Should_DeleteNgo_And_ReturnNoContent_WhenNgoExists()
    {
        // Arrange
        var ngo = new NgoAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<NgoAggregate>>();
        repository
            .GetByIdAsync(ngo.Id)
            .Returns(ngo);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await repository.Received(1).DeleteAsync(ngo);

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<NgoAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<NgoAggregate>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
