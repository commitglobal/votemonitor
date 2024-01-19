namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class GetEndpointTests
{
    [Fact]
    public async Task Should_ReturnCSO_WhenCSOExists()
    {
        // Arrange
        var cso = new CSOAggregateFaker().Generate();

        var repository = Substitute.For<IReadRepository<CSOAggregate>>();
        repository
            .GetByIdAsync(cso.Id)
            .Returns(cso);

        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = cso.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CSOModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<CSOModel>>()
            .Which.Value.Should().BeEquivalentTo(cso, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCSONotFound()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<CSOAggregate>>();
        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CSOModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
