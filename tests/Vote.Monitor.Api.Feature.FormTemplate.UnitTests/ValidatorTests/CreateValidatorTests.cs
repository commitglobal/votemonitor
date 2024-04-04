using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Create.Validator _sut = new();

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    [MemberData(nameof(TranslatedStringTestData.InvalidCodeTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_InvalidCode(string code)
    {
        // Arrange
        var request = new Create.Request
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(TranslatedStringTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_NameInvalid(TranslatedString invalidName)
    {
        // Arrange
        var request = new Create.Request
        {
            Name = invalidName,
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormTypeEmpty()
    {
        // Arrange
        var request = new Create.Request();

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.FormTemplateType);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new Create.Request
        {
            Languages = []
        };

        // Act
        var validationResult = _sut.TestValidate(request);

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
        var request = new Create.Request
        {
            Languages = [LanguagesList.RO.Iso1, invalidLanguageCode]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

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
        var request = new Create.Request
        {
            DefaultLanguage = invalidLanguageCode
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage);
    }

    [Fact]
    public void Validation_ShouldFail_When_DefaultLanguageNotInLanguageList()
    {
        // Arrange
        var request = new Create.Request
        {
            DefaultLanguage = LanguagesList.EN.Iso1,
            Languages = [LanguagesList.RO.Iso1]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage).WithErrorMessage("Languages should contain declared default language.");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Create.Request
        {
            Languages = [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1],
            DefaultLanguage = LanguagesList.EN.Iso1,
            Code = "A code",
            FormTemplateType = FormTemplateType.ClosingAndCounting,
            Name = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First()
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
