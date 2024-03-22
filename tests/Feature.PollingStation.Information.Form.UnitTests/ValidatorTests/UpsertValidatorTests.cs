using FluentValidation.TestHelper;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.TestUtils;
using Xunit;

namespace Feature.PollingStation.Information.Form.UnitTests.ValidatorTests;

public class UpsertValidatorTests
{
    private readonly Upsert.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Questions_Invalid()
    {
        // Arrange
        var request = new Upsert.Request
        {
            Questions = [
                new MultiSelectQuestionRequest(),
                new SingleSelectQuestionRequest(),
                new RatingQuestionRequest(),
                new DateQuestionRequest(),
                new TextQuestionRequest(),
                new NumberQuestionRequest()]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Questions[0].Id");
        result.ShouldHaveValidationErrorFor("Questions[1].Id");
        result.ShouldHaveValidationErrorFor("Questions[2].Id");
        result.ShouldHaveValidationErrorFor("Questions[3].Id");
        result.ShouldHaveValidationErrorFor("Questions[4].Id");
        result.ShouldHaveValidationErrorFor("Questions[5].Id");
    }
    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new Upsert.Request
        {
            Languages = []
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    [InlineData("UnknownIso")]
    public void Validation_ShouldFail_When_EmptyLanguageCodes(string invalidLanguageCode)
    {
        // Arrange
        var request = new Upsert.Request
        {
            Languages = [LanguagesList.RO.Iso1, invalidLanguageCode]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
