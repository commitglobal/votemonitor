using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.Validators;

public class TextInputQuestionRequestValidatorTests
{
    private readonly TextInputQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_InputPlaceholderInvalid(TranslatedString inputPlaceholder)
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            InputPlaceholder = inputPlaceholder
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.InputPlaceholder);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var textInputQuestionRequest = new TextQuestionRequest
        {
            Helptext = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Id = Guid.NewGuid(),
            InputPlaceholder = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First()
        };

        // Act
        var validationResult = _sut.TestValidate(textInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
