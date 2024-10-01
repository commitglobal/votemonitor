namespace Feature.Locations.UnitTests.ValidatorTests;

public class ListRequestValidatorTests
{
    private readonly List.Validator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validation_ShouldPass_When_PageSize_ValidValues(int pageSize)
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PageSize = pageSize,
            PageNumber = 1, 
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public void Validation_ShouldFail_When_PageSize_InvalidValues(int pageSize)
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PageSize = pageSize, 
            PageNumber = 1, 
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validation_ShouldPass_When_PageNumber_ValidValues(int pageNumber)
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PageSize = 10, 
            PageNumber = pageNumber, 
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validation_ShouldFail_When_PageNumber_InvalidValues(int pageNumber)
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PageSize = 10, 
            PageNumber = pageNumber,
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
