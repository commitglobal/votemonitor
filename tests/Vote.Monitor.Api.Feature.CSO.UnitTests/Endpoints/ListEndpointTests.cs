namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Endpoints;

public class ListEndpointTests
{
    [Fact]
    public async Task Should_UseCorrectSpecification()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<CSOAggregate>>();
        var endpoint = Factory.Create<List.Endpoint>(repository);

        repository
            .ListAsync(Arg.Any<ListCSOsSpecification>())
            .Returns([]);

        // Act
        var request = new List.Request();
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>>();

        await repository.Received(1).ListAsync(Arg.Any<ListCSOsSpecification>());
        await repository.Received(1).CountAsync(Arg.Any<ListCSOsSpecification>());
    }

    [Fact]
    public async Task Should_ReturnMappedCSOs()
    {
        // Arrange
        var numberOfCSOs = 3;
        var totalCount = 154;
        var pageSize = 100;

        var csos = new CSOAggregateFaker().Generate(numberOfCSOs);

        var repository = Substitute.For<IReadRepository<CSOAggregate>>();
        repository
            .ListAsync(Arg.Any<ListCSOsSpecification>())
            .Returns(csos);

        repository
            .CountAsync(Arg.Any<ListCSOsSpecification>())
            .Returns(totalCount);

        var endpoint = Factory.Create<List.Endpoint>(repository);

        // Act
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = numberOfCSOs
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<CSOModel>>, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<Ok<PagedResponse<CSOModel>>>()
            .Which.Value.Should().NotBeNull();

        var pagedResult = (result.Result as Ok<PagedResponse<CSOModel>>);

        pagedResult.Value.PageSize.Should().Be(pageSize);
        pagedResult.Value.CurrentPage.Should().Be(numberOfCSOs);
        pagedResult.Value.TotalCount.Should().Be(totalCount);
        pagedResult.Value.Items.Should().BeEquivalentTo(csos, options => options.ExcludingMissingMembers());
    }
}
