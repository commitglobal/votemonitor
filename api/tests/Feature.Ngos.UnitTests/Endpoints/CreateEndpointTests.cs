using Feature.Ngos.Create;
using Feature.Ngos.Specifications;
using Endpoint = Feature.Ngos.Create.Endpoint;

namespace Feature.Ngos.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithNgoModel_WhenNoConflict()
    {
        // Arrange
        var ngoName = "UniqueName";
        var repository = Substitute.For<IRepository<NgoAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetNgoByNameSpecification>())
            .Returns(false);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Name = ngoName };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await repository
            .Received(1)
            .AddAsync(Arg.Is<NgoAggregate>(x => x.Name == ngoName));

        result
            .Should().BeOfType<Results<Ok<NgoModel>, ProblemDetails>>()!
            .Which!
            .Result.Should().BeOfType<Ok<NgoModel>>()!
            .Which!.Value!.Name.Should().Be(ngoName);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenNgoWithSameNameExists()
    {
        // Arrange
        var repository = Substitute.For<IRepository<NgoAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetNgoByNameSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Name = "ExistingName" };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NgoModel>, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<ProblemDetails>();
    }
}
