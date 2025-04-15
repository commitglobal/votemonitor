namespace Feature.PollingStations.UnitTests.ValidatorTests;

public class UpdateRequestValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Id_NotEmpty()
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.Empty,
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Validation_ShouldPass_When_DisplayOrder_GreaterThanOrEqualToZero(int displayOrder)
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DisplayOrder = displayOrder,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldPass_When_Address_NotEmpty()
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
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
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = address,
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
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
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "Tag1", "Value1" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_HaveEmptyKey(string key)
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
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
