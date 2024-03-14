using Vote.Monitor.Api.Feature.Ngo.Get;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Endpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.Endpoints;

public class GetEndpointTests
{
    [Fact]
    public async Task Should_ReturnNgo_WhenNgoExists()
    {
        // Arrange
        var ngo = new NgoAggregateFaker().Generate();

        var repository = Substitute.For<IReadRepository<NgoAggregate>>();
        repository
            .GetByIdAsync(ngo.Id)
            .Returns(ngo);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = ngo.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NgoModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<NgoModel>>()
            .Which.Value.Should().BeEquivalentTo(ngo, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoDoesNotExist()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<NgoAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NgoModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
