using Feature.FormTemplates.Create;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Module.Forms.Requests;

namespace Feature.FormTemplates.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Validator _sut = new();

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    [MemberData(nameof(TranslatedStringTestData.InvalidCodeTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_InvalidCode(string code)
    {
        // Arrange
        var request = new Request
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
        var request = new Request
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

    [Theory]
    [MemberData(nameof(TranslatedStringTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_DescriptionInvalid(TranslatedString invalidDescription)
    {
        // Arrange
        var request = new Request
        {
            Description = invalidDescription,
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
        var request = new Request();

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.FormType);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new Request
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
        var request = new Request
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
        var request = new Request
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
        var request = new Request
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
    public void Validation_ShouldFail_When_InvalidQuestion()
    {
        // Arrange
        var request = new Request
        {
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ],
            Questions = [
                new TextQuestionRequest
                {
                    Text = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First()
                },
                new TextQuestionRequest
                {
                    Text = TranslatedStringTestData.InvalidPartiallyTranslatedTestData.First()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Questions[1].Text[\"IT\"]");
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyQuestions()
    {
        // Arrange
        var request = new Request
        {
            Questions = []
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Questions);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            Languages = [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1],
            DefaultLanguage = LanguagesList.EN.Iso1,
            Code = "A code",
            FormType = FormType.ClosingAndCounting,
            Name = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First(),
            Description = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First()
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
