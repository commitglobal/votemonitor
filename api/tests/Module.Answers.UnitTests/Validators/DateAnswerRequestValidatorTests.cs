using FluentValidation.TestHelper;
using Module.Answers.Requests;
using Module.Answers.Validators;
using Xunit;

namespace Module.Answers.UnitTests.Validators;

public class DateAnswerRequestValidatorTests
{
    private readonly DateAnswerRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyQuestionId()
    {
        // Arrange
        var request = new DateAnswerRequest
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
    public void Validation_ShouldFail_When_EmptyDate()
    {
        // Arrange
        var request = new DateAnswerRequest();

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public void Validation_ShouldPass_WhenValidRequest()
    {
        // Arrange
        var request = new DateAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Date = DateTime.UtcNow
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
