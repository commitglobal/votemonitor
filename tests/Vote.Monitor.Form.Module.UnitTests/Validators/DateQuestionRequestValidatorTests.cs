using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.Validators;

public class DateQuestionRequestValidatorTests
{
    private readonly DateQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(ValidatorTests.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorTests.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorTests.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorTests.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(ValidatorTests.ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(ValidatorTests.ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = "code",
            Helptext = ValidatorTests.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorTests.ValidatorsTestData.ValidPartiallyTranslatedTestData.Last(),
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
