
using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Api.Feature.Notifications.UnitTests.ValidatorTests;

public class SubscribeRequestValidatorTests
{
    private readonly Subscribe.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new Subscribe.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Token_ExceedsLimit()
    {
        // Arrange
        var request = new Subscribe.Request { Token = "a".Repeat(1025) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Token_Empty(string token)
    {
        // Arrange
        var request = new Subscribe.Request { Token = token };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Subscribe.Request
        {
            ObserverId = Guid.NewGuid(),
            Token = "a token"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
