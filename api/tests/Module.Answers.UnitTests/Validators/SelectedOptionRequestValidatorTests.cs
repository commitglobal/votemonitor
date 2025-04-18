using FluentValidation.TestHelper;
using Module.Answers.Requests;
using Module.Answers.Validators;
using Vote.Monitor.TestUtils.Utils;
using Xunit;

namespace Module.Answers.UnitTests.Validators;

public class SelectedOptionRequestValidatorTests
{
    private readonly SelectedOptionRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyOptionId()
    {
        // Arrange
        var request = new SelectedOptionRequest
        {
            OptionId = Guid.Empty
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.OptionId);
    }

    [Fact]
    public void Validation_ShouldFail_When_TextExceedsLimits()
    {
        // Arrange
        var request = new SelectedOptionRequest { Text = "a".Repeat(1025) };

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
        var request = new SelectedOptionRequest
        {
            OptionId = Guid.NewGuid(),
            Text = "a text"
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
