namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.ValidatorTests;

public class AddNgoRequestValidatorTests
{
    private readonly AddNgo.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Id_NotEmpty()
    {
        // Arrange
        var request = new AddNgo.Request { Id = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new AddNgo.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
    [Fact]
    public void Validation_ShouldPass_When_NgoId_NotEmpty()
    {
        // Arrange
        var request = new AddNgo.Request { NgoId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new AddNgo.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }
}
