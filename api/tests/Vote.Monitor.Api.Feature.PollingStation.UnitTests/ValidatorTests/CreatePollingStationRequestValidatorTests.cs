namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ValidatorTests;

public class CreatePollingStationRequestValidatorTests
{
    private readonly Create.Validator.PollingStationRequestValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Validation_ShouldPass_When_DisplayOrder_GreaterThanOrEqualToZero(int displayOrder)
    {
        // Arrange
        var request = new Create.Request.PollingStationRequest()
        {
            DisplayOrder = displayOrder, Address = string.Empty, Tags = new()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_LessThanZero()
    {
        // Arrange
        var request = new Create.Request.PollingStationRequest()
        {
            DisplayOrder = -1, Address = string.Empty, Tags = new()
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldPass_When_Address_NotEmpty()
    {
        // Arrange
        var request = new Create.Request.PollingStationRequest()
        {
            Address = "123 Main St", DisplayOrder = 1, Tags = new()
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Address_Empty(string address)
    {
        // Arrange
        var request = new Create.Request.PollingStationRequest() { Address = address, DisplayOrder = 1, Tags = new() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validation_ShouldPass_When_Tags_NotEmpty()
    {
        // Arrange
        var request = new Create.Request.PollingStationRequest()
        {
            Tags = new Dictionary<string, string> { { "Tag1", "value1" }, { "Tag2", "value2" } },
            DisplayOrder = 0,
            Address = string.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }
}
