using FluentValidation.TestHelper;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class GetTagsValidatorTests
{
    private readonly GetTagValues.Validator _validator = new();

    [Fact]
    public void Validate_SelectTag_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = "SomeValue" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SelectTag);
    }

    [Fact]
    public void Validate_SelectTag_Empty_ShouldFail()
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = string.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectTag);
    }

    [Fact]
    public void Validate_Filter_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = "SomeValue", Filter = new Dictionary<string, string> { { "Key", "Value" } } };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validate_Filter_Empty_ShouldFail()
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = "SomeValue", Filter = new Dictionary<string, string>() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validate_FilterKey_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new GetTagValues.Request
        {
            SelectTag = "SomeValue",
            Filter = new Dictionary<string, string> { { "Key", "Value" } }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Filter.First().Key);
    }

    [Fact]
    public void Validate_FilterKey_Empty_ShouldFail()
    {
        // Arrange
        var request = new GetTagValues.Request
        {
            SelectTag = "SomeValue",
            Filter = new Dictionary<string, string> { { string.Empty, "Value" } }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Filter);
    }
}
