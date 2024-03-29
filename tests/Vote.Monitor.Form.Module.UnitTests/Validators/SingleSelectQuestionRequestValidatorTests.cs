using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.Validators;

public class SingleSelectQuestionRequestValidatorTests
{
    private readonly SingleSelectQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

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
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validation_ShouldFail_When_SomeOptionsAreInvalid()
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Options = [
                new SelectOptionRequest
                {
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last()
                },
                new SelectOptionRequest
                {
                    Text = ValidatorsTestData.InvalidPartiallyTranslatedTestData.Last()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Options[1].Text");
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyOptions()
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Options = []
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Options);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var selectQuestionRequest = new SingleSelectQuestionRequest
        {
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Id = Guid.NewGuid(),
            Options = [
                new SelectOptionRequest
                {
                    Id = Guid.NewGuid(),
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(selectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
