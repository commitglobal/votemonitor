using Vote.Monitor.Core.Constants;

namespace Feature.Forms.UnitTests.ValidatorTests;

public class FromTemplateRequestValidatorTests
{
    private readonly FromTemplate.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new FromTemplate.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new FromTemplate.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_TemplateId_Empty()
    {
        // Arrange
        var request = new FromTemplate.Request { TemplateId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TemplateId);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new FromTemplate.Request
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
    public void Validation_ShouldFail_When_InvalidLanguageCodes(string invalidLanguageCode)
    {
        // Arrange
        var request = new FromTemplate.Request
        {
            Languages = [LanguagesList.RO.Iso1, invalidLanguageCode]
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
    public void Validation_ShouldFail_When_InvalidDefaultLanguage(string invalidLanguageCode)
    {
        // Arrange
        var request = new FromTemplate.Request
        {
            DefaultLanguage = invalidLanguageCode
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage);
    }

    [Fact]
    public void Validation_ShouldFail_When_DefaultLanguageNotInLanguageList()
    {
        // Arrange
        var request = new FromTemplate.Request
        {
            DefaultLanguage = LanguagesList.EN.Iso1,
            Languages = [LanguagesList.RO.Iso1]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage)
            .WithErrorMessage("Languages should contain declared default language.");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new FromTemplate.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            Languages = [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1],
            DefaultLanguage = LanguagesList.EN.Iso1,
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
