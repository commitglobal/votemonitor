namespace Feature.Notifications.UnitTests.ValidatorTests;

public class MarkAsReadRequestValidatorTests
{
    private readonly MarkAsRead.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new MarkAsRead.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new MarkAsRead.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NotificationIds_Contains_Empty()
    {
        // Arrange
        var request = new MarkAsRead.Request { NotificationIds = [Guid.NewGuid(), Guid.Empty] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NotificationIds);
    }

    [Fact]
    public void Validation_ShouldPass_When_NotificationIds_Empty()
    {
        // Arrange
        var request = new MarkAsRead.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            NotificationIds = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new MarkAsRead.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            NotificationIds = [Guid.NewGuid(), Guid.NewGuid()]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}