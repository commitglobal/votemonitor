using Vote.Monitor.TestUtils;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ValidatorTests;

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
            PageSize = pageSize,
            PageNumber = 1, 
            Filter = null
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
            PageSize = pageSize, 
            PageNumber = 1, 
            Filter = null
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
            PageSize = 10, 
            PageNumber = pageNumber, 
            Filter = null
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
            PageSize = 10, 
            PageNumber = pageNumber,
            Filter = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validation_ShouldPass_When_Filter_Valid()
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = 10, 
            PageNumber = 1,
            Filter = new Dictionary<string, string>
            {
                { "tag1", "value1" }, 
                { "tag2", "value2" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Filter_HasEmptyTag(string key)
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = 10,
            PageNumber = 1,
            Filter = new Dictionary<string, string>
            {
                { "tag1", "value1" },
                { key, "tag2" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validation_ShouldPass_When_Filter_Null()
    {
        // Arrange
        var request = new List.Request { PageSize = 10, PageNumber = 1, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }
}
