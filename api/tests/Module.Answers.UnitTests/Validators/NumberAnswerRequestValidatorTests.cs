﻿using FluentValidation.TestHelper;
using Module.Answers.Requests;
using Module.Answers.Validators;
using Xunit;

namespace Module.Answers.UnitTests.Validators;

public class NumberAnswerRequestValidatorTests
{
    private readonly NumberAnswerRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyQuestionId()
    {
        // Arrange
        var request = new NumberAnswerRequest
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
    public void Validation_ShouldFail_When_ValueNegative()
    {
        // Arrange
        var request = new NumberAnswerRequest { Value = -1 };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Validation_ShouldPass_WhenValidRequest(int value)
    {
        // Arrange
        var request = new NumberAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Value = value
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
