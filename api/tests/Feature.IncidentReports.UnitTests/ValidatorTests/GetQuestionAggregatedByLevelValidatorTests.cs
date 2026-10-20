using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Xunit;

namespace Feature.IncidentReports.UnitTests.ValidatorTests;

public class GetQuestionAggregatedByLevelValidatorTests
{
    private readonly GetQuestionAggregatedByLevel.Validator _validator = new();

    public static TheoryData<LocationLevel> Levels => new(LocationLevel.List);

    private static GetQuestionAggregatedByLevel.Request ValidRequest() => new()
    {
        ElectionRoundId = Guid.NewGuid(),
        NgoId = Guid.NewGuid(),
        FormId = Guid.NewGuid(),
        QuestionId = Guid.NewGuid(),
        Level = LocationLevel.Level1
    };

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.ElectionRoundId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_NgoId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.NgoId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Should_Have_Error_When_FormId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.FormId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FormId);
    }

    [Fact]
    public void Should_Have_Error_When_QuestionId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.QuestionId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Fact]
    public void Should_Have_Error_When_Level_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.Level = null!;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level);
    }

    [Theory]
    [MemberData(nameof(Levels))]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid(LocationLevel level)
    {
        // Arrange
        var request = ValidRequest();
        request.Level = level;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
