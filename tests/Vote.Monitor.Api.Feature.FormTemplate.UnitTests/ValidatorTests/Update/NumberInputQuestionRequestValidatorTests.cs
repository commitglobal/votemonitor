using FluentValidation.TestHelper;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests.Update;

public class NumberInputQuestionRequestValidatorTests
{
    private readonly NumberInputQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
    [MemberData(nameof(ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InputPlaceholderInvalid(TranslatedString inputPlaceholder)
    {
        // Arrange
        var numberInputQuestionRequest = new NumberInputQuestionRequest
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
        var numberInputQuestionRequest = new NumberInputQuestionRequest
        {
            InputPlaceholder = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
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
