using Feature.FormTemplates.Draft;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.FormTemplates.UnitTests.Endpoints;
public class DraftEndpointTests
{
    [Fact]
    public async Task Should_DraftFormTemplate_And_Return_NoContent_WhenFormTemplateExists()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker(status: FormStatus.Published).Generate();

        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();
        repository
            .GetByIdAsync(formTemplate.Id)
            .Returns(formTemplate);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = formTemplate.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository
            .Received(1)
            .UpdateAsync(Arg.Is<FormTemplateAggregate>(x => x.Status == FormStatus.Drafted));

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormTemplateDoesNotExists()
    {
        // Arrange
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();
        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
