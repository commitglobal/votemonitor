namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.ValidatorTests;

public class RemoveObserverRequestValidatorTests
{
    private readonly RemoveObserver.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_ElectionRoundId_NotEmpty()
    {
        // Arrange
        var request = new RemoveObserver.Request { ElectionRoundId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new RemoveObserver.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ObserverId_NotEmpty()
    {
        // Arrange
        var request = new RemoveObserver.Request { ObserverId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new RemoveObserver.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }
}
