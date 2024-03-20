using Vote.Monitor.Api.Feature.FormTemplate.Update;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests;

public class UpdateValidatorTests
{
    private readonly Validator _sut = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var request = new Request
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidId()
    {
        // Arrange
        var request = new Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
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
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(TranslatedStringTestData.InvalidCodeTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
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

    [Fact]
    public void Validation_ShouldPass_When_CodeValid()
    {
        // Arrange
        var request = new Request
        {
            Code = "code"
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Code);
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
    [MemberData(nameof(TranslatedStringTestData.ValidPartiallyTranslatedTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldPass_When_NameValid(TranslatedString validName)
    {
        // Arrange
        var request = new Request
        {
            Name = validName,
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Name);
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
    public void Validation_ShouldFail_When_EmptyLanguageCodes(string invalidLanguageCode)
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

    [Fact]
    public void Validation_ShouldFail_When_ValidSupportedLanguages()
    {
        // Arrange
        var request = new Request
        {
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Languages);
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
    public void Validation_ShouldFail_When_EmptyFormType()
    {
        // Arrange
        var request = new Request();

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.FormTemplateType);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidFormType()
    {
        // Arrange
        var request = new Request
        {
            FormTemplateType = FormTemplateType.ClosingAndCounting
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.FormTemplateType);
    }
}
