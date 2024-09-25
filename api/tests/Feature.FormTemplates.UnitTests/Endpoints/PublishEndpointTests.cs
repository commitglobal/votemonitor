﻿using Feature.FormTemplates.Publish;
using Vote.Monitor.Core.Models;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class PublishEndpointTests
{
    [Fact]
    public async Task Should_PublishFormTemplate_And_Return_NoContent_WhenFormTemplateExists()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker(status: FormTemplateStatus.Drafted).Generate();

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
            .UpdateAsync(Arg.Is<FormTemplateAggregate>(x => x.Status == FormTemplateStatus.Published));

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
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
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnProblemDetails_WhenFormTemplateIsPartiallyTranslated()
    {
        // Arrange
        var name = new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "A title",
            [LanguagesList.RO.Iso1] = ""
        };

        var formTemplate = new FormTemplateAggregateFaker(status: FormTemplateStatus.Drafted,
            languages: [LanguagesList.RO, LanguagesList.EN],
            name: name).Generate();

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
            .DidNotReceiveWithAnyArgs()
            .UpdateAsync(Arg.Any<FormTemplateAggregate>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<ProblemDetails>();
    }
}
