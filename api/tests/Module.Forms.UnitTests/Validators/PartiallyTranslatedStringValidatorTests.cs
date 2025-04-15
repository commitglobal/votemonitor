using FluentValidation.TestHelper;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Validators;

namespace Module.Forms.UnitTests.Validators;

public class PartiallyTranslatedStringValidatorTests
{
    [Fact]
    public void Validation_ShouldFail_When_NoTranslations()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1]);
        var translatedString = new TranslatedString();

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("")
            .WithErrorMessage("Provide at least one translation");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("aaaa")]
    public void Validation_ShouldFail_When_LanguageCode_IsNot_2Chars(string languageCode)
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1]);
        var translatedString = new TranslatedString { [languageCode] = "a string" };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor($@"x[""{languageCode}""]")
            .WithErrorMessage($@"Code ""{languageCode}"" is in invalid format.");
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_LanguageCode_IsEmpty(string emptyLanguageCode)
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1]);
        var translatedString = new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "a string", [emptyLanguageCode] = "a string"
        };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor($@"x[""{emptyLanguageCode}""]")
            .WithErrorMessage($@"Code ""{emptyLanguageCode}"" is in invalid format.");
    }

    [Fact]
    public void Validation_ShouldFail_When_LanguageCode_IsNot_InSupportedLanguages()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);
        var translatedString = new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "a string", [LanguagesList.HU.Iso1] = "a string"
        };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(@"x[""HU""]")
            .WithErrorMessage("Supported languages [EN,RO] do not include HU.");
    }

    [Fact]
    public void Validation_ShouldFail_When_MissingLanguageInTranslation()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);
        var translatedString = new TranslatedString { [LanguagesList.RO.Iso1] = "valid" };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("")
            .WithErrorMessage("Missing translation placeholder for \"EN\"");
    }

    [Fact]
    public void Validation_ShouldPass_When_PartiallyTranslated()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);
        var translatedString = new TranslatedString { [LanguagesList.RO.Iso1] = "valid", [LanguagesList.EN.Iso1] = "" };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
