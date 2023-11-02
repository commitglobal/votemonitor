using FluentValidation.TestHelper;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Validation_ShouldPass_When_DisplayOrder_GreaterThanOrEqualToZero(int displayOrder)
    {
        var request = new Create.Request
        {
            DisplayOrder = displayOrder,
            Address = string.Empty,
            Tags = new()
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_LessThanZero()
    {
        var request = new Create.Request
        {
            DisplayOrder = -1,
            Address = string.Empty,
            Tags = new()
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldPass_When_Address_NotEmpty()
    {
        var request = new Create.Request
        {
            Address = "123 Main St",
            DisplayOrder = 1,
            Tags = new()
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Address_Empty(string address)
    {
        var request = new Create.Request
        {
            Address = address,
            DisplayOrder = 1,
            Tags = new()
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validation_ShouldPass_When_Tags_NotEmpty()
    {
        var request = new Create.Request
        {
            Tags = new Dictionary<string, string>
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldFail_When_Tags_Empty_ShouldFail()
    {
        var request = new Create.Request
        {
            Tags = new(),
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldFail_When_Tags_Null()
    {
        var request = new Create.Request
        {
            Tags = null,
            DisplayOrder = 0,
            Address = string.Empty
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldFail_When_Tags_HaveEmptyKey()
    {
        // Arrange
        var request = new Create.Request
        {
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
}
