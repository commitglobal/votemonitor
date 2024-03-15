using FastEndpoints;
using FluentValidation.TestHelper;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Answer.Module.Validators;
using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class TextAnswerRequestValidatorTests //: Validator<TextAnswerRequest>
{
    //public TextAnswerRequestValidator()
    //{
    //    RuleFor(x => x.QuestionId).NotEmpty();
    //    RuleFor(x => x.Text).NotEmpty().MaximumLength(1024);
    //}



    private readonly TextAnswerRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyQuestionId()
    {
        // Arrange
        var request = new TextAnswerRequest
        {
            QuestionId = Guid.Empty
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_TextEmpty(string text)
    {
        // Arrange
        var request = new TextAnswerRequest { Text = text };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validation_ShouldFail_When_TextExceedsLimits()
    {
        // Arrange
        var request = new TextAnswerRequest { Text = "a".Repeat(1025) };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validation_ShouldPass_WhenValidRequest()
    {
        // Arrange
        var request = new TextAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Text = "a text"
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
