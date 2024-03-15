using FluentValidation.TestHelper;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Answer.Module.Validators;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class MultiSelectAnswerRequestValidatorTests
{
    private readonly MultiSelectAnswerRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyQuestionId()
    {
        // Arrange
        var request = new MultiSelectAnswerRequest
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
        var request = new MultiSelectAnswerRequest
        {
            Selection = [
                new SelectedOptionRequest
                {
                    Text = "a text",
                    OptionId = Guid.NewGuid()
                },
                new SelectedOptionRequest
                {
                    Text = "",
                    OptionId = Guid.Empty
                },
            ]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Selection[1].OptionId");
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptySelection()
    {
        // Arrange
        var request = new MultiSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = []
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
        var request = new MultiSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = [
                new SelectedOptionRequest
                {
                    Text = "a text",
                    OptionId = Guid.NewGuid()
                }
            ]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
