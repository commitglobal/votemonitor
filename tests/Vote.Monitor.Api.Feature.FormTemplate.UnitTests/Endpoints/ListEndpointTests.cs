using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.Endpoints;

public class ListEndpointTests
{
    [Fact]
    public async Task Should_UseCorrectSpecification()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
        var endpoint = Factory.Create<List.Endpoint>(repository);

        repository
            .ListAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns([]);

        // Act
        var request = new List.Request();
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<FormTemplateSlimModel>>, ProblemDetails>>();

        await repository.Received(1).ListAsync(Arg.Any<ListFormTemplatesSpecification>());
        await repository.Received(1).CountAsync(Arg.Any<ListFormTemplatesSpecification>());
    }

    [Fact]
    public async Task Should_ReturnMappedFormTemplates()
    {
        // Arrange
        var numberOfFormTemplates = 3;
        var totalCount = 154;
        var pageSize = 100;

        var formTemplates = new FormTemplateModelFaker().Generate(numberOfFormTemplates);

        var repository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
        repository
            .ListAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns(formTemplates);

        repository
            .CountAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns(totalCount);

        var endpoint = Factory.Create<List.Endpoint>(repository);

        // Act
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = numberOfFormTemplates
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<FormTemplateSlimModel>>, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<Ok<PagedResponse<FormTemplateSlimModel>>>()
            .Which.Value.Should().NotBeNull();

        var pagedResult = (result.Result as Ok<PagedResponse<FormTemplateSlimModel>>);

        pagedResult.Value.PageSize.Should().Be(pageSize);
        pagedResult.Value.CurrentPage.Should().Be(numberOfFormTemplates);
        pagedResult.Value.TotalCount.Should().Be(totalCount);
        pagedResult.Value.Items.Should().BeEquivalentTo(formTemplates);
    }
}
