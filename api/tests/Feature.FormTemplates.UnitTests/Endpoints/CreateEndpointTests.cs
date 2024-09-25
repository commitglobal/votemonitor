﻿using Feature.FormTemplates.Create;
using Feature.FormTemplates.Specifications;
using Vote.Monitor.Core.Models;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithFormTemplateModel_WhenNoConflict()
    {
        // Arrange
        var templateName = new TranslatedString { [LanguagesList.RO.Iso1] = "UniqueName" };
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetFormTemplateSpecification>())
            .Returns(false);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request
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
            .Should().BeOfType<Results<Ok<FormTemplateSlimModel>, Conflict<ProblemDetails>>>()!
            .Which!
            .Result.Should().BeOfType<Ok<FormTemplateSlimModel>>()!
            .Which!.Value!.Name.Should().BeEquivalentTo(templateName);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenFormTemplateWithSameCodeExists()
    {
        // Arrange
        var repository = Substitute.For<IRepository<FormTemplateAggregate>>();

        repository
            .AnyAsync(Arg.Any<GetFormTemplateSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Endpoint>(repository);

        // Act
        var request = new Request();
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormTemplateSlimModel>, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }
}
