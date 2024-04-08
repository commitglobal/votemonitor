namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class AddObserverRequestValidatorTests
{
    private readonly AddObserver.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new AddObserver.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new AddObserver.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringNgoId_Empty()
    {
        // Arrange
        var request = new AddObserver.Request { MonitoringNgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringNgoId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new AddObserver.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            MonitoringNgoId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
