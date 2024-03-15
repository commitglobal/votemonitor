using FastEndpoints;
using FluentValidation;
using FluentValidation.TestHelper;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Answer.Module.Validators;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class MultiSelectAnswerRequestValidatorTests
{
    //public MultiSelectAnswerRequestValidator()
    //{
    //    RuleFor(x => x.QuestionId).NotEmpty();
    //    RuleFor(x => x.Selection).NotEmpty();
    //    RuleForEach(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    //}


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
    public void Validation_ShouldFail_When_EmptyDate()
    {
        // Arrange
        var request = new MultiSelectAnswerRequest();

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
        var request = new MultiSelectAnswerRequest
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
