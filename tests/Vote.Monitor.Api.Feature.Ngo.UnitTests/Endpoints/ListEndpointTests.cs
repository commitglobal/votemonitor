using Vote.Monitor.Api.Feature.Ngo.List;
using Vote.Monitor.Api.Feature.Ngo.Specifications;
using Vote.Monitor.TestUtils.Fakes;
using Endpoint = Vote.Monitor.Api.Feature.Ngo.List.Endpoint;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.Endpoints;

public class ListEndpointTests
{
    [Fact]
    public async Task Should_UseCorrectSpecification()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<NgoAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        repository
            .ListAsync(Arg.Any<ListNgosSpecification>())
            .Returns([]);

        // Act
        var request = new Request();
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<NgoModel>>, ProblemDetails>>();

        await repository.Received(1).ListAsync(Arg.Any<ListNgosSpecification>());
        await repository.Received(1).CountAsync(Arg.Any<ListNgosSpecification>());
    }

    [Fact]
    public async Task Should_ReturnMappedNgos()
    {
        // Arrange
        var numberOfNgos = 3;
        var totalCount = 154;
        var pageSize = 100;

        var ngos = new NgoAggregateFaker().Generate(numberOfNgos);

        var repository = Substitute.For<IReadRepository<NgoAggregate>>();
        repository
            .ListAsync(Arg.Any<ListNgosSpecification>())
            .Returns(ngos);

        repository
            .CountAsync(Arg.Any<ListNgosSpecification>())
            .Returns(totalCount);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request
        {
            PageSize = pageSize,
            PageNumber = numberOfNgos
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<NgoModel>>, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<Ok<PagedResponse<NgoModel>>>()
            .Which.Value.Should().NotBeNull();

        var pagedResult = (result.Result as Ok<PagedResponse<NgoModel>>);

        pagedResult.Value.PageSize.Should().Be(pageSize);
        pagedResult.Value.CurrentPage.Should().Be(numberOfNgos);
        pagedResult.Value.TotalCount.Should().Be(totalCount);
        pagedResult.Value.Items.Should().BeEquivalentTo(ngos, options => options.ExcludingMissingMembers());
    }
}
