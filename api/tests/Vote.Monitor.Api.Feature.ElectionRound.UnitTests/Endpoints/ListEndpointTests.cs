using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class ListEndpointTests
{
    [Fact]
    public async Task Should_UseCorrectSpecification()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<List.Endpoint>(repository);

        repository
            .ListAsync(Arg.Any<ListElectionRoundsSpecification>())
            .Returns([]);

        // Act
        var request = new List.Request();
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>>();

        await repository.Received(1).ListAsync(Arg.Any<ListElectionRoundsSpecification>());
        await repository.Received(1).CountAsync(Arg.Any<ListElectionRoundsSpecification>());
    }

    [Fact]
    public async Task Should_ReturnMappedElectionRounds()
    {
        // Arrange
        var numberOfItems = 3;
        var totalCount = 154;
        var pageSize = 100;

        var electionRounds = new ElectionRoundBaseModelFaker().Generate(numberOfItems);

        var repository = Substitute.For<IReadRepository<ElectionRoundAggregate>>();
        repository
            .ListAsync(Arg.Any<ListElectionRoundsSpecification>())
            .Returns(electionRounds);

        repository
            .CountAsync(Arg.Any<ListElectionRoundsSpecification>())
            .Returns(totalCount);

        var endpoint = Factory.Create<List.Endpoint>(repository);

        // Act
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = numberOfItems
        };
        var result = await endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<Ok<PagedResponse<ElectionRoundModel>>>()
            .Which.Value.Should().NotBeNull();

        var pagedResult = result.Result.As<Ok<PagedResponse<ElectionRoundModel>>>()!.Value!;

        pagedResult.PageSize.Should().Be(pageSize);
        pagedResult.CurrentPage.Should().Be(numberOfItems);
        pagedResult.TotalCount.Should().Be(totalCount);
        pagedResult.Items.Should().BeEquivalentTo(electionRounds, options => options.ExcludingMissingMembers());
    }
}
