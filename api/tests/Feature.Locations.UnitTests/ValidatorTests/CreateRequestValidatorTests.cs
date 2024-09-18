namespace Feature.Locations.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundIdEmpty()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.Empty
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_LessThanZero()
    {
        // Arrange
        var request = new Create.Request
        {
            DisplayOrder = -1
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }
}