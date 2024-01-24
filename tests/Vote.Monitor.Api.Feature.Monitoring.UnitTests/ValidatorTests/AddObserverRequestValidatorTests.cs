namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.ValidatorTests;

public class AddObserverRequestValidatorTests
{
    private readonly AddObserver.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Id_NotEmpty()
    {
        // Arrange
        var request = new AddObserver.Request { Id = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new AddObserver.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldPass_When_ObserverId_NotEmpty()
    {
        // Arrange
        var request = new AddObserver.Request { ObserverId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ObserverId);
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
    public void Validation_ShouldPass_When_InviterNgoId_NotEmpty()
    {
        // Arrange
        var request = new AddObserver.Request { InviterNgoId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.InviterNgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_InviterNgoId_Empty()
    {
        // Arrange
        var request = new AddObserver.Request { InviterNgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InviterNgoId);
    }
}
