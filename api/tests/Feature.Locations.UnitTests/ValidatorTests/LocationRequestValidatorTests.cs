namespace Feature.Locations.UnitTests.ValidatorTests;

public class LocationRequestValidatorTests
{
    private readonly Create.Validator.LocationRequestValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_LessThanZero()
    {
        // Arrange
        var request = new Create.Request.LocationRequest
        {
            DisplayOrder = -1
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }
}
