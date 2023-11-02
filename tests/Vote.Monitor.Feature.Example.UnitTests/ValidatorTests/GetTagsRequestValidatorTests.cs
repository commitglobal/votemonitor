using FluentValidation.TestHelper;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class GetTagsRequestValidatorTests
{
    private readonly GetTagValues.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_SelectTag_NotEmpty()
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = "SomeValue" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SelectTag);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_SelectTag_Empty(string selectTag)
    {
        // Arrange
        var request = new GetTagValues.Request { SelectTag = selectTag };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectTag);
    }

    [Fact]
    public void Validation_ShouldPass_When_When_Filter_NotEmpty()
    {
        // Arrange
        var request = new GetTagValues.Request
        {
            SelectTag = "SomeValue",
            Filter = new Dictionary<string, string>
            {
                { "Key", "Value" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validation_ShouldPass_When_FilterKey_NotEmpty()
    {
        // Arrange
        var request = new GetTagValues.Request
        {
            SelectTag = "SomeValue",
            Filter = new Dictionary<string, string>
            {
                { "Key", "Value" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Filter.First().Key);
    }

    [Fact]
    public void Validation_ShouldFail_When_FilterKey_Empty()
    {
        // Arrange
        var request = new GetTagValues.Request
        {
            SelectTag = "SomeValue",
            Filter = new Dictionary<string, string>
            {
                { string.Empty, "Value" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Filter);
    }
}
