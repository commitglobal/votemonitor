using FluentValidation.TestHelper;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class ListValidatorTests
{
    private readonly List.Validator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_PageSize_ValidValues_ShouldPass(int pageSize)
    {
        // Arrange
        var request = new List.Request { PageSize = pageSize, PageNumber = 1, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public void Validate_PageSize_InvalidValues_ShouldFail(int pageSize)
    {
        // Arrange
        var request = new List.Request { PageSize = pageSize, PageNumber = 1, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validate_PageNumber_ValidValues_ShouldPass(int pageNumber)
    {
        // Arrange
        var request = new List.Request { PageSize = 10, PageNumber = pageNumber, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_PageNumber_InvalidValues_ShouldFail(int pageNumber)
    {
        // Arrange
        var request = new List.Request { PageSize = 10, PageNumber = pageNumber, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validate_Filter_Valid_ShouldPass()
    {
        // Arrange
        var request = new List.Request { PageSize = 10, PageNumber = 1, Filter = new Dictionary<string, string> { { "tag1", "value1" }, { "tag2", "value2" } } };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validate_Filter_InvalidTag_ShouldFail()
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = 10,
            PageNumber = 1,
            Filter = new Dictionary<string, string>
            {
                { "tag1", "value1" },
                { "", "tag2" }
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void Validate_Filter_Null_ShouldPass()
    {
        // Arrange
        var request = new List.Request { PageSize = 10, PageNumber = 1, Filter = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Filter);
    }
}
