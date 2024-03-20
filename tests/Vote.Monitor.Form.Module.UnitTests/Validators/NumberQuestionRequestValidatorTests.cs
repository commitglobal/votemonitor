using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.Validators;

public class NumberQuestionRequestValidatorTests
{
    private readonly NumberQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

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
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_InputPlaceholderInvalid(TranslatedString inputPlaceholder)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            InputPlaceholder = inputPlaceholder
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.InputPlaceholder);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var numberInputQuestionRequest = new NumberQuestionRequest
        {
            InputPlaceholder = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Helptext = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "a code",
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(numberInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
