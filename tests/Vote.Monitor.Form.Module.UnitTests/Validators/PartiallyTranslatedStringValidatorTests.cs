using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Validators;

<<<<<<<< HEAD:tests/Vote.Monitor.Form.Module.UnitTests/Validators/PartiallyTranslatedStringValidatorTests.cs
namespace Vote.Monitor.Form.Module.UnitTests.Validators;
========
namespace Vote.Monitor.Form.Module.UnitTests.ValidatorTests;
>>>>>>>> main:tests/Vote.Monitor.Form.Module.UnitTests/ValidatorTests/PartiallyTranslatedStringValidatorTests.cs

public class PartiallyTranslatedStringValidatorTests
{
    [Fact]
    public void Validation_ShouldFail_When_NoTranslations()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1], 2, 3);
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
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1], 2, 3);
        var translatedString = new TranslatedString
        {
            [languageCode] = "a string"
        };

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
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1], 2, 3);
        var translatedString = new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "a string",
            [emptyLanguageCode] = "a string"
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
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1], 2, 256);
        var translatedString = new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "a string",
            [LanguagesList.HU.Iso1] = "a string"
        };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(@"x[""HU""]")
            .WithErrorMessage("Supported languages [EN,RO] do not include HU.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("aaaaaa")]
    public void Validation_ShouldFail_When_Translation_ExceedsLimits(string invalidTranslation)
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1], 2, 5);
        var translatedString = new TranslatedString
        {
            [LanguagesList.RO.Iso1] = "valid",
            [LanguagesList.EN.Iso1] = invalidTranslation
        };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(@"x[""EN""]")
            .WithErrorMessage("Translation for 'EN' must be between 2 and 5 characters.");
    }

    [Fact]
    public void Validation_ShouldFail_When_MissingLanguageInTranslation()
    {
        // Arrange
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1], 2, 5);
        var translatedString = new TranslatedString
        {
            [LanguagesList.RO.Iso1] = "valid"
        };

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
        var sut = new PartiallyTranslatedStringValidator([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1], 2, 5);
        var translatedString = new TranslatedString
        {
            [LanguagesList.RO.Iso1] = "valid",
            [LanguagesList.EN.Iso1] = ""
        };

        // Act
        var validationResult = sut.TestValidate(translatedString);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
