namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.ValidatorTests;

public class ArchiveRequestValidatorTests
{
    private readonly Archive.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Id_NotEmpty()
    {
        // Arrange
        var request = new Archive.Request { Id = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Archive.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
