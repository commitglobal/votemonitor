using FluentValidation.TestHelper;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class UpdateValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Validate_Id_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new Update.Request
        {
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
    public void Validate_Id_Empty_ShouldFail()
    {
        // Arrange
        var request = new Update.Request
        {
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

    [Fact]
    public void Validate_DisplayOrder_GreaterThanOrEqualToZero_ShouldPass()
    {
        // Arrange
        var request = new Update.Request
        {
            Id = Guid.NewGuid(),
            DisplayOrder = 10,
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
    public void Validate_Address_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new Update.Request
        {
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

    [Fact]
    public void Validate_Address_Empty_ShouldFail()
    {
        // Arrange
        var request = new Update.Request
        {
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "",
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
    public void Validate_Tags_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new Update.Request
        {
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

    [Fact]
    public void Validate_Tags_InvalidTag_ShouldFail()
    {
        // Arrange
        var request = new Update.Request
        {
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
                {
                    { "", "Value" }
                }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validate_Tags_Null_ShouldFail()
    {
        // Arrange
        var request = new Update.Request
        {
            Id = Guid.NewGuid(),
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }
}
