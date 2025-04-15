using FluentValidation.TestHelper;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Module.Forms.Requests;
using Module.Forms.Validators;
using Vote.Monitor.TestUtils.Utils;

namespace Module.Forms.UnitTests.Validators;

public class DisplayLogicRequestValidatorTests
{
    private readonly DisplayLogicRequestValidator _sut = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyParentId()
    {
        // Arrange
        var request = new DisplayLogicRequest
        {
            ParentQuestionId = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.ParentQuestionId);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptyCondition()
    {
        // Arrange
        var request = new DisplayLogicRequest
        {
            ParentQuestionId = Guid.Empty,
            Value = "1"
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Condition);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_ValueEmpty(string invalidValue)
    {
        // Arrange
        var request = new DisplayLogicRequest
        {
            Value = invalidValue
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validation_ShouldFail_When_ValueExceedsLimit()
    {
        // Arrange
        var request = new DisplayLogicRequest
        {
            Value = "1".Repeat(1025)
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new DisplayLogicRequest
        {
            ParentQuestionId = Guid.NewGuid(),
            Condition = DisplayLogicCondition.GreaterEqual,
            Value = "12"
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
