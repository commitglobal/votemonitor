
using FluentValidation.TestHelper;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Create.Validator validator = new();
    [Fact]
    public void Validate_DisplayOrder_GreaterThanOrEqualToZero_ShouldPass()
    {
        var request = new Create.Request
        {
            DisplayOrder = 5,
            Address = string.Empty,
            Tags = new()
        };
        var result = validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validate_DisplayOrder_LessThanZero_ShouldFail()
    {
        var request = new Create.Request
        {
            DisplayOrder = -1,
            Address = string.Empty,
            Tags = new()
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validate_Address_NotEmpty_ShouldPass()
    {
        var request = new Create.Request
        {
            Address = "123 Main St",
            DisplayOrder = 1,
            Tags = new()
        };
        var result = validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validate_Address_Empty_ShouldFail()
    {
        var request = new Create.Request
        {
            Address = "",
            DisplayOrder = 1,
            Tags = new()
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validate_Tags_NotEmpty_ShouldPass()
    {
        var request = new Create.Request
        {
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validate_Tags_Empty_ShouldFail()
    {
        var request = new Create.Request
        {
            Tags = new(),
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validate_Tags_Null_ShouldFail()
    {
        var request = new Create.Request
        {
            Tags = null,
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }
}
