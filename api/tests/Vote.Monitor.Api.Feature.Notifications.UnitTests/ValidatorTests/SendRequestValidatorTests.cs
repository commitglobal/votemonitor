using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Api.Feature.Notifications.UnitTests.ValidatorTests;

public class SendRequestValidatorTests
{
    private readonly Send.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Send.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ObserverIds_Empty()
    {
        // Arrange
        var request = new Send.Request { ObserverIds = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverIds);
    }

    [Fact]
    public void Validation_ShouldFail_When_ObserverIds_ContainsEmpty()
    {
        // Arrange
        var request = new Send.Request { ObserverIds = [Guid.NewGuid(), Guid.Empty,] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverIds);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Title_Empty(string title)
    {
        // Arrange
        var request = new Send.Request { Title = title };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldFail_When_Title_ExceedsLimit()
    {
        // Arrange
        var request = new Send.Request { Title = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Body_Empty(string body)
    {
        // Arrange
        var request = new Send.Request { Body = body };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldFail_When_Body_ExceedsLimit()
    {
        // Arrange
        var request = new Send.Request { Body = "a".Repeat(1025) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Send.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            ObserverIds = [Guid.NewGuid()],
            Title = "a title",
            Body = "a body"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
