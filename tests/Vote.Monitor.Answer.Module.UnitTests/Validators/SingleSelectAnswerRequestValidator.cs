using FluentValidation.TestHelper;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Answer.Module.Validators;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class SingleSelectAnswerRequestValidatorTests
{
    private readonly SingleSelectAnswerRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyQuestionId()
    {
        // Arrange
        var request = new SingleSelectAnswerRequest()
        {
            QuestionId = Guid.Empty
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Fact]
    public void Validation_ShouldFail_When_InvalidSelection()
    {
        // Arrange
        var request = new SingleSelectAnswerRequest
        {
            Selection =
                new SelectedOptionRequest
                {
                    Text = "",
                    OptionId = Guid.Empty
                }
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x=>x.Selection.OptionId);
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptySelection()
    {
        // Arrange
        var request = new SingleSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = null
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_WhenValidRequest()
    {
        // Arrange
        var request = new SingleSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = new SelectedOptionRequest
            {
                Text = "a text",
                OptionId = Guid.NewGuid()
            }
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
