namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundIdEmpty()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.Empty,
            Address = "123 Main St",
            DisplayOrder = 1,
            Tags = new()
        };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Validation_ShouldPass_When_DisplayOrder_GreaterThanOrEqualToZero(int displayOrder)
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            DisplayOrder = displayOrder,
            Address = string.Empty,
            Tags = new()
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
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            DisplayOrder = -1,
            Address = string.Empty,
            Tags = new()
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
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Address = "123 Main St",
            DisplayOrder = 1,
            Tags = new()
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
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Address = address,
            DisplayOrder = 1,
            Tags = new()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validation_ShouldPass_When_Tags_NotEmpty()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Tags = new Dictionary<string, string>
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"}
            },
            DisplayOrder = 0,
            Address = string.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldFail_When_Tags_Empty_ShouldFail()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Tags = new(),
            DisplayOrder = 0,
            Address = string.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldFail_When_Tags_Null()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Tags = null,
            DisplayOrder = 0,
            Address = string.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_HaveEmptyKey(string key)
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { key, "Value" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }
}
