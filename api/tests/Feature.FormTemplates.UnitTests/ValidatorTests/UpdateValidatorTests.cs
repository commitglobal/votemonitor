﻿using Feature.FormTemplates.Update;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;

namespace Feature.FormTemplates.UnitTests.ValidatorTests;

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
    [InlineData("unknown iso")]
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
    [InlineData("unknown iso")]
    public void Validation_ShouldFail_When_InvalidDefaultLanguageCode(string invalidLanguageCode)
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
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            Id = Guid.NewGuid(),
            FormTemplateType = FormTemplateType.ClosingAndCounting,
            DefaultLanguage = LanguagesList.EN.Iso1,
            Languages = [LanguagesList.EN.Iso1],
            Code = "c!",
            Questions = [],
            Name = new TranslatedStringFaker([LanguagesList.EN.Iso1]).Generate(),
            Description = new TranslatedStringFaker([LanguagesList.EN.Iso1]).Generate(),
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
