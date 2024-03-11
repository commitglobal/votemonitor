namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenNoConflict()
    {
        // Arrange
        var templateName = new TranslatedString { [LanguagesList.RO.Iso1] = "UniqueName" };
        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetFormTemplateSpecification>())
            .Returns(false);

        var endpoint = Factory.Create<Create.Endpoint>(repository, timeService);

        // Act
        var request = new Create.Request
        {
            Name = templateName,
            Code = "a code",
            Languages = [LanguagesList.RO.Iso1]
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository
               .Received(1)
               .AddAsync(Arg.Is<FormTemplateAggregate>(x => x.Name == templateName));

        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, Conflict<ProblemDetails>>>()!
            .Which!
            .Result.Should().BeOfType<Ok<AttachmentModel>>()!
            .Which!.Value!.Name.Should().BeEquivalentTo(templateName);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenNgoWithSameNameExists()
    {
        // Arrange
        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetFormTemplateSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Create.Endpoint>(repository, timeService);

        // Act
        var request = new Create.Request();
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }
}
