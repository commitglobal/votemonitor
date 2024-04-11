using Feature.FormTemplates.Delete;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    [Fact]
    public async Task Should_DeleteFormTemplate_And_ReturnNoContent_WhenFormTemplateExists()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker().Generate();

        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();
        repository
            .GetByIdAsync(formTemplate.Id)
            .Returns(formTemplate);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = formTemplate.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository.Received(1).DeleteAsync(formTemplate);

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormTemplateNotFound()
    {
        // Arrange
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<FormTemplateAggregate>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
