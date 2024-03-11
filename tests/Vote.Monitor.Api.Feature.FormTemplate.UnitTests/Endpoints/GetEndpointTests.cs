namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.Endpoints;

public class GetEndpointTests
{
    [Fact]
    public async Task Should_ReturnFormTemplate_WhenFormTemplateExists()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker().Generate();

        var repository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
        repository
            .GetByIdAsync(formTemplate.Id)
            .Returns(formTemplate);

        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = formTemplate.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<AttachmentModel>>()
            .Which.Value.Should().BeEquivalentTo(formTemplate, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormTemplateDoesNotExist()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
